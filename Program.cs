using System;
using System.IO;
using ERAM_2_GEOJSON.Parsers;
using ERAM_2_GEOJSON.GeoJson;

namespace ERAM_2_GEOJSON
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get the current directory and XML file path
            string currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string inputXmlPath = $"{currentDirectory}\\Geomaps_lite-example.xml";
            string outputDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "ERAM_2_GEOJSON_Output");

            try
            {
                // Deletes previous directory if found
                if (Directory.Exists(outputDirectory))
                {
                    // Delete the directory and all its contents
                    Directory.Delete(outputDirectory, true);
                    Console.WriteLine($"Deleted existing output directory: {outputDirectory}");
                }

                // Create a new directory
                Directory.CreateDirectory(outputDirectory);
                Console.WriteLine($"Created output directory at: {outputDirectory}");

                // Parse the XML file to extract GeoMap records
                XmlParser parser = new XmlParser();
                var geoMapRecords = parser.Parse(inputXmlPath);

                // Generate GeoJSON files from the parsed data
                GeoJsonGenerator generator = new GeoJsonGenerator();
                generator.GenerateGeoJson(geoMapRecords, outputDirectory, customProperties: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
