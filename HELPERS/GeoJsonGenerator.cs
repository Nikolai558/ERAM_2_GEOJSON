using ERAM_2_GEOJSON.Models;
using Newtonsoft.Json;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ERAM_2_GEOJSON.MODELS;

namespace ERAM_2_GEOJSON.Helpers
{
    public class GeoJsonGenerator
    {
        public void GenerateGeoJson(List<GeoMapRecord> geoMapRecords, string outputDirectory, bool includeCustomProperties)
        {
            foreach (var record in geoMapRecords)
            {
                string recordDirectory = Path.Combine(outputDirectory, $"{record.GeomapId}_{record.LabelLine1}-{record.LabelLine2}");
                EnsureDirectoryExists(outputDirectory, recordDirectory);

                var featuresByFilter = new Dictionary<string, FeatureCollection>();

                foreach (var objectType in record.ObjectTypes)
                {
                    ProcessLines(objectType, recordDirectory, featuresByFilter, includeCustomProperties);
                    ProcessSymbols(objectType, recordDirectory, featuresByFilter, includeCustomProperties);
                    ProcessText(objectType, recordDirectory, featuresByFilter, includeCustomProperties);
                }

                WriteGeoJsonToFiles(featuresByFilter, recordDirectory);
            }
        }

        private void ProcessLines(GeoMapObjectType objectType, string recordDirectory, Dictionary<string, FeatureCollection> featuresByFilter, bool includeCustomProperties)
        {
            var groupedLines = new Dictionary<string, List<List<IPosition>>>();

            foreach (var line in objectType.Lines)
            {
                string filterKey = ConstructFilterKey(line.AppliedLineFilters, "_Lines");
                string filterDirectory = EnsureDirectoryExists(recordDirectory, filterKey);

                if (!groupedLines.ContainsKey(filterKey))
                {
                    groupedLines[filterKey] = new List<List<IPosition>>();
                }

                var start = new Position(
                    CoordinateConverter.ConvertDMSToDecimal(line.StartLatitude),
                    CoordinateConverter.ConvertDMSToDecimal(line.StartLongitude)
                );
                var end = new Position(
                    CoordinateConverter.ConvertDMSToDecimal(line.EndLatitude),
                    CoordinateConverter.ConvertDMSToDecimal(line.EndLongitude)
                );

                if (groupedLines[filterKey].Count > 0 &&
                    groupedLines[filterKey].Last().Last().Equals(start))
                {
                    groupedLines[filterKey].Last().Add(end); // Extend the last LineString
                }
                else
                {
                    groupedLines[filterKey].Add(new List<IPosition> { start, end }); // Create a new LineString
                }
            }

            AddGroupedLinesToFeatures(groupedLines, featuresByFilter, objectType, includeCustomProperties);
        }

        private void ProcessSymbols(GeoMapObjectType objectType, string recordDirectory, Dictionary<string, FeatureCollection> featuresByFilter, bool includeCustomProperties)
        {
            foreach (var symbol in objectType.Symbols)
            {
                string filterKey = ConstructFilterKey(symbol.AppliedSymbolFilters, "_Symbols");
                string filterDirectory = EnsureDirectoryExists(recordDirectory, filterKey);

                AddPointToFeatures(featuresByFilter, filterKey, symbol, includeCustomProperties, objectType);
            }
        }

        private void ProcessText(GeoMapObjectType objectType, string recordDirectory, Dictionary<string, FeatureCollection> featuresByFilter, bool includeCustomProperties)
        {
            foreach (var symbol in objectType.Symbols)
            {
                foreach (var textObject in symbol.TextObjects)
                {
                    string filterKey = ConstructFilterKey(textObject.AppliedTextFilters, "_Text");
                    string filterDirectory = EnsureDirectoryExists(recordDirectory, filterKey);

                    AddTextPointToFeatures(featuresByFilter, filterKey, textObject, symbol, includeCustomProperties, objectType);
                }
            }
        }

        private string ConstructFilterKey(List<int> filters, string suffix)
        {
            string baseKey = filters.Count == 1
                ? $"Filter_{filters[0]:D2}"
                : $"Multi-Filter_{string.Join("_", filters.OrderBy(f => f).Select(f => f.ToString("D2")))}";

            return baseKey + suffix;
        }

