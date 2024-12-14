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
        public void GenerateGeoJson(List<GeoMapRecord> geoMapRecords, string outputDirectory, bool includeCustomProperties)
        {
            foreach (var record in geoMapRecords)
            {
                string recordDirectory = Path.Combine(outputDirectory, $"{record.GeomapId}_{record.LabelLine1}-{record.LabelLine2}");
                Directory.CreateDirectory(recordDirectory);

                string filePath = Path.Combine(recordDirectory, $"{record.GeomapId}_Record.geojson");
                var featureCollection = new FeatureCollection();

                foreach (var objectType in record.ObjectTypes)
                {
                    ProcessLines(objectType, featureCollection, includeCustomProperties);
                    ProcessSymbols(objectType, featureCollection, includeCustomProperties);
                    ProcessText(objectType, featureCollection, includeCustomProperties);
                }

                WriteGeoJsonToFile(filePath, featureCollection);
            }
        }

        private void ProcessLines(GeoMapObjectType objectType, FeatureCollection featureCollection, bool includeCustomProperties)
        {
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

                var properties = new Dictionary<string, object>
                {
                    { "style", line.AppliedLineStyle },
                    { "thickness", line.AppliedLineThickness },
                    { "bcg", line.AppliedLineBcgGroup },
                    { "filters", line.AppliedLineFilters }
                };

                if (includeCustomProperties)
                {
                    properties.Add("E2G_MapObjectType", objectType.MapObjectType);
                    properties.Add("E2G_MapGroupId", objectType.MapGroupId);
                    properties.Add("E2G_LineObjectId", line.LineObjectId);
                }

                var lineString = new LineString(coordinates);
                featureCollection.Features.Add(new Feature(lineString, properties));
            }
        }

        private void ProcessSymbols(GeoMapObjectType objectType, FeatureCollection featureCollection, bool includeCustomProperties)
        {
            foreach (var symbol in objectType.Symbols)
            {
                var position = new Position(
                    CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                    CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude));

                var properties = new Dictionary<string, object>
                {
                    { "bcg", symbol.AppliedSymbolBcgGroup },
                    { "filters", symbol.AppliedSymbolFilters },
                    { "style", symbol.AppliedSymbolStyle },
                    { "size", symbol.AppliedSymbolFontSize }
                };

                if (includeCustomProperties)
                {
                    properties.Add("E2G_MapObjectType", objectType.MapObjectType);
                    properties.Add("E2G_MapGroupId", objectType.MapGroupId);
                    properties.Add("E2G_SymbolId", symbol.SymbolId);
                }

                var point = new Point(position);
                featureCollection.Features.Add(new Feature(point, properties));
            }
        }

        private void ProcessText(GeoMapObjectType objectType, FeatureCollection featureCollection, bool includeCustomProperties)
        {
            foreach (var symbol in objectType.Symbols)
            {
                foreach (var textObject in symbol.TextObjects)
                {
                    var position = new Position(
                        CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                        CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude));

                    var properties = new Dictionary<string, object>
                    {
                        { "bcg", textObject.AppliedTextBcgGroup },
                        { "filters", textObject.AppliedTextFilters },
                        { "size", textObject.AppliedTextFontSize },
                        { "underline", textObject.AppliedTextUnderline ?? false },
                        { "xOffset", textObject.AppliedTextXPixelOffset },
                        { "yOffset", textObject.AppliedTextYPixelOffset }
                    };

                    // Add "text" or "E2G_text" based on AppliedTextDisplaySetting
                    if (textObject.AppliedTextDisplaySetting == true)
                    {
                        properties.Add("text", textObject.TextLines);
                    }
                    else
                    {
                        properties.Add("E2G_text", textObject.TextLines);
                    }

                    // Include custom properties if required
                    if (includeCustomProperties)
                    {
                        properties.Add("E2G_MapObjectType", objectType.MapObjectType);
                        properties.Add("E2G_MapGroupId", objectType.MapGroupId);
                        properties.Add("E2G_SymbolId", symbol.SymbolId);
                    }

                    var point = new Point(position);
                    featureCollection.Features.Add(new Feature(point, properties));
                }
            }
        }

        private void WriteGeoJsonToFile(string filePath, FeatureCollection featureCollection)
        {
            try
            {
                string geoJsonContent = JsonConvert.SerializeObject(featureCollection, Formatting.Indented);
                File.WriteAllText(filePath, geoJsonContent);
                Console.WriteLine($"GeoJSON written to: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing GeoJSON to file: {ex.Message}");
            }
        }
    }
}