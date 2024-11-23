using System;
using ERAM_2_GEOJSON.Parsers;

namespace ERAM_2_GEOJSON
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputXmlPath = "C:\\Users\\ksand\\source\\repos\\ERAM_2_GEOJSON\\Geomaps_lite-example.xml";

            try
            {
                XmlParser parser = new XmlParser();
                var geoMapRecords = parser.Parse(inputXmlPath);

                foreach (var record in geoMapRecords)
                {
                    Console.WriteLine($"GeoMap ID: {record.GeomapId}");
                    Console.WriteLine($"Label Line 1: {record.LabelLine1}");
                    Console.WriteLine($"Label Line 2: {record.LabelLine2}");

                    foreach (var objectType in record.ObjectTypes)
                    {
                        Console.WriteLine($"  Object Type: {objectType.MapObjectType}");
                        Console.WriteLine($"  Map Group ID: {objectType.MapGroupId}");

                        foreach (var symbol in objectType.Symbols)
                        {
                            Console.WriteLine($"    Symbol ID: {symbol.SymbolId}");
                            Console.WriteLine($"    Latitude: {symbol.Latitude}");
                            Console.WriteLine($"    Longitude: {symbol.Longitude}");

                            // Retrieve default and overriding filter groups
                            var defaultFilters = objectType.DefaultSymbolProperties?.GeoSymbolFilters;
                            var overridingFilters = symbol.FilterGroups;

                            if (overridingFilters != null && overridingFilters.Count > 0)
                            {
                                // Overriding filters exist; display them
                                Console.WriteLine("    Default Properties Filter Groups: None");
                                Console.WriteLine("    Overriding Properties Filter Groups:");
                                foreach (var filterGroup in overridingFilters)
                                {
                                    Console.WriteLine($"      {filterGroup}");
                                }
                            }
                            else if (defaultFilters != null && defaultFilters.Count > 0)
                            {
                                // No overriding filters; display default filters
                                Console.WriteLine("    Default Properties Filter Groups:");
                                foreach (var filterGroup in defaultFilters)
                                {
                                    Console.WriteLine($"      {filterGroup}");
                                }
                                Console.WriteLine("    Overriding Properties Filter Groups: None");
                            }
                            else
                            {
                                // Neither default nor overriding filters exist
                                Console.WriteLine("    Default Properties Filter Groups: None");
                                Console.WriteLine("    Overriding Properties Filter Groups: None");
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