        private string EnsureDirectoryExists(string baseDirectory, string filterKey)
        {
            string cleanedKey = filterKey.Replace("_Symbols", "").Replace("_Text", "").Replace("_Lines", "");
            string directoryPath = Path.Combine(baseDirectory, cleanedKey);
            Directory.CreateDirectory(directoryPath);
            return directoryPath;
        }

        private void AddGroupedLinesToFeatures(Dictionary<string, List<List<IPosition>>> groupedLines, Dictionary<string, FeatureCollection> featuresByFilter, GeoMapObjectType objectType, bool includeCustomProperties)
        {
            foreach (var group in groupedLines)
            {
                string filterKey = group.Key;

                if (!featuresByFilter.ContainsKey(filterKey))
                {
                    featuresByFilter[filterKey] = new FeatureCollection();
                }

                var multiLineString = new MultiLineString(group.Value.Select(line => new LineString(line)));

                var properties = includeCustomProperties
                    ? new Dictionary<string, object>
                    {
                        { "E2G_MapObjectType", objectType.MapObjectType },
                        { "E2G_MapGroupId", objectType.MapGroupId },
                        { "E2G_LineObjectId", objectType.Lines.FirstOrDefault()?.LineObjectId }
                    }
                    : null;

                featuresByFilter[filterKey].Features.Add(new Feature(multiLineString, properties));
            }
        }

        private void AddPointToFeatures(Dictionary<string, FeatureCollection> featuresByFilter, string filterKey, GeoMapSymbol symbol, bool includeCustomProperties, GeoMapObjectType objectType)
        {
            if (!featuresByFilter.ContainsKey(filterKey))
            {
                featuresByFilter[filterKey] = new FeatureCollection();
            }

            var position = new Position(
                CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude)
            );

            var properties = includeCustomProperties
                ? new Dictionary<string, object>
                {
                    { "E2G_MapObjectType", objectType.MapObjectType },
                    { "E2G_MapGroupId", objectType.MapGroupId },
                    { "E2G_SymbolId", symbol.SymbolId }
                }
                : null;

            featuresByFilter[filterKey].Features.Add(new Feature(new Point(position), properties));
        }

        private void AddTextPointToFeatures(Dictionary<string, FeatureCollection> featuresByFilter, string filterKey, GeoMapText textObject, GeoMapSymbol symbol, bool includeCustomProperties, GeoMapObjectType objectType)
        {
            if (!featuresByFilter.ContainsKey(filterKey))
            {
                featuresByFilter[filterKey] = new FeatureCollection();
            }

            var position = new Position(
                CoordinateConverter.ConvertDMSToDecimal(symbol.Latitude),
                CoordinateConverter.ConvertDMSToDecimal(symbol.Longitude)
            );

            var properties = new Dictionary<string, object> { { "text", textObject.TextLines } };

            if (includeCustomProperties)
            {
                properties.Add("E2G_MapObjectType", objectType.MapObjectType);
                properties.Add("E2G_MapGroupId", objectType.MapGroupId);
                properties.Add("E2G_SymbolId", symbol.SymbolId);
            }

            featuresByFilter[filterKey].Features.Add(new Feature(new Point(position), properties));
        }

        private void WriteGeoJsonToFiles(Dictionary<string, FeatureCollection> featuresByFilter, string baseDirectory)
        {
            foreach (var filterKey in featuresByFilter.Keys)
            {
                // Construct the full output path relative to the base directory
                string filterDirectory = Path.Combine(baseDirectory, filterKey);
                filterDirectory = filterDirectory.Replace("_Text", "").Replace("_Symbols", "").Replace("_Lines", "");
                Directory.CreateDirectory(filterDirectory); // Ensure the directory exists

                string outputFilePath = Path.Combine(filterDirectory, $"{Path.GetFileName(filterKey)}.geojson");

                try
                {
                    string geoJsonContent = JsonConvert.SerializeObject(featuresByFilter[filterKey], Formatting.Indented);
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
}
