using ERAM_2_GEOJSON.Models;
using Newtonsoft.Json;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERAM_2_GEOJSON.Helpers
{
    public class GeoJsonGeneratorByFilters
    {
        /// <summary>
        /// Generates GeoJSON files by grouping data with identical filters. 
        /// The output format must be specified by the user.
        /// </summary>
        /// <param name="geoMapRecords">List of GeoMap records to process.</param>
        /// <param name="outputDirectory">Directory where the output files will be saved.</param>
        /// <param name="includeCustomProperties">Whether to include custom properties for debugging and organization.</param>
        
        public void GenerateGeoJson(List<GeoMapRecord> geoMapRecords, string outputDirectory, bool includeCustomProperties)
        {
            // Iterate through each GeoMapRecord in the input list
            foreach (var record in geoMapRecords)
            {
                // Sanitize labels for directory name creation
                string sanitizedGeomapId = FileNameSanitizer.SanitizeForFileName(record.GeomapId);
                string sanitizedLabelLine1 = FileNameSanitizer.SanitizeForFileName(record.LabelLine1);
                string sanitizedLabelLine2 = FileNameSanitizer.SanitizeForFileName(record.LabelLine2);

                // Create a directory for the current record, combining its ID and labels
                string recordDirectory = Path.Combine(outputDirectory, $"{sanitizedGeomapId}_{sanitizedLabelLine1}-{sanitizedLabelLine2}");
                EnsureDirectoryExists(outputDirectory, recordDirectory); // Ensure the directory exists

                // Dictionary to group features by filter for the current record
                var featuresByFilter = new Dictionary<string, FeatureCollection>();

                // Process each object type in the record
                foreach (var objectType in record.ObjectTypes)
                {
                    // Process lines within the object type, grouped by filter
                    ProcessLines(objectType, recordDirectory, featuresByFilter, includeCustomProperties);

                    // Process symbols within the object type, grouped by filter
                    ProcessSymbols(objectType, recordDirectory, featuresByFilter, includeCustomProperties);

                    // Process text objects within the object type, grouped by filter
                    ProcessText(objectType, recordDirectory, featuresByFilter, includeCustomProperties);
                }

                // Write the grouped features to GeoJSON files in the record's directory
                WriteGeoJsonToFiles(featuresByFilter, recordDirectory);
            }
        }

        // Processes GeoMapLine objects, grouping them by filters and adding them to the feature collection
        private void ProcessLines(GeoMapObjectType objectType, string recordDirectory, Dictionary<string, FeatureCollection> featuresByFilter, bool includeCustomProperties)
        {
            // Dictionary to group line segments by their unique filter key
            var groupedLines = new Dictionary<string, List<List<IPosition>>>();

            // Iterate through all lines in the given object type
            foreach (var line in objectType.Lines)
            {
                // Generate a unique key for the filter group and specify "_Lines" for file differentiation
                string filterKey = ConstructFilterKey(line.AppliedLineFilters, "_Lines");

                // Ensure a directory exists for the given filter key
                string filterDirectory = EnsureDirectoryExists(recordDirectory, filterKey);

                // Initialize the group if it doesn't already exist
                if (!groupedLines.ContainsKey(filterKey))
                {
                    groupedLines[filterKey] = new List<List<IPosition>>();
                }

                // Convert start and end coordinates from DMS to decimal format
                var start = new Position(
                    CoordinateConverter.ConvertDMSToDecimal(line.StartLatitude),
                    CoordinateConverter.ConvertDMSToDecimal(line.StartLongitude)
                );
                var end = new Position(
                    CoordinateConverter.ConvertDMSToDecimal(line.EndLatitude),
                    CoordinateConverter.ConvertDMSToDecimal(line.EndLongitude)
                );

                // Check if the current line continues from the last segment in the group
                if (groupedLines[filterKey].Count > 0 &&
                    groupedLines[filterKey].Last().Last().Equals(start))
                {
                    // Extend the last LineString with the new endpoint
                    groupedLines[filterKey].Last().Add(end); // Extend the last LineString
                }
                else
                {
                    // Create a new LineString with the current start and end points
                    groupedLines[filterKey].Add(new List<IPosition> { start, end }); // Create a new LineString
                }
            }

            // Add the grouped lines to the feature collection for further processing
            AddGroupedLinesToFeatures(groupedLines, featuresByFilter, objectType, includeCustomProperties);
        }

        // Processes GeoMapSymbol objects, grouping them by filters and adding them as point features
        private void ProcessSymbols(GeoMapObjectType objectType, string recordDirectory, Dictionary<string, FeatureCollection> featuresByFilter, bool includeCustomProperties)
        {
            // Iterate over all symbols in the given object type
            foreach (var symbol in objectType.Symbols)
            {
                // Generate a unique key for the filter group and specify "_Symbols" for file differentiation
                string filterKey = ConstructFilterKey(symbol.AppliedSymbolFilters, "_Symbols");

                // Ensure a directory exists for the given filter key
                string filterDirectory = EnsureDirectoryExists(recordDirectory, filterKey);

                // Add the symbol as a point feature to the feature collection
                AddPointToFeatures(featuresByFilter, filterKey, symbol, includeCustomProperties, objectType);
            }
        }

        // Processes GeoMapText objects, grouping them by filters and adding them as text features
        private void ProcessText(GeoMapObjectType objectType, string recordDirectory, Dictionary<string, FeatureCollection> featuresByFilter, bool includeCustomProperties)
        {
            // Iterate over all symbols in the given object type
            foreach (var symbol in objectType.Symbols)
            {
                // Iterate over all text objects associated with the current symbol
                foreach (var textObject in symbol.TextObjects)
                {
                    // Generate a unique key for the filter group and specify "_Text" for file differentiation
                    string filterKey = ConstructFilterKey(textObject.AppliedTextFilters, "_Text");

                    // Ensure a directory exists for the given filter key
                    string filterDirectory = EnsureDirectoryExists(recordDirectory, filterKey);

                    // Add the text object as a point feature with associated properties
                    AddTextPointToFeatures(featuresByFilter, filterKey, textObject, symbol, includeCustomProperties, objectType);
                }
            }
        }

        // Constructs a unique filter key based on a list of filters and a suffix
        private string ConstructFilterKey(List<int> filters, string suffix)
        {
            // If there's only one filter, create a simple key in the format "Filter_##"
            string baseKey = filters.Count == 1
                ? $"Filter_{filters[0]:D2}"
                // If multiple filters exist, create a composite key in the format "Multi-Filter_##_##_..."
                : $"Multi-Filter_{string.Join("_", filters.OrderBy(f => f).Select(f => f.ToString("D2")))}";

            // Append the suffix (e.g., "_Symbols", "_Text", "_Lines") to distinguish file types
            return baseKey + suffix;
        }

        // Ensures a directory exists for the given filter key and returns its path
        private string EnsureDirectoryExists(string baseDirectory, string filterKey)
        {
            // Remove type-specific suffixes from the filter key to create a cleaned directory name
            string cleanedKey = filterKey.Replace("_Symbols", "").Replace("_Text", "").Replace("_Lines", "");

            // Combine the base directory with the cleaned key to form the directory path
            string directoryPath = Path.Combine(baseDirectory, cleanedKey);

            // Create the directory if it doesn't already exist
            Directory.CreateDirectory(directoryPath);

            // Return the full path of the directory
            return directoryPath;
        }

        // Adds grouped lines to the feature collection, associating them with their corresponding filter keys
        private void AddGroupedLinesToFeatures(Dictionary<string, List<List<IPosition>>> groupedLines, Dictionary<string, FeatureCollection> featuresByFilter, GeoMapObjectType objectType, bool includeCustomProperties)
        {
            // Iterate over each group of lines, where the key is the filter key and the value is the list of line segments
            foreach (var group in groupedLines)
            {
                string filterKey = group.Key; // The unique key representing this group of filters

                // Ensure the feature collection exists for the current filter key
                if (!featuresByFilter.ContainsKey(filterKey))
                {
                    featuresByFilter[filterKey] = new FeatureCollection();
                }

                // Create a MultiLineString from the grouped line segments
                var multiLineString = new MultiLineString(group.Value.Select(line => new LineString(line)));

                // Create properties for the MultiLineString if custom properties are enabled
                var properties = includeCustomProperties
                    ? new Dictionary<string, object>
                    {
                        { "E2G_MapObjectType", objectType.MapObjectType },                      // Type of the map object (e.g., "AIRWAY")
                        { "E2G_MapGroupId", objectType.MapGroupId },                            // Group ID associated with the object
                        { "E2G_LineObjectId", objectType.Lines.FirstOrDefault()?.LineObjectId } // Unique ID for the line object
                    }
                    : null; // No properties if custom properties are disabled

                // Add the MultiLineString as a GeoJSON feature to the feature collection
                featuresByFilter[filterKey].Features.Add(new Feature(multiLineString, properties));
            }
        }

        // Adds a symbol as a point feature to the feature collection associated with the specified filter key
        private void AddPointToFeatures(Dictionary<string, FeatureCollection> featuresByFilter, string filterKey, GeoMapSymbol symbol, bool includeCustomProperties, GeoMapObjectType objectType)
        {
            // Ensure the feature collection exists for the given filter key
            if (!featuresByFilter.ContainsKey(filterKey))
            {
                featuresByFilter[filterKey] = new FeatureCollection();
            }

            // Convert the symbol's latitude and longitude from DMS to decimal format
            var position = new Position(
                CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude)
            );

            // Create properties for the point feature if custom properties are enabled
            var properties = includeCustomProperties
                ? new Dictionary<string, object>
                {
                    { "E2G_MapObjectType", objectType.MapObjectType }, // Geomap MapObjectType (e.g., "VOR")
                    { "E2G_MapGroupId", objectType.MapGroupId },       // Geomap MapGroupId
                    { "E2G_SymbolId", symbol.SymbolId }                // Geomap SymbolId
                }
                : null; // No properties if custom properties are disabled

            // Add the symbol as a GeoJSON Point feature to the feature collection
            featuresByFilter[filterKey].Features.Add(new Feature(new GeoJSON.Net.Geometry.Point(position), properties));
        }

        // Adds a GeoMapText object as a point feature to the feature collection associated with the specified filter key
        private void AddTextPointToFeatures(Dictionary<string, FeatureCollection> featuresByFilter, string filterKey, GeoMapText textObject, GeoMapSymbol symbol, bool includeCustomProperties, GeoMapObjectType objectType)
        {
            // Ensure the feature collection exists for the given filter key
            if (!featuresByFilter.ContainsKey(filterKey))
            {
                featuresByFilter[filterKey] = new FeatureCollection();
            }

            // Convert the symbol's latitude and longitude from DMS to decimal format
            var position = new Position(
                CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude)
            );

            // Initialize a dictionary for properties
            var properties = new Dictionary<string, object>();

            // Add the text content based on its AppliedTextDisplaySetting
            if (textObject.AppliedTextDisplaySetting == true)
            {
                properties.Add("text", textObject.TextLines); // Add as "text" if display setting is true
            }
            else
            {
                properties.Add("E2G_text", textObject.TextLines); // Add as "E2G_text" for non-displaying text
            }

            // Add additional custom properties if requested
            if (includeCustomProperties)
            {
                properties.Add("E2G_MapObjectType", objectType.MapObjectType); // Geomap MapObjectType (e.g., "WAYPOINT")
                properties.Add("E2G_MapGroupId", objectType.MapGroupId);       // Geomap MapGroupId
                properties.Add("E2G_SymbolId", symbol.SymbolId);               // Geomap SymbolId
            }

            // Add the text object as a GeoJSON Point feature to the feature collection
            featuresByFilter[filterKey].Features.Add(new Feature(new GeoJSON.Net.Geometry.Point(position), properties));
        }

        // Writes the GeoJSON features grouped by filter to individual files in their respective directories
        private void WriteGeoJsonToFiles(Dictionary<string, FeatureCollection> featuresByFilter, string baseDirectory)
        {
            // Iterate through each filter key in the feature collection dictionary
            foreach (var filterKey in featuresByFilter.Keys)
            {
                // Construct the output directory path by appending the filter key to the base directory
                string filterDirectory = Path.Combine(baseDirectory, filterKey);

                // Remove suffixes like "_Text", "_Symbols", and "_Lines" from the filter key for directory naming
                filterDirectory = filterDirectory.Replace("_Text", "").Replace("_Symbols", "").Replace("_Lines", "");

                // Ensure the directory exists (create it if it doesn't)
                Directory.CreateDirectory(filterDirectory); // Ensure the directory exists

                // Construct the output file path using the filter directory and filter key
                string outputFilePath = Path.Combine(filterDirectory, $"{Path.GetFileName(filterKey)}.geojson");

                try
                {
                    // Serialize the FeatureCollection for the current filter key to a GeoJSON-formatted string
                    string geoJsonContent = JsonConvert.SerializeObject(featuresByFilter[filterKey], Formatting.None);

                    // Write the serialized GeoJSON content to the output file
                    File.WriteAllText(outputFilePath, geoJsonContent);
                }
                catch (Exception ex)
                {
                    // Log an error message if an exception occurs during the file write operation
                    Console.WriteLine($"An error occurred while writing GeoJSON to file: {ex.Message}");
                }
            }
        }
    }
}