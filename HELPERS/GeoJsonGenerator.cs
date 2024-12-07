using ERAM_2_GEOJSON.Helpers;
using ERAM_2_GEOJSON.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;

namespace ERAM_2_GEOJSON.Helpers
{
    public class GeoJsonGenerator
    {
        public void GenerateGeoJson(List<GeoMapRecord> geoMapRecords, string outputDirectory, bool includeCustomProperties)
        {
            // Dictionaries to hold feature collections grouped by filter group
            var linesByFilter = new Dictionary<string, FeatureCollection>();
            var symbolsByFilter = new Dictionary<string, FeatureCollection>();
            var textByFilter = new Dictionary<string, FeatureCollection>();

            foreach (var record in geoMapRecords)
            {
                string recordBasePath = Path.Combine(outputDirectory, $"{record.GeomapId}_{record.LabelLine1}-{record.LabelLine2}");

                foreach (var objectType in record.ObjectTypes)
                {
                    // Process Lines
                    if (objectType.HasLine)
                    {
                        ProcessLines(objectType, recordBasePath, linesByFilter, includeCustomProperties);
                    }

                    // Process Symbols
                    if (objectType.HasSymbol)
                    {
                        ProcessSymbols(objectType, recordBasePath, symbolsByFilter, includeCustomProperties);
                    }

                    // Process Text
                    if (objectType.HasText)
                    {
                        ProcessText(objectType, recordBasePath, textByFilter, includeCustomProperties);
                    }
                }
            }

            // Write the GeoJSON files
            WriteGeoJsonToFiles(linesByFilter, "Lines", outputDirectory);
            WriteGeoJsonToFiles(symbolsByFilter, "Symbols", outputDirectory);
            WriteGeoJsonToFiles(textByFilter, "Text", outputDirectory);
        }

        private void ProcessLines(GeoMapObjectType objectType, string recordBasePath, Dictionary<string, FeatureCollection> featureCollectionsByFilter, bool includeCustomProperties)
        {
            var groupedLines = new Dictionary<(string MapObjectType, string MapGroupId, string LineObjectId), List<List<IPosition>>>();

            foreach (var line in objectType.Lines)
            {
                if (line.AppliedLineFilters.Count == 0)
                {
                    Console.WriteLine($"Warning: Line with LineObjectId '{line.LineObjectId}' in MapObjectType '{objectType.MapObjectType}' has no filter group and will be skipped.");
                    continue;
                }

                var key = (objectType.MapObjectType, objectType.MapGroupId, line.LineObjectId);

                if (!groupedLines.ContainsKey(key))
                {
                    groupedLines[key] = new List<List<IPosition>>();
                }

                var currentLineString = new List<IPosition>
                {
                    new Position(
                        CoordinateConverter.ConvertDMSToDecimal(line.StartLatitude),
                        CoordinateConverter.ConvertDMSToDecimal(line.StartLongitude)
                    ),
                    new Position(
                        CoordinateConverter.ConvertDMSToDecimal(line.EndLatitude),
                        CoordinateConverter.ConvertDMSToDecimal(line.EndLongitude)
                    )
                };

                // Check if this line can continue the last LineString
                if (groupedLines[key].Count > 0)
                {
                    var lastLineString = groupedLines[key][groupedLines[key].Count - 1];
                    var lastPoint = lastLineString[^1];

                    if (lastPoint.Equals(currentLineString[0]))
                    {
                        lastLineString.Add(currentLineString[1]);
                    }
                    else
                    {
                        groupedLines[key].Add(currentLineString);
                    }
                }
                else
                {
                    groupedLines[key].Add(currentLineString);
                }
            }

            // Create features for each filter
            foreach (var group in groupedLines)
            {
                foreach (var filter in objectType.DefaultLineFilters)
                {
                    string filterKey = GetFilterKey(filter);
                    if (!featureCollectionsByFilter.ContainsKey(filterKey))
                    {
                        featureCollectionsByFilter[filterKey] = new FeatureCollection();
                    }

                    var lineStrings = group.Value.Select(line => new LineString(line));
                    var multiLineString = new MultiLineString(lineStrings);
                    var properties = new Dictionary<string, object>();

                    if (includeCustomProperties)
                    {
                        properties.Add("E2G_MapObjectType", group.Key.MapObjectType);
                        properties.Add("E2G_MapGroupId", group.Key.MapGroupId);
                        properties.Add("E2G_LineObjectId", group.Key.LineObjectId);
                    }

                    featureCollectionsByFilter[filterKey].Features.Add(new Feature(multiLineString, properties));
                }
            }
        }

