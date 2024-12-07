using System;
using System.IO;
using System.Linq;

namespace ERAM_2_GEOJSON.Helpers
{
    public static class DirectoryHandler
    {
        public static void CreateOutputDirectory(string outputDirectory)
        {
            try
            {
                // Check if the directory exists
                if (Directory.Exists(outputDirectory))
                {
                    // Delete the directory and all its contents
                    Directory.Delete(outputDirectory, true);
                    Console.WriteLine($"Deleted existing directory: {outputDirectory}");
                }

                // Create the directory
                Directory.CreateDirectory(outputDirectory);
                Console.WriteLine($"Created new directory: {outputDirectory}");
            }
            catch (Exception ex)
            {
                // Log the error and throw a new exception to indicate failure
                Console.WriteLine($"An error occurred while managing the directory: {ex.Message}");
                throw new InvalidOperationException($"Failed to create the output directory at: {outputDirectory}", ex);
            }
        }

        public static string CreateRecordDirectory(string outputDirectory, string recordId, string labelLine1, string labelLine2)
        {
            try
            {
                // Sanitize inputs of invalide characters
                string sanitizedRecordId = SanitizeForFileName(recordId);
                string sanitizedLabelLine1 = SanitizeForFileName(labelLine1);
                string sanitizedLabelLine2 = SanitizeForFileName(labelLine2);

                // Construct the Record Directory path ex: "MAIN-ZOB_MAPS"
                string recordDirectory = Path.Combine(outputDirectory, $"{sanitizedRecordId}-{sanitizedLabelLine1}_{sanitizedLabelLine2}");

                // Check if the directory exists
                if (!Directory.Exists(recordDirectory))
                {
                    // Create the directory
                    Directory.CreateDirectory(recordDirectory);
                    Console.WriteLine($"Created record directory: {recordDirectory}");
                }

                return recordDirectory;
            }
            catch (Exception ex)
            {
                // Log the error and throw a new exception to indicate failure
                Console.WriteLine($"An error occurred while creating the record directory: {ex.Message}");
                throw new InvalidOperationException($"Failed to create the record directory at: {outputDirectory}", ex);
            }
        }

        // Helper method to remove invalid characters from a string to make it safe for file or directory names
        private static string SanitizeForFileName(string input)
        {
            // Get invalid characters for file and directory names
            char[] invalidChars = Path.GetInvalidFileNameChars();

            // Remove any invalid character from the input string
            return new string(input.Where(ch => !invalidChars.Contains(ch)).ToArray());
        }
    }
}
