using System;
using ERAM_2_GEOJSON.Parsers;
using ERAM_2_GEOJSON.Models;

namespace ERAM_2_GEOJSON
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define the path to the XML file.
            string inputXmlPath = "C:\\Users\\ksand\\source\\repos\\ERAM_2_GEOJSON\\Geomaps_lite-example.xml";

            try
            {
                // Create an instance of XmlParser.
                XmlParser parser = new XmlParser();

                // Parse the XML file.
                var geoMapRecords = parser.Parse(inputXmlPath);

                // Print the parsed data to verify correctness.
                foreach (var record in geoMapRecords)
                {
                    Console.WriteLine($"GeoMap ID: {record.GeomapId}");
                    Console.WriteLine($"Label Line 1: {record.LabelLine1}");
                    Console.WriteLine($"Label Line 2: {record.LabelLine2}");

                    foreach (var objectType in record.ObjectTypes)
                    {
                        Console.WriteLine($"  Object Type: {objectType.MapObjectType}");
                        Console.WriteLine($"  Map Group ID: {objectType.MapGroupId}");

                        foreach (var line in objectType.Lines)
                        {
                            Console.WriteLine($"    Line Object ID: {line.LineObjectId}");
                            Console.WriteLine($"    Start Latitude: {line.StartLatitude}");
                            Console.WriteLine($"    Start Longitude: {line.StartLongitude}");
                            Console.WriteLine($"    End Latitude: {line.EndLatitude}");
                            Console.WriteLine($"    End Longitude: {line.EndLongitude}");
                        }

                        foreach (var symbol in objectType.Symbols)
                        {
                            Console.WriteLine($"    Symbol ID: {symbol.SymbolId}");
                            Console.WriteLine($"    Latitude: {symbol.Latitude}");
                            Console.WriteLine($"    Longitude: {symbol.Longitude}");

                            if (symbol.GeoMapText != null)
                            {
                                Console.WriteLine($"      Text Line: {symbol.GeoMapText.TextLine}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
