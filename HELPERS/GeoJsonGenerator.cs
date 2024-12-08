using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using ERAM_2_GEOJSON.Helpers;
using ERAM_2_GEOJSON.Models;

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
                    ProcessLines(objectType, recordBasePath, includeCustomProperties);
                    ProcessSymbols(objectType, recordBasePath, includeCustomProperties);
                    ProcessText(objectType, recordBasePath, includeCustomProperties);
                }
            }
        }

        private void ProcessLines(GeoMapObjectType objectType, string recordBasePath, bool includeCustomProperties)
        {
            var groupedLines = new Dictionary<string, FeatureCollection>();

            foreach (var line in objectType.Lines)
            {
                var filters = line.AppliedLineFilters;
                string filterKey = filters.Count > 1
                    ? $"Multi-Filter_{string.Join("_", filters.OrderBy(f => f))}"
                    : $"Filter_{filters.First():D2}";

                string filterDirectory = Path.Combine(recordBasePath, filterKey);
                Directory.CreateDirectory(filterDirectory);

                if (!groupedLines.ContainsKey(filterKey))
                {
                    groupedLines[filterKey] = new FeatureCollection();
                }

                var lineString = new LineString(new List<IPosition>
                {
                    new Position(
                        CoordinateConverter.ConvertDMSToDecimal(line.StartLatitude),
                        CoordinateConverter.ConvertDMSToDecimal(line.StartLongitude)
                    ),
                    new Position(
                        CoordinateConverter.ConvertDMSToDecimal(line.EndLatitude),
                        CoordinateConverter.ConvertDMSToDecimal(line.EndLongitude)
                    )
                });

                var properties = includeCustomProperties
                    ? new Dictionary<string, object>
                    {
                        { "E2G_MapObjectType", objectType.MapObjectType },
                        { "E2G_MapGroupId", objectType.MapGroupId },
                        { "E2G_LineObjectId", line.LineObjectId }
                    }
                    : null;

                groupedLines[filterKey].Features.Add(new Feature(lineString, properties));
            }

            foreach (var filterKey in groupedLines.Keys)
            {
                string filePath = Path.Combine(recordBasePath, filterKey, $"{filterKey}_Lines.geojson");
                WriteGeoJsonToFile(groupedLines[filterKey], filePath);
            }
        }

        private void ProcessSymbols(GeoMapObjectType objectType, string recordBasePath, bool includeCustomProperties)
        {
            var groupedSymbols = new Dictionary<string, FeatureCollection>();

            foreach (var symbol in objectType.Symbols)
            {
                var filters = symbol.AppliedSymbolFilters;
                string filterKey = filters.Count > 1
                    ? $"Multi-Filter_{string.Join("_", filters.OrderBy(f => f))}"
                    : $"Filter_{filters.First():D2}";

                string filterDirectory = Path.Combine(recordBasePath, filterKey);
                Directory.CreateDirectory(filterDirectory);

                if (!groupedSymbols.ContainsKey(filterKey))
                {
                    groupedSymbols[filterKey] = new FeatureCollection();
                }

                var point = new Point(new Position(
                    CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                    CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude)
                ));

                var properties = includeCustomProperties
                    ? new Dictionary<string, object>
                    {
                        { "E2G_MapObjectType", objectType.MapObjectType },
                        { "E2G_MapGroupId", objectType.MapGroupId },
                        { "E2G_SymbolId", symbol.SymbolId }
                    }
                    : null;

                groupedSymbols[filterKey].Features.Add(new Feature(point, properties));
            }

            foreach (var filterKey in groupedSymbols.Keys)
            {
                string filePath = Path.Combine(recordBasePath, filterKey, $"{filterKey}_Symbols.geojson");
                WriteGeoJsonToFile(groupedSymbols[filterKey], filePath);
            }
        }

        private void ProcessText(GeoMapObjectType objectType, string recordBasePath, bool includeCustomProperties)
        {
            var groupedText = new Dictionary<string, FeatureCollection>();

            foreach (var symbol in objectType.Symbols)
            {
                foreach (var textObject in symbol.TextObjects)
                {
                    var filters = textObject.AppliedTextFilters;
                    string filterKey = filters.Count > 1
                        ? $"Multi-Filter_{string.Join("_", filters.OrderBy(f => f))}"
                        : $"Filter_{filters.First():D2}";

                    string filterDirectory = Path.Combine(recordBasePath, filterKey);
                    Directory.CreateDirectory(filterDirectory);

                    if (!groupedText.ContainsKey(filterKey))
                    {
                        groupedText[filterKey] = new FeatureCollection();
                    }

                    var point = new Point(new Position(
                        CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                        CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude)
                    ));

                    var properties = new Dictionary<string, object>
                    {
                        { "text", string.Join(" ", textObject.TextLines) }
                    };

                    if (includeCustomProperties)
                    {
                        properties.Add("E2G_MapObjectType", objectType.MapObjectType);
                        properties.Add("E2G_MapGroupId", objectType.MapGroupId);
                        properties.Add("E2G_SymbolId", symbol.SymbolId);
                    }

                    groupedText[filterKey].Features.Add(new Feature(point, properties));
                }
            }

            foreach (var filterKey in groupedText.Keys)
            {
                string filePath = Path.Combine(recordBasePath, filterKey, $"{filterKey}_Text.geojson");
                WriteGeoJsonToFile(groupedText[filterKey], filePath);
            }
        }

        private void WriteGeoJsonToFile(FeatureCollection featureCollection, string filePath)
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
