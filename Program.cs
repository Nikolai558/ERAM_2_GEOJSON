using ERAM_2_GEOJSON.Helpers;
using ERAM_2_GEOJSON.Models;
using ERAM_2_GEOJSON.Parsers;
using System;
using System.Collections.Generic;
using System.IO;

namespace ERAM_2_GEOJSON
{
    class Program
    {
        static void Main(string[] args)
        {
            // Declare file names
            string geomapXmlFileName = "Geomaps.xml";
            // TODO: Write code that will parse this file and then un-comment out this line.
            //string consoleCommandControlFileName = "ConsoleCommandControl.xml";

            // TODO: Delete or comment-out prior to release
            if (args.Length == 0) // Only override if no arguments are passed
            {
                args = new string[]
                {
                    @"C:\Users\ksand\source\repos\ERAM_2_GEOJSON\GENERAL_RESOURCES\",
                    @"C:\Users\ksand\downloads\",
                    "true"
                };

                geomapXmlFileName = "Geomaps_lite-example.xml";
                // TODO: Write code that will parse this file and then un-comment out this line.
                //consoleCommandControlFileName = "ConsoleCommandControl.xml";
            }

            // Validate the number of arguments
            if (args.Length != 3)
            {
                Console.WriteLine(@"ERROR: Expected args to be passed as follows:");
                Console.WriteLine(@"     sourceFileDirectory userSelectedOutputDirectory includeCustomProperties(true/false)");
                Console.WriteLine(@"Note: Wrap paths with spaces in double quotes; For example:");
                Console.WriteLine(@"       ""C:\Users\username has a space in it\desktop\geomaps"" ""C:\Users\username\desktop\Project Folder"" true");
                return;
            }

            try
            {
                // Set args to variables
                string sourceFileDirectory = args[0];
                string userSelectedOutputDirectory = args[1];
                bool includeCustomProperties = bool.Parse(args[2]); // Converted to a boolean

                // Create file path variables using userSelectedOutputDirectory and the file name variables
                string geomapXmlFilePath = Path.Combine(sourceFileDirectory, geomapXmlFileName);
                // TODO: Write code that will parse this file and then un-comment out this line.
                //string consoleCommandControlXmlFilePath = Path.Combine(sourceFileDirectory, consoleCommandControlFileName);

                // Ensure the source Geomap.xml file exists
                if (!File.Exists(geomapXmlFilePath))
                {
                    Console.WriteLine($"Error: Could not find '{geomapXmlFileName}' here:");
                    Console.WriteLine($"       '{userSelectedOutputDirectory}'");
                    return;
                }

                // Checks if the ConsoleCommandControl.xml file exists and prints a notificaiton message if not.
                // TODO: Write code that will parse this file and then un-comment out this line.
                //if (!File.Exists(consoleCommandControlXmlFilePath))
                //{
                //    Console.WriteLine($"Information: Could not find '{consoleCommandControlFileName}' here, but is not required:");
                //    Console.WriteLine($"             '{userSelectedOutputDirectory}'");
                //}

                // Parse the geomapXML file into GeoMapRecords
                List<GeoMapRecord> geoMapRecords = GeomapXmlParser.Parse(geomapXmlFilePath);

                // Create the output directory
                string outputDirectory = Path.Combine(userSelectedOutputDirectory, "ERAM_2_GEOJSON_OUTPUT");
                DirectoryHandler.CreateOutputDirectory(outputDirectory);

                // Generate GeoJSON
                GeoJsonGenerator generator = new GeoJsonGenerator();
                generator.GenerateGeoJson(geoMapRecords, outputDirectory, includeCustomProperties);

                Console.WriteLine("GeoJSON generation complete.");
                // TODO: Write code that will parse this file and then un-comment out this line.
                //Console.WriteLine("ConsoleCommandControl generation complete.");
                Console.WriteLine("Check the specified output directory for the results.");
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: The third argument (includeCustomProperties) must be 'true' or 'false'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