        private void ProcessSymbols(GeoMapObjectType objectType, string recordBasePath, Dictionary<string, FeatureCollection> featureCollectionsByFilter, bool includeCustomProperties)
        {
            foreach (var symbol in objectType.Symbols)
            {
                if (symbol.AppliedSymbolFilters.Count == 0)
                {
                    Console.WriteLine($"Warning: Symbol with SymbolId '{symbol.SymbolId}' in MapObjectType '{objectType.MapObjectType}' has no filter group and will be skipped.");
                    continue;
                }

                foreach (var filter in symbol.AppliedSymbolFilters)
                {
                    string filterKey = GetFilterKey(filter);
                    if (!featureCollectionsByFilter.ContainsKey(filterKey))
                    {
                        featureCollectionsByFilter[filterKey] = new FeatureCollection();
                    }

                    var position = new Position(
                        CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                        CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude)
                    );

                    var point = new Point(position);
                    var properties = new Dictionary<string, object>();

                    if (includeCustomProperties)
                    {
                        properties.Add("E2G_MapObjectType", objectType.MapObjectType);
                        properties.Add("E2G_MapGroupId", objectType.MapGroupId);
                        properties.Add("E2G_SymbolId", symbol.SymbolId);
                    }

                    featureCollectionsByFilter[filterKey].Features.Add(new Feature(point, properties));
                }
            }
        }

        private void ProcessText(GeoMapObjectType objectType, string recordBasePath, Dictionary<string, FeatureCollection> featureCollectionsByFilter, bool includeCustomProperties)
        {
            foreach (var symbol in objectType.Symbols)
            {
                if (symbol.TextObjects?.Count > 0)
                {
                    foreach (var textObject in symbol.TextObjects)
                    {
                        if (textObject.AppliedTextFilters.Count == 0)
                        {
                            Console.WriteLine($"Warning: Text in SymbolId '{symbol.SymbolId}' for MapObjectType '{objectType.MapObjectType}' has no filter group and will be skipped.");
                            continue;
                        }

                        foreach (var filter in textObject.AppliedTextFilters)
                        {
                            string filterKey = GetFilterKey(filter);
                            if (!featureCollectionsByFilter.ContainsKey(filterKey))
                            {
                                featureCollectionsByFilter[filterKey] = new FeatureCollection();
                            }

                            var position = new Position(
                                CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                                CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude)
                            );

                            var point = new Point(position);
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

                            featureCollectionsByFilter[filterKey].Features.Add(new Feature(point, properties));
                        }
                    }
                }
            }
        }

        private void WriteGeoJsonToFiles(Dictionary<string, FeatureCollection> featureCollectionsByFilter, string featureType, string outputDirectory)
        {
            foreach (var filterKey in featureCollectionsByFilter.Keys)
            {
                string outputFilePath = Path.Combine(outputDirectory, filterKey, $"{filterKey}_{featureType}.geojson");

                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

                // Write the GeoJSON to file
                WriteGeoJsonToFile(featureCollectionsByFilter[filterKey], outputFilePath);
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

        private string GetFilterKey(int filter)
        {
            return filter > 99 ? $"Multi-Filter_{filter}" : $"Filter_{filter:D2}";
        }
    }
}
