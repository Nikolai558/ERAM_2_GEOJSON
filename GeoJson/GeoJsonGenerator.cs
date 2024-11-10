using System.Collections.Generic;
using System.IO;
using System.Xml;
using ERAM_2_GEOJSON.Models;
using Newtonsoft.Json;


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
                    // Handle Lines
                    GenerateLinesGeoJson(objectType.Lines, outputDirectory, customProperties, record.GeomapId, record.LabelLine1, record.LabelLine2);

                    // Handle Symbols and Text similarly...
                }
            }
        }

        private void GenerateLinesGeoJson(List<GeoMapLine> lines, string outputDirectory, bool customProperties, string geomapId, string labelLine1, string labelLine2)
        {
            // Example logic to generate GeoJSON for lines and write to a file
            var features = new List<object>();

            foreach (var line in lines)
            {
                double startLat = Helpers.CoordinateConverter.ConvertDMSToDecimal(line.StartLatitude);
                double startLon = Helpers.CoordinateConverter.ConvertDMSToDecimal(line.StartLongitude);
                double endLat = Helpers.CoordinateConverter.ConvertDMSToDecimal(line.EndLatitude);
                double endLon = Helpers.CoordinateConverter.ConvertDMSToDecimal(line.EndLongitude);

                var feature = new
                {
                    type = "Feature",
                    geometry = new
                    {
                        type = "LineString",
                        coordinates = new[]
                        {
                            new[] { startLon, startLat },
                            new[] { endLon, endLat }
                        }
                    },
                    properties = new Dictionary<string, object>()
                };

                if (customProperties)
                {
                    feature.properties.Add("ksanders7070_MapObjectType", line.LineObjectId);
                }

                features.Add(feature);
            }

            var geoJson = new
            {
                type = "FeatureCollection",
                features = features
            };

            string outputPath = Path.Combine(outputDirectory, $"{geomapId}_{labelLine1}_{labelLine2}_Lines.geojson");
            File.WriteAllText(outputPath, JsonConvert.SerializeObject(geoJson, Newtonsoft.Json.Formatting.Indented));
        }
    }
}
