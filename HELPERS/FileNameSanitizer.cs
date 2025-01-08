using System.IO;
using System.Linq;

namespace ERAM_2_GEOJSON.Helpers
{
    public static class FileNameSanitizer
    {
        /// <summary>
        /// Removes invalid characters from a string to make it safe for file or directory names.
        /// </summary>
        /// <param name="input">The input string to sanitize.</param>
        /// <returns>A sanitized string safe for use as a file or directory name.</returns>
        public static string SanitizeForFileName(string input)
        {
            // Get invalid characters for file and directory names
            char[] invalidChars = Path.GetInvalidFileNameChars();

            // Remove any invalid character from the input string
            return new string(input.Where(ch => !invalidChars.Contains(ch)).ToArray());
        }
    }
}
