using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
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
                    foreach (var line in objectType.Lines)
                    {
                        GenerateLinesGeoJson(line, objectType, record, outputDirectory, customProperties);
                    }
                }
            }
        }

        private void GenerateLinesGeoJson(GeoMapLine line, GeoMapObjectType objectType, GeoMapRecord record, string outputDirectory, bool customProperties)
        {
            // Group lines by FilterGroup
            var filterGroupIds = line.OverridingFilterGroups ?? line.FilterGroups;
            foreach (var filterGroupId in filterGroupIds)
            {
                string filterDirectory = Path.Combine(outputDirectory, record.GeomapId, $"Filter_{filterGroupId:D2}");
                Directory.CreateDirectory(filterDirectory);

                // Convert line coordinates to GeographicPosition
                var coordinates = new List<IPosition>
                {
                    new Position(ConvertDMSToDecimal(line.StartLatitude), ConvertDMSToDecimal(line.StartLongitude)),
                    new Position(ConvertDMSToDecimal(line.EndLatitude), ConvertDMSToDecimal(line.EndLongitude))
                };

                LineString lineString = new LineString(coordinates);
                Feature feature = new Feature(lineString, GenerateProperties(line, objectType, record, customProperties));

                // Write GeoJSON to file
                string geoJsonFilePath = Path.Combine(filterDirectory, $"Filter_{filterGroupId:D2}_Lines.geojson");
                WriteGeoJsonFile(geoJsonFilePath, feature);
            }
        }

        private IDictionary<string, object> GenerateProperties(GeoMapLine line, GeoMapObjectType objectType, GeoMapRecord record, bool customProperties)
        {
            var properties = new Dictionary<string, object>
            {
                { "E2G_MapObjectType", objectType.MapObjectType },
                { "E2G_MapGroupId", objectType.MapGroupId },
                { "E2G_LineObjectId", line.LineObjectId }
            };

            if (customProperties)
            {
                properties.Add("custom", "true");
            }

            return properties;
        }

        private void WriteGeoJsonFile(string filePath, Feature feature)
        {
            FeatureCollection featureCollection = new FeatureCollection(new List<Feature> { feature });
            string geoJson = Newtonsoft.Json.JsonConvert.SerializeObject(featureCollection, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(filePath, geoJson);
        }

        private double ConvertDMSToDecimal(string dms)
        {
            return CoordinateConverter.ConvertDMSToDecimal(dms);
        }
    }
}
