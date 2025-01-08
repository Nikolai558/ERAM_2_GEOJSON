using System;
using System.IO;

namespace ERAM_2_GEOJSON.UI
{
    public static class UserInterface
    {
        /// <summary>
        /// Provides a display for the user to select their options for converting their files.
        /// </summary>
        public static (string sourceFileDirectory, string userSelectedOutputDirectory, string outputByFormat, bool includeCustomProperties) Start(string geomapXmlFileName, string consoleCommandControlFileName)
        {
            // Hello Page
            Console.WriteLine("Welcome to ERAM_2_GEOJSON!\n\n");
            Console.WriteLine($"This tool converts the FAA ERAM {geomapXmlFileName} file into GeoJSON files.\n");
            Console.WriteLine($"If {consoleCommandControlFileName} is available in the same directory as {geomapXmlFileName},");
            Console.WriteLine($"a comprehensive .txt file will be created from that data.\n\n");
            Console.WriteLine($"For additional details concerning how the geojson features are created");
            Console.WriteLine($"with the different output format options, reference the README.md file found here:\n");
            Console.WriteLine($"https://github.com/Nikolai558/ERAM_2_GEOJSON\n\n");
            Console.WriteLine($"Press any key to continue...");
            Console.ReadKey();

            // Step 1: Source Directory Input
            Console.Clear();
            Console.WriteLine("SOURCE DIRECTORY\n");
            Console.WriteLine($"Copy and Paste the path to the directory that contains the '{geomapXmlFileName}' file.");
            Console.WriteLine($"Note: If you have the '{consoleCommandControlFileName}' file, ensure it is in this same directory.\n");
            string sourceFileDirectory = PromptForDirectory($"Enter (copy/paste) the source directory:", geomapXmlFileName);

            // Step 2: Output Directory Input
            Console.Clear();
            Console.WriteLine("OUTPUT DIRECTORY\n");
            Console.WriteLine("Copy and Paste the path to the directory where you wish to have the E2G_OUTPUT folder created and files output to.");
            Console.WriteLine($"WARNING... If the E2G_OUTPUT folder already exists in the directory you provide, it will be deleted and remade for these new files.\n");
            string userSelectedOutputDirectory = PromptForUserSelectedOutputDirectory("Enter (copy/paste) the output directory where results will be saved:");

            // Step 3: Select Output outputByFormat
            Console.Clear();
            Console.WriteLine("SELECT OUTPUT FORMAT:\n");
            Console.WriteLine("\t1 - By Filters (grouped by filter assignments)");
            Console.WriteLine("\t2 - By Attributes (grouped by object attributes)");
            Console.WriteLine("\t3 - Raw (ungrouped output)");
            string outputByFormat = GetOutputoutputByFormat();

            // Step 4: Include Custom Properties
            Console.Clear();
            Console.WriteLine("E2G-CUSTOM PROPERTIES\n");
            Console.WriteLine("These optional properties provide additional details for organization and data-review.\n");
            Console.WriteLine("Example:");
            Console.WriteLine("\t\"properties\": {");
            Console.WriteLine("\t  \"E2G_MapObjectType\": \"SupplementalLine\",");
            Console.WriteLine("\t  \"E2G_MapGroupId\": \"65\",");
            Console.WriteLine("\t  \"E2G_LineObjectId\": \"KPIT_DEP\"");
            Console.WriteLine("\t}\n\n");
            Console.WriteLine("NOTE... including these properties may increase your output data size by about 40%.\n\n");
            Console.Write("Include E2G-custom properties? (T/F): ");
            bool includeCustomProperties = GetBooleanInputFromTF();

            // Confirmation
            Console.Clear();
            Console.WriteLine("REVIEW SETTINGS\n");
            Console.WriteLine($"\tSource Directory:\t\t{sourceFileDirectory}");
            Console.WriteLine($"\tOutput Directory:\t\t{userSelectedOutputDirectory}");
            Console.WriteLine($"\tOutput Format:\t\t\t{outputByFormat}");
            Console.WriteLine($"\tInclude Custom Properties:\t{includeCustomProperties}\n\n");

            Console.Write("Proceed with these settings? (Y/N): ");
            if (!ConfirmAction())
            {
                Console.WriteLine("Operation cancelled by user.");
                Environment.Exit(0);
            }

            Console.Clear();
            return (sourceFileDirectory, userSelectedOutputDirectory, outputByFormat, includeCustomProperties);
        }

        private static string PromptForDirectory(string message, string requiredFile = null)
        {
            string directory;
            while (true)
            {
                Console.WriteLine(message);
                directory = Console.ReadLine().Trim('"');

                if (Directory.Exists(directory))
                {
                    if (requiredFile != null && !File.Exists(Path.Combine(directory, requiredFile)))
                    {
                        Console.WriteLine($"Error: '{requiredFile}' not found here:\n'{directory}'");
                        continue;
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("Error: Directory does not exist. Please enter a valid directory.");
                }
            }
            return directory;
        }

        private static string PromptForUserSelectedOutputDirectory(string message)
        {
            string directory;
            while (true)
            {
                Console.WriteLine(message);
                directory = Console.ReadLine().Trim('"');

                if (Directory.Exists(directory))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Error: Directory does not exist. Please enter a valid directory.");
                }
            }
            return directory;
        }

        private static string GetOutputoutputByFormat()
        {
            while (true)
            {
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1": return "filters";
                    case "2": return "attributes";
                    case "3": return "raw";
                    default:
                        Console.WriteLine("Invalid selection. Please enter 1, 2, or 3.");
                        break;
                }
            }
        }

        private static bool GetBooleanInputFromTF()
        {
            while (true)
            {
                string input = Console.ReadLine().Trim().ToLower();
                if (input == "t") return true;
                if (input == "f") return false;
                Console.WriteLine("Invalid input. Enter 'T' or 'F': ");
            }
        }

        private static bool ConfirmAction()
        {
            string input = Console.ReadLine().Trim().ToLower();
            return input == "y" || input == "yes";
        }
    }
}
