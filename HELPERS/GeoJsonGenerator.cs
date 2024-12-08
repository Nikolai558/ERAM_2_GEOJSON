using ERAM_2_GEOJSON.Helpers;
using ERAM_2_GEOJSON.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;

namespace ERAM_2_GEOJSON.Helpers
{
    public class GeoJsonGenerator
    {
        public void GenerateGeoJson(List<GeoMapRecord> geoMapRecords, string outputDirectory, bool includeCustomProperties)
        {
            foreach (var record in geoMapRecords)
            {
                string recordBasePath = Path.Combine(outputDirectory, $"{record.GeomapId}_{record.LabelLine1}-{record.LabelLine2}");

                foreach (var objectType in record.ObjectTypes)
                {
                    if (objectType.HasLine)
                    {
                        ProcessLines(objectType, recordBasePath, includeCustomProperties);
                    }

                    if (objectType.HasSymbol)
                    {
                        ProcessSymbols(objectType, recordBasePath, includeCustomProperties);
                    }

                    if (objectType.HasText)
                    {
                        ProcessText(objectType, recordBasePath, includeCustomProperties);
                    }
                }
            }
        }

        private void ProcessLines(GeoMapObjectType objectType, string recordBasePath, bool includeCustomProperties)
        {
            var groupedLines = objectType.Lines.GroupBy(line => line.AppliedLineFilters);

            foreach (var group in groupedLines)
            {
                string filterKey = group.Key.Count > 1
                    ? $"Multi-Filter_{string.Join("_", group.Key.Select(f => f.ToString("D2")))}"
                    : $"Filter_{group.Key.First().ToString("D2")}";

                string filterDirectory = Path.Combine(recordBasePath, filterKey);
                Directory.CreateDirectory(filterDirectory);

                var featureCollection = new FeatureCollection();

                foreach (var line in group)
                {
                    var lineCoordinates = new List<IPosition>
                    {
                        new Position(CoordinateConverter.ConvertDMSToDecimal(line.StartLatitude), CoordinateConverter.ConvertDMSToDecimal(line.StartLongitude)),
                        new Position(CoordinateConverter.ConvertDMSToDecimal(line.EndLatitude), CoordinateConverter.ConvertDMSToDecimal(line.EndLongitude))
                    };

                    var properties = includeCustomProperties
                        ? new Dictionary<string, object>
                        {
                            { "E2G_MapObjectType", objectType.MapObjectType },
                            { "E2G_MapGroupId", objectType.MapGroupId },
                            { "E2G_LineObjectId", line.LineObjectId }
                        }
                        : null;

                    var lineString = new LineString(lineCoordinates);
                    featureCollection.Features.Add(new Feature(lineString, properties));
                }

                string outputFilePath = Path.Combine(filterDirectory, $"{filterKey}_Lines.geojson");
                WriteGeoJsonToFile(featureCollection, outputFilePath);
            }
        }

        private void ProcessSymbols(GeoMapObjectType objectType, string recordBasePath, bool includeCustomProperties)
        {
            var groupedSymbols = objectType.Symbols.GroupBy(symbol => symbol.AppliedSymbolFilters);

            foreach (var group in groupedSymbols)
            {
                string filterKey = group.Key.Count > 1
                    ? $"Multi-Filter_{string.Join("_", group.Key.Select(f => f.ToString("D2")))}"
                    : $"Filter_{group.Key.First().ToString("D2")}";

                string filterDirectory = Path.Combine(recordBasePath, filterKey);
                Directory.CreateDirectory(filterDirectory);

                var featureCollection = new FeatureCollection();

                foreach (var symbol in group)
                {
                    var position = new Position(CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude), CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude));

                    var properties = includeCustomProperties
                        ? new Dictionary<string, object>
                        {
                            { "E2G_MapObjectType", objectType.MapObjectType },
                            { "E2G_MapGroupId", objectType.MapGroupId },
                            { "E2G_SymbolId", symbol.SymbolId }
                        }
                        : new Dictionary<string, object>
                        {
                            { "text", symbol.TextObjects.SelectMany(t => t.TextLines).ToList() }
                        };

                    var point = new Point(position);
                    featureCollection.Features.Add(new Feature(point, properties));
                }

                string outputFilePath = Path.Combine(filterDirectory, $"{filterKey}_Symbols.geojson");
                WriteGeoJsonToFile(featureCollection, outputFilePath);
            }
        }

        private void ProcessText(GeoMapObjectType objectType, string recordBasePath, bool includeCustomProperties)
        {
            foreach (var symbol in objectType.Symbols)
            {
                foreach (var textObject in symbol.TextObjects)
                {
                    var appliedFilters = textObject.AppliedTextFilters.Count > 0
                        ? textObject.AppliedTextFilters
                        : objectType.DefaultTextFilters;

                    string filterKey = appliedFilters.Count > 1
                        ? $"Multi-Filter_{string.Join("_", appliedFilters.Select(f => f.ToString("D2")))}"
                        : $"Filter_{appliedFilters.First().ToString("D2")}";

                    string filterDirectory = Path.Combine(recordBasePath, filterKey);
                    Directory.CreateDirectory(filterDirectory);

                    var featureCollection = new FeatureCollection();

                    var position = new Position(CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude), CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude));

                    var properties = new Dictionary<string, object>
                    {
                        { "E2G_MapObjectType", objectType.MapObjectType },
                        { "E2G_MapGroupId", objectType.MapGroupId },
                        { "E2G_SymbolId", symbol.SymbolId },
                        { "text", textObject.TextLines }
                    };

                    var point = new Point(position);
                    featureCollection.Features.Add(new Feature(point, properties));

                    string outputFilePath = Path.Combine(filterDirectory, $"{filterKey}_Text.geojson");
                    WriteGeoJsonToFile(featureCollection, outputFilePath);
                }
            }
        }

        private void WriteGeoJsonToFile(FeatureCollection featureCollection, string outputFilePath)
        {
            try
            {
                string geoJsonContent = JsonConvert.SerializeObject(featureCollection, Formatting.Indented);
                File.WriteAllText(outputFilePath, geoJsonContent);
                Console.WriteLine($"GeoJSON written to: {outputFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing GeoJSON to file: {ex.Message}");
            }
        }
    }
}
