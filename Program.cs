using ERAM_2_GEOJSON.Helpers;
using ERAM_2_GEOJSON.Models;
using ERAM_2_GEOJSON.Parsers;
using ERAM_2_GEOJSON.UI;
using Helpers;
using Models;
using Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ERAM_2_GEOJSON
{
    class Program
    {
        // TODO: Update version number
        private const string CurrentVersion = "1.0.0rc1";
        static void Main()
        {
            // TODO: Check testing status before release.
            bool testing = false; // if testing=true, UI and versionCheck are disabled

            // Set the title of the console window
            if (testing)
            {
                Console.Title = $"E2G (testing)";
            }
            else
            {
                Console.Title = $"E2G (v{CurrentVersion})";
            }

            // Declare file names and variables
            string geomapXmlFileName = "Geomaps.xml"; // Required file containing all geomaps.
            string consoleCommandControlFileName = "ConsoleCommandControl.xml"; // Optional file containing menu labels.
            string sourceFileDirectory; // user selected directory to find required files.
            string userSelectedOutputDirectory; // user selected directory for output. Code will append "\E2G" as needed.
            string outputByFormat; // user selected output format of the geojsons (filter/attributes/raw).
            bool includeCustomProperties; // determines if the geojson features will include organizing/debugging properties.

            // Assign variables based on testing status
            if (testing)
            {
                // Assign variables
                sourceFileDirectory = @"C:\Users\ksand\OneDrive\Desktop\ProjFolder\c427ac12_ZOB_121224_103124_T\";
                userSelectedOutputDirectory = @"C:\Users\ksand\downloads\";
                outputByFormat = "raw";
                includeCustomProperties = false;
            }
            else
            {
                // TODO: Uncomment and test once GitHub is back online.
                // Check for new version. Not available if testing=true
                Console.WriteLine("Checking for updates...\n\n");
                VersionCheck versionCheck = new VersionCheck(CurrentVersion);
                versionCheck.CheckForUpdates();
                Console.Clear();

                // Launch UI to assign variables
                (sourceFileDirectory, userSelectedOutputDirectory, outputByFormat, includeCustomProperties) = UserInterface.Start(geomapXmlFileName, consoleCommandControlFileName);
            }

            // Create file path variables using userSelectedOutputDirectory and the file name variables.
            string geomapXmlFilePath = Path.Combine(sourceFileDirectory, geomapXmlFileName);
            string consoleCommandControlXmlFilePath = Path.Combine(sourceFileDirectory, consoleCommandControlFileName);

            // Ensure the source Geomap.xml file exists in chosen directory
            if (!File.Exists(geomapXmlFilePath))
            {
                Console.WriteLine($"\n\nError: Could not find '{geomapXmlFileName}' here:");
                Console.WriteLine($"       '{userSelectedOutputDirectory}'\n\n");
                return;
            }

            try
            {
                // Parse the geomapXML file into GeoMapRecords
                Console.Write($"Parsing '{geomapXmlFileName}'...");
                var (geoMapRecords, bcgMenuToGeomapIds, filterMenuToGeomapIds) = GeomapXmlParser.Parse(geomapXmlFilePath);
                Console.WriteLine(" Complete");

                // Create the output directory
                string outputDirectory = Path.Combine(userSelectedOutputDirectory, "E2G_OUTPUT");
                DirectoryHandler.CreateOutputDirectory(outputDirectory);

                Console.Write($"\nCreating geojsons...");
                if (outputByFormat == "filters")
                {
                    // Generate GeoJSON by Filters
                    GeoJsonGeneratorByFilters generator = new GeoJsonGeneratorByFilters();
                    generator.GenerateGeoJson(geoMapRecords, outputDirectory, includeCustomProperties);
                }
                else if (outputByFormat == "attributes")
                {
                    // Generate GeoJSON by Attributes
                    GeoJsonGeneratorByAttributes generator = new GeoJsonGeneratorByAttributes();
                    generator.GenerateGeoJson(geoMapRecords, outputDirectory, includeCustomProperties);
                }
                else if (outputByFormat == "raw")
                {
                    // Generate GeoJSON by Attributes
                    GeoJsonGeneratorByRaw generator = new GeoJsonGeneratorByRaw();
                    generator.GenerateGeoJson(geoMapRecords, outputDirectory, includeCustomProperties);
                }
                else
                {
                    Console.WriteLine($"ERROR: outputByFormat set to '{outputByFormat}'");
                    return;
                }

                // Console.WriteLine("\n\n\t\t\tGeojsons complete.");
                Console.WriteLine(" Complete");

                // Add isDefault features to ByFilters and ByAttributes .geojsons that were output.
                if (!(outputByFormat == "raw"))
                 {
                     Console.Write($"\nAdding isDefaults to output files...");
                     AddIsDefaults.AddToGeoJsonFiles(outputDirectory);
                     // Console.WriteLine($"\n\n\t\t\tisDefaults added.");
                     Console.WriteLine(" Complete");
                }

                // Process ConsoleCommandControl.xml if it exists
                if (File.Exists(consoleCommandControlXmlFilePath))
                {

                    // Parse the ConsoleCommandControl.xml
                    Console.Write($"\nParsing & creating 'ConsoleCommandControl' data...");
                    ConsoleCommandControl_Records consoleCommandControlRecords = ConsoleCommandControlXmlParser.Deserialize(consoleCommandControlXmlFilePath);

                    // Generate ConsoleCommandControl.txt
                    string consoleCommandControlOutputPath = Path.Combine(outputDirectory, "ConsoleCommandControl.txt");
                    ConsoleCommandControlTxtGenerator.GenerateTxtFile(consoleCommandControlOutputPath, consoleCommandControlRecords, bcgMenuToGeomapIds, filterMenuToGeomapIds);

                    //Console.WriteLine("\n\n\t\t\tConsoleCommandControl.txt Complete.");
                    Console.WriteLine(" Complete");
                }

                // Process is done, prompt user to open the directory where files are stored.
                Console.WriteLine("\n\nDONE!\n\n");
                Console.WriteLine("         Your files have been saved here:");
                Console.WriteLine($"         {outputDirectory}");
                Console.WriteLine("\n\nPress any key to open that folder and exit this program...");
                Console.ReadKey();

                // Open a windows explorer window to the outputDirectory
                if (System.IO.Directory.Exists(outputDirectory))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = outputDirectory,
                        UseShellExecute = true
                    });
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
