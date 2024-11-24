using System;
using ERAM_2_GEOJSON.Parsers;

namespace ERAM_2_GEOJSON
{
    class Program
    {
        static void Main(string[] args)
        {
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
                    Console.WriteLine($"Label Line 1: {record.LabelLine1}");
                    Console.WriteLine($"Label Line 2: {record.LabelLine2}");

                    foreach (var objectType in record.ObjectTypes)
                    {
                        Console.WriteLine($"  Object Type: {objectType.MapObjectType}");
                        Console.WriteLine($"  Map Group ID: {objectType.MapGroupId}");

                        // Display GeoMapSymbol details including overriding and default filter groups
                        foreach (var symbol in objectType.Symbols)
                        {
                            Console.WriteLine($"    Symbol ID: {symbol.SymbolId}");
                            Console.WriteLine($"    Latitude: {symbol.Latitude}");
                            Console.WriteLine($"    Longitude: {symbol.Longitude}");

                            // Using switch to determine which filter groups to display
                            switch (symbol.OverridingFilterGroups?.Count > 0)
                            {
                                case true:
                                    Console.WriteLine("    Default Properties Filter Groups: None");
                                    Console.WriteLine("    Overriding Properties Filter Groups:");
                                    foreach (var filterGroup in symbol.OverridingFilterGroups!)
                                    {
                                        Console.WriteLine($"      {filterGroup}");
                                    }
                                    break;

                                case false when symbol.FilterGroups.Count > 0:
                                    Console.WriteLine("    Default Properties Filter Groups:");
                                    foreach (var filterGroup in symbol.FilterGroups)
                                    {
                                        Console.WriteLine($"      {filterGroup}");
                                    }
                                    Console.WriteLine("    Overriding Properties Filter Groups: None");
                                    break;

                                default:
                                    Console.WriteLine("    Default Properties Filter Groups: None");
                                    Console.WriteLine("    Overriding Properties Filter Groups: None");
                                    break;
                            }
                        }

                        // Display GeoMapLine details including overriding and default filter groups
                        foreach (var line in objectType.Lines)
                        {
                            Console.WriteLine($"    Line Object ID: {line.LineObjectId}");
                            Console.WriteLine($"    Start Latitude: {line.StartLatitude}");
                            Console.WriteLine($"    Start Longitude: {line.StartLongitude}");
                            Console.WriteLine($"    End Latitude: {line.EndLatitude}");
                            Console.WriteLine($"    End Longitude: {line.EndLongitude}");

                            // Using switch to determine which filter groups to display
                            switch (line.OverridingFilterGroups?.Count > 0)
                            {
                                case true:
                                    Console.WriteLine("    Default Properties Filter Groups: None");
                                    Console.WriteLine("    Overriding Properties Filter Groups:");
                                    foreach (var filterGroup in line.OverridingFilterGroups!)
                                    {
                                        Console.WriteLine($"      {filterGroup}");
                                    }
                                    break;

                                case false when line.FilterGroups.Count > 0:
                                    Console.WriteLine("    Default Properties Filter Groups:");
                                    foreach (var filterGroup in line.FilterGroups)
                                    {
                                        Console.WriteLine($"      {filterGroup}");
                                    }
                                    Console.WriteLine("    Overriding Properties Filter Groups: None");
                                    break;

                                default:
                                    Console.WriteLine("    Default Properties Filter Groups: None");
                                    Console.WriteLine("    Overriding Properties Filter Groups: None");
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
