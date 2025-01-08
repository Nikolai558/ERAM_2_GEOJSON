using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERAM_2_GEOJSON.Helpers
{
    public static class AddIsDefaults
    {
        /// <summary>
        /// Parses the geojsons that were output and adds the isDefault features to the beginning of the file. The values are gathered from the file name of the output geojson.
        /// </summary>
        public static void AddToGeoJsonFiles(string outputDirectory)
        {
            // All sub-directories and their .geojson files.
            var geoJsonFiles = Directory.GetFiles(outputDirectory, "*.geojson", SearchOption.AllDirectories);

            foreach (var filePath in geoJsonFiles)
            {
                // Get full file path of the file
                string fileName = Path.GetFileName(filePath);

                // Parse file name segments to extract properties
                var properties = ParseFileNameSegments(fileName);

                // Determine the file type (Lines, Symbols, Text)
                string type = fileName.Contains("_Lines") ? "Line" :
                              fileName.Contains("_Symbols") ? "Symbol" : "Text";

                // Create isDefaults feature
                var isDefaultsFeature = CreateIsDefaultsFeature(type, properties);

                // Load existing FeatureCollection
                var featureCollection = JsonConvert.DeserializeObject<FeatureCollection>(File.ReadAllText(filePath));

                // Prepend the isDefaults feature
                featureCollection.Features.Insert(0, isDefaultsFeature);

                // Save the updated FeatureCollection
                File.WriteAllText(filePath, JsonConvert.SerializeObject(featureCollection, Formatting.None));
            }
        }

        private static Dictionary<string, object> ParseFileNameSegments(string fileName)
        {
            var properties = new Dictionary<string, object>();

            // Split file name at underscores to separate the different data-types into different strings.
            var segments = fileName.Split('_');

            // Parse each string from the split.
            foreach (var segment in segments)
            {
                if (segment.StartsWith("BCG ")) // BCG DATA.
                {
                    var bcgValue = segment.Substring("BCG ".Length);

                    // Remove leading zero if more than 1 character.
                    properties["bcg"] = bcgValue.StartsWith("0") && bcgValue.Length > 1
                        ? int.Parse(bcgValue.Substring(1))
                        : int.Parse(bcgValue);
                }
                else if (segment.StartsWith("Filters ")) // FILTERS DATA
                {
                    // Remove leading zero if more than 1 character.
                    var filters = segment.Substring("Filters ".Length)
                        .Split(' ')
                        .Select(f => f.StartsWith("0") && f.Length > 1 ? int.Parse(f.Substring(1)) : int.Parse(f))
                        .ToList();
                    properties["filters"] = filters;
                }
                else if (segment.StartsWith("Style ")) // STYLE DATA
                {
                    properties["style"] = segment.Substring("Style ".Length);
                }
                else if (segment.StartsWith("Thick ")) // THICKNESS DATA
                {
                    properties["thickness"] = int.Parse(segment.Substring("Thick ".Length));
                }
                else if (segment.StartsWith("Font ")) // FONT/SIZE DATA
                {
                    properties["size"] = int.Parse(segment.Substring("Font ".Length));
                }
                else if (segment.StartsWith("Underline ")) // UNDERLINE T/F DATA
                {
                    properties["underline"] = segment.Substring("Underline ".Length) == "T";
                }
                else if (segment.StartsWith("X ")) // xOFFSET DATA
                {
                    properties["xOffset"] = int.Parse(segment.Substring("X ".Length));
                }
                else if (segment.StartsWith("Y ")) // yOFFSET DATA
                {
                    properties["yOffset"] = int.Parse(segment.Substring("Y ".Length));
                }
            }

            return properties;
        }

        private static Feature CreateIsDefaultsFeature(string type, Dictionary<string, object> properties)
        {
            // Always include the type-specific property: "isLineDefaults" "isSymbolDefaults" "isTextDefaults"
            properties[$"is{type}Defaults"] = true;

            // Add default geometry
            var geometry = new GeoJSON.Net.Geometry.Point(new Position(90.0, 180.0)); // Coordinates chosen for simplicity, standardization, and when viewing in a geojson viewer, this data will be placed outside the data-display area.

            // Opaque is not an attribute found in the geomaps.xml but is an option for CRC so for `_Text.geojson`, add "opaque": false
            if (type == "Text")
            {
                properties["opaque"] = false;
            }

            return new Feature(geometry, properties);
        }
    }
}
