using ERAM_2_GEOJSON.Models;
using Newtonsoft.Json;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERAM_2_GEOJSON.Helpers
{
    public class GeoJsonGeneratorByAttributes
    {
        /// <summary>
        /// Generates geojson files combining data that share identical attributes. Must be chosen output format by user.
        /// </summary>
        public void GenerateGeoJson(List<GeoMapRecord> geoMapRecords, string outputDirectory, bool includeCustomProperties)
        {
            foreach (var record in geoMapRecords)
            {
                // Sanitize labels for directory name creation
                string sanitizedGeomapId = FileNameSanitizer.SanitizeForFileName(record.GeomapId);
                string sanitizedLabelLine1 = FileNameSanitizer.SanitizeForFileName(record.LabelLine1);
                string sanitizedLabelLine2 = FileNameSanitizer.SanitizeForFileName(record.LabelLine2);

                // Creates the output directory subdirectory name by GeomapID and then the label line 1 and 2. Passes it in to be created.
                string recordDirectory = Path.Combine(outputDirectory, $"{sanitizedGeomapId}_{sanitizedLabelLine1}-{sanitizedLabelLine2}");
                Directory.CreateDirectory(recordDirectory);

                ProcessLines(record.ObjectTypes, recordDirectory, includeCustomProperties);
                ProcessSymbols(record.ObjectTypes, recordDirectory, includeCustomProperties);
                ProcessText(record.ObjectTypes, recordDirectory, includeCustomProperties);
            }
        }

        private void ProcessLines(List<GeoMapObjectType> objectTypes, string recordDirectory, bool includeCustomProperties)
        {
            // Dictionary to group lines by a unique file key, each key maps to a list of line segments (IPosition)
            var groupedLines = new Dictionary<string, List<List<IPosition>>>();

            // Dictionary to store properties associated with each file key
            var linePropertiesMap = new Dictionary<string, Dictionary<string, object>>();

            // Iterate over each object type
            foreach (var objectType in objectTypes)
            {
                // Iterate over all lines in the current object type
                foreach (var line in objectType.Lines)
                {
                    // Generate a unique file key based on the object type and line attributes
                    string fileKey = ConstructLineFileKey(objectType, line);

                    // Initialize entry in groupedLines and linePropertiesMap if fileKey is new
                    if (!groupedLines.ContainsKey(fileKey))
                    {
                        groupedLines[fileKey] = new List<List<IPosition>>();
                        linePropertiesMap[fileKey] = CreateLineProperties(objectType, line, includeCustomProperties);
                    }

                    // Convert start and end coordinates of the line from DMS to decimal format
                    var start = new Position(
                        CoordinateConverter.ConvertDMSToDecimal(line.StartLatitude),
                        CoordinateConverter.ConvertDMSToDecimal(line.StartLongitude)
                    );
                    var end = new Position(
                        CoordinateConverter.ConvertDMSToDecimal(line.EndLatitude),
                        CoordinateConverter.ConvertDMSToDecimal(line.EndLongitude)
                    );

                    // "Efficient LineString Handling"
                    // Check if the last segment in the group ends where the new line starts
                    if (groupedLines[fileKey].Count > 0 &&
                        groupedLines[fileKey].Last().Last().Equals(start))
                    {
                        // Extend the last segment with the new endpoint
                        groupedLines[fileKey].Last().Add(end);
                    }
                    else
                    {
                        // Create a new segment starting with the current line's start and end points
                        groupedLines[fileKey].Add(new List<IPosition> { start, end });
                    }
                }
            }

            // Write each group of lines to a GeoJSON file
            foreach (var group in groupedLines)
            {
                // Sanitize the file key to create a valid file name
                string filePath = Path.Combine(recordDirectory, SanitizeFileName(group.Key));

                // Create a MultiLineString from the grouped line segments
                var multiLineString = new MultiLineString(group.Value.Select(line => new LineString(line)));

                // Create a feature collection and add the MultiLineString as a feature with its properties
                var featureCollection = new FeatureCollection();
                featureCollection.Features.Add(new Feature(multiLineString, linePropertiesMap[group.Key]));

                // Write the feature collection to a GeoJSON file
                WriteGeoJsonToFile(filePath, featureCollection);
            }
        }

        private void ProcessSymbols(List<GeoMapObjectType> objectTypes, string recordDirectory, bool includeCustomProperties)
        {
            // Dictionary to group symbols by a unique file key
            var groupedSymbols = new Dictionary<string, List<Feature>>();

            // Iterate over each object type
            foreach (var objectType in objectTypes)
            {
                // Iterate over all symbols in the current object type
                foreach (var symbol in objectType.Symbols)
                {
                    // Generate a unique file key based on the object type and symbol attributes
                    string fileKey = ConstructSymbolFileKey(objectType, symbol);

                    // Initialize entry in groupedSymbols if the fileKey is new
                    if (!groupedSymbols.ContainsKey(fileKey))
                    {
                        groupedSymbols[fileKey] = new List<Feature>();
                    }

                    // Convert the symbol's latitude and longitude from DMS to decimal format
                    var position = new Position(
                        CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                        CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude)
                    );

                    // Create properties for the symbol if includeCustomProperties is true
                    var properties = includeCustomProperties
                        ? CreateSymbolProperties(objectType, symbol)
                        : null;

                    // Add the symbol as a GeoJSON feature with its position and properties
                    groupedSymbols[fileKey].Add(new Feature(new GeoJSON.Net.Geometry.Point(position), properties));
                }
            }

            // Write each group of symbols to a GeoJSON file
            foreach (var group in groupedSymbols)
            {
                // Sanitize the file key to create a valid file name
                string filePath = Path.Combine(recordDirectory, SanitizeFileName(group.Key));

                // Create a feature collection containing all symbols in the group
                var featureCollection = new FeatureCollection(group.Value);

                // Write the feature collection to a GeoJSON file
                WriteGeoJsonToFile(filePath, featureCollection);
            }
        }

        private void ProcessText(List<GeoMapObjectType> objectTypes, string recordDirectory, bool includeCustomProperties)
        {
            // Dictionary to group valid text objects by a unique file key
            var groupedText = new Dictionary<string, List<Feature>>();

            // Dictionary to group symbols that do not have associated valid text objects
            var groupedSymbols = new Dictionary<string, List<Feature>>(); // Handle symbols separately

            // Iterate over each object type
            foreach (var objectType in objectTypes)
            {
                // Iterate over each symbol in the current object type
                foreach (var symbol in objectType.Symbols)
                {
                    bool hasProcessedText = false; // Track whether text objects were processed for this symbol

                    // Iterate over text objects associated with the current symbol
                    foreach (var textObject in symbol.TextObjects)
                    {
                        // Skip text objects that have their AppliedTextDisplaySetting set to false
                        if (textObject.AppliedTextDisplaySetting == false)
                        {
                            continue;
                        }

                        hasProcessedText = true; // Mark that at least one text object was processed

                        // Generate a unique file key for the text object
                        string fileKey = ConstructTextFileKey(objectType, symbol, textObject);

                        // Initialize the groupedText entry if the fileKey is new
                        if (!groupedText.ContainsKey(fileKey))
                        {
                            groupedText[fileKey] = new List<Feature>();
                        }

                        // Convert the symbol's latitude and longitude to a position
                        var position = new Position(
                            CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                            CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude)
                        );

                        // Create properties for the text object, including custom properties if requested
                        var properties = CreateTextProperties(objectType, symbol, textObject, includeCustomProperties);

                        // Add the text object as a GeoJSON feature
                        groupedText[fileKey].Add(new Feature(new GeoJSON.Net.Geometry.Point(position), properties));
                    }

                    // If no valid text objects were processed, treat the symbol as a standalone feature
                    if (!hasProcessedText)
                    {
                        // Generate a unique file key for the symbol
                        string symbolFileKey = ConstructSymbolFileKey(objectType, symbol);

                        // Initialize the groupedSymbols entry if the fileKey is new
                        if (!groupedSymbols.ContainsKey(symbolFileKey))
                        {
                            groupedSymbols[symbolFileKey] = new List<Feature>();
                        }

                        // Convert the symbol's latitude and longitude to a position
                        var position = new Position(
                            CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                            CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude)
                        );

                        // Create properties for the symbol
                        var symbolProperties = CreateSymbolProperties(objectType, symbol);

                        // Add the symbol as a GeoJSON feature
                        groupedSymbols[symbolFileKey].Add(new Feature(new GeoJSON.Net.Geometry.Point(position), symbolProperties));
                    }
                }
            }

            // Write grouped text objects to GeoJSON files
            foreach (var group in groupedText)
            {
                string filePath = Path.Combine(recordDirectory, SanitizeFileName(group.Key));
                var featureCollection = new FeatureCollection(group.Value);
                WriteGeoJsonToFile(filePath, featureCollection);
            }

            // Write grouped symbols for which no text objects were processed
            foreach (var group in groupedSymbols)
            {
                string filePath = Path.Combine(recordDirectory, SanitizeFileName(group.Key));
                var featureCollection = new FeatureCollection(group.Value);
                WriteGeoJsonToFile(filePath, featureCollection);
            }
        }

        // Constructs a unique file key for GeoMapLine objects based on their attributes
        // BCG and Filters are always padded to be 2 digits to help with sorting by name in a file broswer window.
        private string ConstructLineFileKey(GeoMapObjectType objectType, GeoMapLine line)
        {
            return $"BCG {((int)line.AppliedLineBcgGroup).ToString("D2")}_Filters {string.Join(" ", line.AppliedLineFilters.OrderBy(f => f).Select(f => f.ToString("D2")))}_Type {objectType.MapObjectType}_Group {objectType.MapGroupId}_Object {line.LineObjectId}_Style {line.AppliedLineStyle}_Thick {line.AppliedLineThickness}_Lines.geojson";
        }

        // Constructs a unique file key for GeoMapSymbol objects based on their attributes
        private string ConstructSymbolFileKey(GeoMapObjectType objectType, GeoMapSymbol symbol)
        {
            return $"BCG {((int)symbol.AppliedSymbolBcgGroup).ToString("D2")}_Filters {string.Join(" ", symbol.AppliedSymbolFilters.OrderBy(f => f).Select(f => f.ToString("D2")))}_Type {objectType.MapObjectType}_Group {objectType.MapGroupId}_Style {symbol.AppliedSymbolStyle}_Font {symbol.AppliedSymbolFontSize}_Symbols.geojson";
        }

        // Constructs a unique file key for GeoMapText objects based on their attributes
        private string ConstructTextFileKey(GeoMapObjectType objectType, GeoMapSymbol symbol, GeoMapText textObject)
        {
            return $"BCG {((int)textObject.AppliedTextBcgGroup).ToString("D2")}_Filters {string.Join(" ", textObject.AppliedTextFilters.OrderBy(f => f).Select(f => f.ToString("D2")))}_Type {objectType.MapObjectType}_Group {objectType.MapGroupId}_Font {textObject.AppliedTextFontSize}_Underline {(textObject.AppliedTextUnderline ?? false ? "T" : "F")}_X {textObject.AppliedTextXPixelOffset}_Y {textObject.AppliedTextYPixelOffset}_Text.geojson";
        }

        // Creates a dictionary of properties for a GeoMapLine object if custom properties are enabled
        private Dictionary<string, object> CreateLineProperties(GeoMapObjectType objectType, GeoMapLine line, bool includeCustomProperties)
        {
            // Return null if custom properties are not to be included
            if (!includeCustomProperties) return null;

            // Return a dictionary containing debug and organizational properties
            return new Dictionary<string, object>
            {
                { "E2G_MapObjectType", objectType.MapObjectType }, // Type of the map object (e.g., "AIRWAY")
                { "E2G_MapGroupId", objectType.MapGroupId },       // Group ID associated with the object
                { "E2G_LineObjectId", line.LineObjectId }          // Unique ID for the line object
            };
        }

        // Creates a dictionary of properties for a GeoMapSymbol object
        private Dictionary<string, object> CreateSymbolProperties(GeoMapObjectType objectType, GeoMapSymbol symbol)
        {
            // Return a dictionary containing debug and organizational properties
            return new Dictionary<string, object>
            {
                { "E2G_MapObjectType", objectType.MapObjectType }, // Geomap MapObjectType (e.g., "VOR")
                { "E2G_MapGroupId", objectType.MapGroupId },       // Geomap MapGroupId
                { "E2G_SymbolId", symbol.SymbolId }                // Geomap SymbolId
            };
        }

        // Creates a dictionary of properties for a GeoMapText object
        private Dictionary<string, object> CreateTextProperties(GeoMapObjectType objectType, GeoMapSymbol symbol, GeoMapText textObject, bool includeCustomProperties)
        {
            // Initialize a dictionary with the basic "text" property containing the text lines
            var properties = new Dictionary<string, object>
            {
                { "text", textObject.TextLines } // The text lines to be included in the GeoJSON feature
            };

            // Add custom properties if includeCustomProperties is true
            if (includeCustomProperties)
            {
                properties.Add("E2G_MapObjectType", objectType.MapObjectType); // Geomap MapObjectType (e.g., "WAYPOINT")
                properties.Add("E2G_MapGroupId", objectType.MapGroupId);       // Geomap MapGroupId
                properties.Add("E2G_SymbolId", symbol.SymbolId);               // Geomap SymbolId
            }

            // Return the dictionary of properties
            return properties;
        }

        // Writes a GeoJSON FeatureCollection to a file
        private void WriteGeoJsonToFile(string filePath, FeatureCollection featureCollection)
        {
            try
            {
                // Serialize the FeatureCollection object to a GeoJSON-formatted string
                string geoJsonContent = JsonConvert.SerializeObject(featureCollection, Formatting.None);

                // Write the serialized GeoJSON string to the specified file path
                File.WriteAllText(filePath, geoJsonContent);
            }
            catch (Exception ex)
            {
                // Log an error message if any exception occurs during the file write operation
                Console.WriteLine($"An error occurred while writing GeoJSON to file: {ex.Message}");
            }
        }

        // Removes invalid characters from a file name
        private string SanitizeFileName(string input)
        {
            // Remove all characters that are not allowed in file names
            return string.Concat(input.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
