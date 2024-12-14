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
        public void GenerateGeoJson(List<GeoMapRecord> geoMapRecords, string outputDirectory, bool includeCustomProperties)
        {
            foreach (var record in geoMapRecords)
            {
                string recordDirectory = Path.Combine(outputDirectory, $"{record.GeomapId}_{record.LabelLine1}-{record.LabelLine2}");
                Directory.CreateDirectory(recordDirectory);

                ProcessLines(record.ObjectTypes, recordDirectory, includeCustomProperties);
                ProcessSymbols(record.ObjectTypes, recordDirectory, includeCustomProperties);
                ProcessText(record.ObjectTypes, recordDirectory, includeCustomProperties);
            }
        }

        private void ProcessLines(List<GeoMapObjectType> objectTypes, string recordDirectory, bool includeCustomProperties)
        {
            var groupedLines = new Dictionary<string, List<List<IPosition>>>();
            var linePropertiesMap = new Dictionary<string, Dictionary<string, object>>();

            foreach (var objectType in objectTypes)
            {
                foreach (var line in objectType.Lines)
                {
                    string fileKey = ConstructLineFileKey(objectType, line);

                    if (!groupedLines.ContainsKey(fileKey))
                    {
                        groupedLines[fileKey] = new List<List<IPosition>>();
                        linePropertiesMap[fileKey] = CreateLineProperties(objectType, line, includeCustomProperties);
                    }

                    var start = new Position(
                        CoordinateConverter.ConvertDMSToDecimal(line.StartLatitude),
                        CoordinateConverter.ConvertDMSToDecimal(line.StartLongitude)
                    );
                    var end = new Position(
                        CoordinateConverter.ConvertDMSToDecimal(line.EndLatitude),
                        CoordinateConverter.ConvertDMSToDecimal(line.EndLongitude)
                    );

                    if (groupedLines[fileKey].Count > 0 &&
                        groupedLines[fileKey].Last().Last().Equals(start))
                    {
                        groupedLines[fileKey].Last().Add(end);
                    }
                    else
                    {
                        groupedLines[fileKey].Add(new List<IPosition> { start, end });
                    }
                }
            }

            foreach (var group in groupedLines)
            {
                string filePath = Path.Combine(recordDirectory, SanitizeFileName(group.Key));

                var multiLineString = new MultiLineString(group.Value.Select(line => new LineString(line)));
                var featureCollection = new FeatureCollection();
                featureCollection.Features.Add(new Feature(multiLineString, linePropertiesMap[group.Key]));

                WriteGeoJsonToFile(filePath, featureCollection);
            }
        }

        private void ProcessSymbols(List<GeoMapObjectType> objectTypes, string recordDirectory, bool includeCustomProperties)
        {
            var groupedSymbols = new Dictionary<string, List<Feature>>();

            foreach (var objectType in objectTypes)
            {
                foreach (var symbol in objectType.Symbols)
                {
                    string fileKey = ConstructSymbolFileKey(objectType, symbol);

                    if (!groupedSymbols.ContainsKey(fileKey))
                    {
                        groupedSymbols[fileKey] = new List<Feature>();
                    }

                    var position = new Position(
                        CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                        CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude)
                    );

                    var properties = includeCustomProperties
                        ? CreateSymbolProperties(objectType, symbol)
                        : null;

                    groupedSymbols[fileKey].Add(new Feature(new Point(position), properties));
                }
            }

            foreach (var group in groupedSymbols)
            {
                string filePath = Path.Combine(recordDirectory, SanitizeFileName(group.Key));
                var featureCollection = new FeatureCollection(group.Value);
                WriteGeoJsonToFile(filePath, featureCollection);
            }
        }

        private void ProcessText(List<GeoMapObjectType> objectTypes, string recordDirectory, bool includeCustomProperties)
        {
            var groupedText = new Dictionary<string, List<Feature>>();

            foreach (var objectType in objectTypes)
            {
                foreach (var symbol in objectType.Symbols)
                {
                    foreach (var textObject in symbol.TextObjects)
                    {
                        string fileKey = ConstructTextFileKey(objectType, symbol, textObject);

                        if (!groupedText.ContainsKey(fileKey))
                        {
                            groupedText[fileKey] = new List<Feature>();
                        }

                        var position = new Position(
                            CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                            CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude)
                        );

                        var properties = CreateTextProperties(objectType, symbol, textObject, includeCustomProperties);

                        groupedText[fileKey].Add(new Feature(new Point(position), properties));
                    }
                }
            }

            foreach (var group in groupedText)
            {
                string filePath = Path.Combine(recordDirectory, SanitizeFileName(group.Key));
                var featureCollection = new FeatureCollection(group.Value);
                WriteGeoJsonToFile(filePath, featureCollection);
            }
        }

        private string ConstructLineFileKey(GeoMapObjectType objectType, GeoMapLine line)
        {
            return $"Type {objectType.MapObjectType}_Group {objectType.MapGroupId}_Object {line.LineObjectId}_Filters {string.Join(" ", line.AppliedLineFilters.OrderBy(f => f).Select(f => f.ToString("D2")))}_BCG {line.AppliedLineBcgGroup}_Style {line.AppliedLineStyle}_Thick {line.AppliedLineThickness}_Lines.geojson";
        }

        private string ConstructSymbolFileKey(GeoMapObjectType objectType, GeoMapSymbol symbol)
        {
            return $"Type {objectType.MapObjectType}_Group {objectType.MapGroupId}_Filters {string.Join(" ", symbol.AppliedSymbolFilters.OrderBy(f => f).Select(f => f.ToString("D2")))}_BCG {symbol.AppliedSymbolBcgGroup}_Style {symbol.AppliedSymbolStyle}_Font {symbol.AppliedSymbolFontSize}_Symbols.geojson";
        }

        private string ConstructTextFileKey(GeoMapObjectType objectType, GeoMapSymbol symbol, GeoMapText textObject)
        {
            return $"Type {objectType.MapObjectType}_Group {objectType.MapGroupId}_Filters {string.Join(" ", textObject.AppliedTextFilters.OrderBy(f => f).Select(f => f.ToString("D2")))}_BCG {textObject.AppliedTextBcgGroup}_Font {textObject.AppliedTextFontSize}_Underline {(textObject.AppliedTextUnderline ?? false ? "T" : "F")}_X {textObject.AppliedTextXPixelOffset}_Y {textObject.AppliedTextYPixelOffset}_Text.geojson";
        }

        private Dictionary<string, object> CreateLineProperties(GeoMapObjectType objectType, GeoMapLine line, bool includeCustomProperties)
        {
            if (!includeCustomProperties) return null;

            return new Dictionary<string, object>
            {
                { "E2G_MapObjectType", objectType.MapObjectType },
                { "E2G_MapGroupId", objectType.MapGroupId },
                { "E2G_LineObjectId", line.LineObjectId }
            };
        }

        private Dictionary<string, object> CreateSymbolProperties(GeoMapObjectType objectType, GeoMapSymbol symbol)
        {
            return new Dictionary<string, object>
            {
                { "E2G_MapObjectType", objectType.MapObjectType },
                { "E2G_MapGroupId", objectType.MapGroupId },
                { "E2G_SymbolId", symbol.SymbolId }
            };
        }

        private Dictionary<string, object> CreateTextProperties(GeoMapObjectType objectType, GeoMapSymbol symbol, GeoMapText textObject, bool includeCustomProperties)
        {
            var properties = new Dictionary<string, object>
            {
                { "text", textObject.TextLines }
            };

            if (includeCustomProperties)
            {
                properties.Add("E2G_MapObjectType", objectType.MapObjectType);
                properties.Add("E2G_MapGroupId", objectType.MapGroupId);
                properties.Add("E2G_SymbolId", symbol.SymbolId);
            }

            return properties;
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

        private string SanitizeFileName(string input)
        {
            return string.Concat(input.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
