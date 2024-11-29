using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ERAM_2_GEOJSON.Models;
using ERAM_2_GEOJSON.Helpers;

namespace ERAM_2_GEOJSON.GeoJson
{
    public class GeoJsonGenerator
    {
        public void GenerateGeoJson(List<GeoMapRecord> geoMapRecords, string outputDirectory, bool customProperties)
        {
            foreach (var record in geoMapRecords)
            {
                foreach (var objectType in record.ObjectTypes)
                {
                    // Generate GeoJSON for lines
                    GenerateLinesGeoJson(objectType.Lines, objectType, record, outputDirectory, customProperties);
                }
            }
        }

        private void GenerateLinesGeoJson(List<GeoMapLine> lines, GeoMapObjectType objectType, GeoMapRecord record, string outputDirectory, bool customProperties)
        {
            // Create the proper directory structure based on GeomapId, LabelLine1, and LabelLine2
            string recordDirectory = Path.Combine(outputDirectory, $"{record.GeomapId}-{record.LabelLine1}_{record.LabelLine2}");

            if (!Directory.Exists(recordDirectory))
            {
                Directory.CreateDirectory(recordDirectory);
            }

            // Collect filter group IDs and iterate over them to create GeoJSON
            var filterGroups = lines
                .SelectMany(line => line.OverridingFilterGroups ?? line.FilterGroups)
                .Distinct()
                .ToList();

            // Dictionary to accumulate features for each filter group
            var filterGroupFeatures = new Dictionary<string, FeatureCollection>();

            foreach (var filterGroupId in filterGroups)
            {
                if (!filterGroupFeatures.ContainsKey(filterGroupId))
                {
                    filterGroupFeatures[filterGroupId] = new FeatureCollection();
                }

                var featureCollection = filterGroupFeatures[filterGroupId];

                var groupedLines = lines.Where(line => (line.OverridingFilterGroups ?? line.FilterGroups).Contains(filterGroupId));

                // Initialize properties for the GeoJSON feature
                var featureProperties = new Dictionary<string, object>
    {
        { "E2G_MapObjectType", objectType.MapObjectType },
        { "E2G_MapGroupId", objectType.MapGroupId },
        { "E2G_LineObjectId", groupedLines.First().LineObjectId }
    };

                if (customProperties)
                {
                    featureProperties.Add("custom", "true");
                }

                List<LineString> lineStrings = new List<LineString>();
                List<IPosition> currentCoordinates = new List<IPosition>();

                GeoMapLine? previousLine = null;

                foreach (var line in groupedLines)
                {
                    var startPosition = new Position(
                        CoordinateConverter.ConvertDMSToDecimal(line.StartLatitude),
                        CoordinateConverter.ConvertDMSToDecimal(line.StartLongitude)
                    );

                    var endPosition = new Position(
                        CoordinateConverter.ConvertDMSToDecimal(line.EndLatitude),
                        CoordinateConverter.ConvertDMSToDecimal(line.EndLongitude)
                    );

                    if (previousLine != null && startPosition.Equals(new Position(
                        CoordinateConverter.ConvertDMSToDecimal(previousLine.EndLatitude),
                        CoordinateConverter.ConvertDMSToDecimal(previousLine.EndLongitude))))
                    {
                        // If the current line starts where the previous one ends, continue the current LineString
                        currentCoordinates.Add(endPosition);
                    }
                    else
                    {
                        // Otherwise, finalize the previous LineString and start a new one
                        if (currentCoordinates.Count > 1)
                        {
                            lineStrings.Add(new LineString(currentCoordinates));
                        }

                        currentCoordinates = new List<IPosition> { startPosition, endPosition };
                    }

                    previousLine = line;
                }

                // Finalize the last LineString
                if (currentCoordinates.Count > 1)
                {
                    lineStrings.Add(new LineString(currentCoordinates));
                }

                if (lineStrings.Count > 0)
                {
                    MultiLineString multiLineString = new MultiLineString(lineStrings);
                    Feature feature = new Feature(multiLineString, featureProperties);
                    featureCollection.Features.Add(feature);
                }
            }

            // Write all feature collections to their respective filter directories
            foreach (var filterGroupId in filterGroupFeatures.Keys)
            {
                var featureCollection = filterGroupFeatures[filterGroupId];

                if (featureCollection.Features.Any())
                {
                    // Create a directory for each filter group
                    string filterDirectory = Path.Combine(recordDirectory, $"Filter_{filterGroupId:D2}");
                    if (!Directory.Exists(filterDirectory))
                    {
                        Directory.CreateDirectory(filterDirectory);
                    }

                    // Write the GeoJSON file
                    string outputFilePath = Path.Combine(filterDirectory, $"Filter_{filterGroupId:D2}_Lines.geojson");
                    string geoJsonContent = JsonConvert.SerializeObject(featureCollection, Formatting.Indented);
                    File.WriteAllText(outputFilePath, geoJsonContent);
                }
            }
        }
    }
}
