using ERAM_2_GEOJSON.Helpers;
using ERAM_2_GEOJSON.Models;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ERAM_2_GEOJSON.Helpers
{
    public class GeoJsonGenerator
    {
        public void GenerateGeoJson(List<GeoMapRecord> geoMapRecords, string outputDirectory, bool includeCustomProperties)
        {
            var featureCollections = new Dictionary<string, FeatureCollection>();

            foreach (var record in geoMapRecords)
            {
                string recordBasePath = Path.Combine(outputDirectory, $"{record.GeomapId}_{record.LabelLine1}-{record.LabelLine2}");

                foreach (var objectType in record.ObjectTypes)
                {
                    if (objectType.HasLine)
                    {
                        ProcessLines(objectType, recordBasePath, featureCollections, includeCustomProperties);
                    }

                    if (objectType.HasSymbol)
                    {
                        ProcessSymbols(objectType, recordBasePath, featureCollections, includeCustomProperties);
                    }

                    if (objectType.HasText)
                    {
                        ProcessText(objectType, recordBasePath, featureCollections, includeCustomProperties);
                    }
                }
            }

            WriteGeoJsonToFiles(featureCollections);
        }

        private void ProcessLines(GeoMapObjectType objectType, string recordBasePath, Dictionary<string, FeatureCollection> featureCollections, bool includeCustomProperties)
        {
            var groupedLines = objectType.Lines.GroupBy(line => string.Join("_", line.AppliedLineFilters.Select(f => f.ToString("D2"))));

            foreach (var group in groupedLines)
            {
                string filterKey = group.Key.Contains("_") ? $"Multi-Filter_{group.Key}" : $"Filter_{group.Key}";
                string directoryPath = Path.Combine(recordBasePath, filterKey);
                Directory.CreateDirectory(directoryPath);
                string filePath = Path.Combine(directoryPath, $"{filterKey}_Lines.geojson");

                if (!featureCollections.ContainsKey(filePath))
                {
                    featureCollections[filePath] = new FeatureCollection();
                }

                foreach (var line in group)
                {
                    var lineCoordinates = new List<IPosition>
                    {
                        new Position(CoordinateConverter.ConvertDMSToDecimal(line.StartLatitude), CoordinateConverter.ConvertDMSToDecimal(line.StartLongitude)),
                        new Position(CoordinateConverter.ConvertDMSToDecimal(line.EndLatitude), CoordinateConverter.ConvertDMSToDecimal(line.EndLongitude))
                    };

                    var lineString = new LineString(lineCoordinates);
                    var properties = includeCustomProperties
                        ? new Dictionary<string, object>
                        {
                            { "E2G_MapObjectType", objectType.MapObjectType },
                            { "E2G_MapGroupId", objectType.MapGroupId },
                            { "E2G_LineObjectId", line.LineObjectId }
                        }
                        : null;

                    featureCollections[filePath].Features.Add(new Feature(lineString, properties));
                }
            }
        }

        private void ProcessSymbols(GeoMapObjectType objectType, string recordBasePath, Dictionary<string, FeatureCollection> featureCollections, bool includeCustomProperties)
        {
            var groupedSymbols = objectType.Symbols.GroupBy(symbol => string.Join("_", symbol.AppliedSymbolFilters.Select(f => f.ToString("D2"))));

            foreach (var group in groupedSymbols)
            {
                string filterKey = group.Key.Contains("_") ? $"Multi-Filter_{group.Key}" : $"Filter_{group.Key}";
                string directoryPath = Path.Combine(recordBasePath, filterKey);
                Directory.CreateDirectory(directoryPath);
                string filePath = Path.Combine(directoryPath, $"{filterKey}_Symbols.geojson");

                if (!featureCollections.ContainsKey(filePath))
                {
                    featureCollections[filePath] = new FeatureCollection();
                }

                foreach (var symbol in group)
                {
                    var position = new Position(CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude), CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude));
                    var point = new Point(position);
                    var properties = includeCustomProperties
                        ? new Dictionary<string, object>
                        {
                            { "E2G_MapObjectType", objectType.MapObjectType },
                            { "E2G_MapGroupId", objectType.MapGroupId },
                            { "E2G_SymbolId", symbol.SymbolId }
                        }
                        : null;

                    featureCollections[filePath].Features.Add(new Feature(point, properties));
                }
            }
        }

        private void ProcessText(GeoMapObjectType objectType, string recordBasePath, Dictionary<string, FeatureCollection> featureCollections, bool includeCustomProperties)
        {
            foreach (var symbol in objectType.Symbols)
            {
                foreach (var textObject in symbol.TextObjects)
                {
                    string filterKey = string.Join("_", textObject.AppliedTextFilters.Select(f => f.ToString("D2")));
                    filterKey = filterKey.Contains("_") ? $"Multi-Filter_{filterKey}" : $"Filter_{filterKey}";
                    string directoryPath = Path.Combine(recordBasePath, filterKey);
                    Directory.CreateDirectory(directoryPath);
                    string filePath = Path.Combine(directoryPath, $"{filterKey}_Text.geojson");

                    if (!featureCollections.ContainsKey(filePath))
                    {
                        featureCollections[filePath] = new FeatureCollection();
                    }

                    var position = new Position(CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude), CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude));
                    var point = new Point(position);

                    var properties = new Dictionary<string, object>
                    {
                        { "E2G_MapObjectType", objectType.MapObjectType },
                        { "E2G_MapGroupId", objectType.MapGroupId },
                        { "E2G_SymbolId", symbol.SymbolId },
                        { "text", textObject.TextLines }
                    };

                    featureCollections[filePath].Features.Add(new Feature(point, properties));
                }
            }
        }

        private void WriteGeoJsonToFiles(Dictionary<string, FeatureCollection> featureCollections)
        {
            foreach (var kvp in featureCollections)
            {
                try
                {
                    string geoJsonContent = JsonConvert.SerializeObject(kvp.Value, Formatting.Indented);
                    File.WriteAllText(kvp.Key, geoJsonContent);
                    Console.WriteLine($"GeoJSON written to: {kvp.Key}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while writing GeoJSON to file: {ex.Message}");
                }
            }
        }
    }
}
