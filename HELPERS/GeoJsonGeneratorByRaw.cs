using ERAM_2_GEOJSON.Models;
using Newtonsoft.Json;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System.Collections.Generic;
using System.IO;

namespace ERAM_2_GEOJSON.Helpers
{
    public class GeoJsonGeneratorByRaw
    {
        /// <summary>
        /// Generates GeoJSON files combining all data from GeoMaps.xml without grouping or optimized line handling.
        /// This is a direct/raw conversion of the data and must be selected as the output format by the user.
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

                // Construct the file path for the output GeoJSON file
                string filePath = Path.Combine(outputDirectory, $"{sanitizedGeomapId}_{sanitizedLabelLine1}-{sanitizedLabelLine2}.geojson");

                // Initialize a FeatureCollection to store all features for this record
                var featureCollection = new FeatureCollection();

                // Process each object type in the record
                foreach (var objectType in record.ObjectTypes)
                {
                    // Process lines, symbols, and text objects and add them to the feature collection
                    ProcessLines(objectType, featureCollection, includeCustomProperties);
                    ProcessSymbols(objectType, featureCollection, includeCustomProperties);
                    ProcessText(objectType, featureCollection, includeCustomProperties);
                }

                // Write the complete feature collection to the GeoJSON file
                WriteGeoJsonToFile(filePath, featureCollection);
            }
        }

        // Processes GeoMapLine objects, converting each into a GeoJSON LineString and adding it to the feature collection
        private void ProcessLines(GeoMapObjectType objectType, FeatureCollection featureCollection, bool includeCustomProperties)
        {
            // Iterate through each line in the given object type
            foreach (var line in objectType.Lines)
            {
                var coordinates = new List<IPosition>
                {
                    new Position(
                        CoordinateConverter.ConvertDMSToDecimal(line.StartLatitude),
                        CoordinateConverter.ConvertDMSToDecimal(line.StartLongitude)),
                    new Position(
                        CoordinateConverter.ConvertDMSToDecimal(line.EndLatitude),
                        CoordinateConverter.ConvertDMSToDecimal(line.EndLongitude))
                };

                // Initialize properties dictionary for the line feature
                var properties = new Dictionary<string, object>
                {
                    { "style", line.AppliedLineStyle },         // Style of the line
                    { "thickness", line.AppliedLineThickness }, // Thickness of the line 
                    { "bcg", line.AppliedLineBcgGroup },        // Brightness Control Group of the line
                    { "filters", line.AppliedLineFilters }      // Filters applied to the line
                };

                // Add custom properties if enabled
                if (includeCustomProperties)
                {
                    properties.Add("E2G_MapObjectType", objectType.MapObjectType); // Type of the map object (e.g., "AIRWAY")
                    properties.Add("E2G_MapGroupId", objectType.MapGroupId);       // Group ID associated with the object
                    properties.Add("E2G_LineObjectId", line.LineObjectId);         // Unique ID for the line object
                }

                // Create a GeoJSON LineString feature using the coordinates and properties
                var lineString = new LineString(coordinates);

                // Add the feature to the collection
                featureCollection.Features.Add(new Feature(lineString, properties));
            }
        }

        // Processes GeoMapSymbol objects, converting each into a GeoJSON Point feature and adding it to the feature collection
        private void ProcessSymbols(GeoMapObjectType objectType, FeatureCollection featureCollection, bool includeCustomProperties)
        {
            // Iterate through each symbol in the given object type
            foreach (var symbol in objectType.Symbols)
            {
                // Convert the symbol's latitude and longitude from DMS to decimal format
                var position = new Position(
                    CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                    CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude));

                // Initialize properties dictionary for the symbol feature
                var properties = new Dictionary<string, object>
                {
                    { "bcg", symbol.AppliedSymbolBcgGroup },    // Brightness Control Group associated with the symbol
                    { "filters", symbol.AppliedSymbolFilters }, // Filters applied to the symbol
                    { "style", symbol.AppliedSymbolStyle },     // Style of the symbol
                    { "size", symbol.AppliedSymbolFontSize }    // Font size for the symbol
                };

                // Add custom properties if enabled
                if (includeCustomProperties)
                {
                    properties.Add("E2G_MapObjectType", objectType.MapObjectType); // Geomap MapObjectType (e.g., "VOR")
                    properties.Add("E2G_MapGroupId", objectType.MapGroupId);       // Geomap MapGroupId
                    properties.Add("E2G_SymbolId", symbol.SymbolId);               // Geomap SymbolId
                }

                // Create a GeoJSON Point feature using the position and properties
                var point = new GeoJSON.Net.Geometry.Point(position);

                // Add the feature to the collection
                featureCollection.Features.Add(new Feature(point, properties));
            }
        }

        // Processes GeoMapText objects, converting each into a GeoJSON Point feature and adding it to the feature collection
        private void ProcessText(GeoMapObjectType objectType, FeatureCollection featureCollection, bool includeCustomProperties)
        {
            // Iterate through all symbols in the given object type
            foreach (var symbol in objectType.Symbols)
            {
                // Iterate through all text objects associated with the current symbol
                foreach (var textObject in symbol.TextObjects)
                {
                    // Convert the symbol's latitude and longitude from DMS to decimal format
                    var position = new Position(
                        CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                        CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude));

                    // Initialize properties dictionary for the text feature
                    var properties = new Dictionary<string, object>
                    {
                        { "bcg", textObject.AppliedTextBcgGroup },                 // Brightness Control Group for the text
                        { "filters", textObject.AppliedTextFilters },              // Filters applied to the text
                        { "size", textObject.AppliedTextFontSize },                // Font size of the text
                        { "underline", textObject.AppliedTextUnderline ?? false }, // Whether the text is underlined
                        { "xOffset", textObject.AppliedTextXPixelOffset },         // Horizontal pixel offset for the text
                        { "yOffset", textObject.AppliedTextYPixelOffset }          // Vertical pixel offset for the text
                    };

                    // Add the text content to the properties
                    // Use "text" for text meant to be displayed and "E2G_text" for non-displaying text
                    if (textObject.AppliedTextDisplaySetting == true)
                    {
                        properties.Add("text", textObject.TextLines);
                    }
                    else
                    {
                        properties.Add("E2G_text", textObject.TextLines);
                    }

                    // Add additional custom properties if requested
                    if (includeCustomProperties)
                    {
                        properties.Add("E2G_MapObjectType", objectType.MapObjectType); // Geomap MapObjectType (e.g., "WAYPOINT")
                        properties.Add("E2G_MapGroupId", objectType.MapGroupId);       // Geomap MapGroupId
                        properties.Add("E2G_SymbolId", symbol.SymbolId);               // Geomap SymbolId
                    }

                    // Create a GeoJSON Point feature using the position and properties
                    var point = new GeoJSON.Net.Geometry.Point(position);

                    // Add the feature to the collection
                    featureCollection.Features.Add(new Feature(point, properties));
                }
            }
        }

        // Writes a GeoJSON FeatureCollection to a file
        private void WriteGeoJsonToFile(string filePath, FeatureCollection featureCollection)
        {
            try
            {
                // Serialize the FeatureCollection object to a GeoJSON-formatted string
                string geoJsonContent = JsonConvert.SerializeObject(featureCollection, Formatting.None);

                // Write the serialized GeoJSON content to the specified file path
                File.WriteAllText(filePath, geoJsonContent);
            }
            catch (Exception ex)
            {
                // Log an error message if an exception occurs during the file write operation
                Console.WriteLine($"An error occurred while writing GeoJSON to file: {ex.Message}");
            }
        }
    }
}