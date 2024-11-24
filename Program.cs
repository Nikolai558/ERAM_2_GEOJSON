using System;
using ERAM_2_GEOJSON.Parsers;
using ERAM_2_GEOJSON.Helpers;

namespace ERAM_2_GEOJSON
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test coordinate conversion with DMS value
            string dmsValue = "42124729N";
            Console.WriteLine("Testing DMS to Decimal Conversion:");
            Console.WriteLine("Input DMS: " + dmsValue);
            double decimalValue = CoordinateConverter.ConvertDMSToDecimal(dmsValue);
            Console.WriteLine("Expected Decimal Value: 42.21313611");
            Console.WriteLine("Actual Decimal Value: " + decimalValue);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            // Get the current directory and XML file path
            string currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string inputXmlPath = $"{currentDirectory}\\Geomaps_lite-example.xml";

            try
            {
                // Parse the XML file to extract GeoMap records
                XmlParser parser = new XmlParser();
                var geoMapRecords = parser.Parse(inputXmlPath);

                // Iterate through each GeoMap record and display its properties
                foreach (var record in geoMapRecords)
                {
                    Console.WriteLine($"GeoMap ID: {record.GeomapId}");
                    Console.WriteLine($"  Label Line 1: {record.LabelLine1}");
                    Console.WriteLine($"  Label Line 2: {record.LabelLine2}");

                    foreach (var objectType in record.ObjectTypes)
                    {
                        Console.WriteLine($"    Object Type: {objectType.MapObjectType}");
                        Console.WriteLine($"    Map Group ID: {objectType.MapGroupId}");

                        // Display GeoMapSymbol details including overriding and default filter groups
                        foreach (var symbol in objectType.Symbols)
                        {
                            Console.WriteLine($"      Symbol ID: {symbol.SymbolId}");
                            Console.WriteLine($"        Latitude: {symbol.Latitude}");
                            Console.WriteLine($"        Longitude: {symbol.Longitude}");

                            // Using switch to determine which filter groups to display
                            switch (symbol.OverridingFilterGroups?.Count > 0)
                            {
                                case true:
                                    Console.WriteLine("        Using Overriding Properties Filter Groups: " +
                                                      string.Join(", ", symbol.OverridingFilterGroups!));
                                    break;

                                case false when symbol.FilterGroups.Count > 0:
                                    Console.WriteLine("        Using Default Properties Filter Groups: " +
                                                      string.Join(", ", symbol.FilterGroups));
                                    break;

                                default:
                                    Console.WriteLine("        WARNING: No default or overriding Properties Filter Groups found");
                                    break;
                            }
                        }

                        // Display GeoMapLine details including overriding and default filter groups
                        foreach (var line in objectType.Lines)
                        {
                            Console.WriteLine($"      Line Object ID: {line.LineObjectId}");
                            Console.WriteLine($"        Start Latitude: {line.StartLatitude}");
                            Console.WriteLine($"        Start Longitude: {line.StartLongitude}");
                            Console.WriteLine($"        End Latitude: {line.EndLatitude}");
                            Console.WriteLine($"        End Longitude: {line.EndLongitude}");

                            // Using switch to determine which filter groups to display
                            switch (line.OverridingFilterGroups?.Count > 0)
                            {
                                case true:
                                    Console.WriteLine("        Using Overriding Properties Filter Groups: " +
                                                      string.Join(", ", line.OverridingFilterGroups!));
                                    break;

                                case false when line.FilterGroups.Count > 0:
                                    Console.WriteLine("        Using Default Properties Filter Groups: " +
                                                      string.Join(", ", line.FilterGroups));
                                    break;

                                default:
                                    Console.WriteLine("        WARNING: No default or overriding Properties Filter Groups found");
                                    break;
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
