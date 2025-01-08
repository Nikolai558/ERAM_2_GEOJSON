using System;
using System.IO;
using ERAM_2_GEOJSON.Helpers;

namespace ERAM_2_GEOJSON.Helpers
{
    /// <summary>
    /// Provides utility methods to manage directories for the application.
    /// </summary>
    public static class DirectoryHandler
    {
        /// <summary>
        /// Creates the output directory after deleting it if it already exists.
        /// </summary>
        /// <param name="outputDirectory">The path to the output directory.</param>
        /// <exception cref="InvalidOperationException">Thrown when directory creation fails.</exception>
        public static void CreateOutputDirectory(string outputDirectory)
        {
            try
            {
                // If the directory exists, delete it along with its contents
                if (Directory.Exists(outputDirectory))
                {
                    Directory.Delete(outputDirectory, true);
                }

                // Create a new directory at the specified path
                Directory.CreateDirectory(outputDirectory);
            }
            catch (Exception ex)
            {
                // Log the error and throw a more meaningful exception
                Console.WriteLine($"An error occurred while managing the directory: {ex.Message}");
                throw new InvalidOperationException($"Failed to create the output directory at: {outputDirectory}", ex);
            }
        }

        /// <summary>
        /// Creates a directory for a specific record using sanitized inputs.
        /// </summary>
        /// <param name="outputDirectory">The base output directory.</param>
        /// <param name="recordId">The record identifier.</param>
        /// <param name="labelLine1">The first label line for naming.</param>
        /// <param name="labelLine2">The second label line for naming.</param>
        /// <returns>The path to the created record directory.</returns>
        /// <exception cref="InvalidOperationException">Thrown when directory creation fails.</exception>
        public static string CreateRecordDirectory(string outputDirectory, string recordId, string labelLine1, string labelLine2)
        {
            try
            {
                // Sanitize inputs to ensure safe file or directory names
                string sanitizedRecordId = FileNameSanitizer.SanitizeForFileName(recordId);
                string sanitizedLabelLine1 = FileNameSanitizer.SanitizeForFileName(labelLine1);
                string sanitizedLabelLine2 = FileNameSanitizer.SanitizeForFileName(labelLine2);

                // Construct the directory path based on sanitized inputs
                string recordDirectory = Path.Combine(outputDirectory, $"{sanitizedRecordId}-{sanitizedLabelLine1}_{sanitizedLabelLine2}");

                // Create the directory if it does not already exist
                if (!Directory.Exists(recordDirectory))
                {
                    Directory.CreateDirectory(recordDirectory);
                }

                return recordDirectory; // Return the path to the created directory
            }
            catch (Exception ex)
            {
                // Log the error and throw a more meaningful exception
                Console.WriteLine($"An error occurred while creating the record directory: {ex.Message}");
                throw new InvalidOperationException($"Failed to create the record directory at: {outputDirectory}", ex);
            }
        }
    }
}
