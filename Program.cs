using System;
using ERAM_2_GEOJSON.Parsers;
using ERAM_2_GEOJSON.Models;

namespace ERAM_2_GEOJSON
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: ERAM_2_GEOJSON <InputXmlPath> <OutputDirectory> [CustomProperties=true/false]");
                return;
            }

            string inputXmlPath = args[0];
            string outputDirectory = args[1];
            bool customProperties = args.Length > 2 && bool.TryParse(args[2], out bool result) ? result : false;

            try
            {
                var parser = new XmlParser();
                var geoMapRecords = parser.Parse(inputXmlPath);

                Console.WriteLine("Parsed GeoMap Records:");
                foreach (var record in geoMapRecords)
                {
                    Console.WriteLine($"GeoMap ID: {record.GeomapId}");
                    foreach (var objType in record.ObjectTypes)
                    {
                        Console.WriteLine($"\tObject Type: {objType.MapObjectType}, Group ID: {objType.MapGroupId}");
                        foreach (var line in objType.Lines)
                        {
                            Console.WriteLine($"\t\tLine Object ID: {line.LineObjectId}, Start: ({line.StartLatitude}, {line.StartLongitude}), End: ({line.EndLatitude}, {line.EndLongitude})");
                        }
                    }
                }

                // TODO: Generate GeoJSON from parsed records (next step).
                Console.WriteLine("GeoJSON generation would be initiated here.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
