using System;
using System.IO;
using System.Xml.Serialization;
using Models;

namespace Parsers
{
    public static class ConsoleCommandControlXmlParser
    {
        /// <summary>
        /// Parses the ConsoleCommandControl.xml file and deserializes it into a ConsoleCommandControl_Records object.
        /// </summary>
        /// <param name="xmlFilePath">Path to the ConsoleCommandControl.xml file.</param>
        /// <returns>Deserialized ConsoleCommandControl_Records object.</returns>
        public static ConsoleCommandControl_Records Deserialize(string xmlFilePath)
        {
            // Check if the provided file path is null or empty
            if (string.IsNullOrWhiteSpace(xmlFilePath))
                throw new ArgumentException("The XML file path is null or empty.", nameof(xmlFilePath));

            // Check if the file exists at the provided path
            if (!File.Exists(xmlFilePath))
                throw new FileNotFoundException($"The file {xmlFilePath} does not exist.");

            try
            {
                // Initialize the XmlSerializer with the root type (ConsoleCommandControl_Records)
                var serializer = new XmlSerializer(typeof(ConsoleCommandControl_Records));

                // Open the file for reading and deserialize the content
                using (var reader = new StreamReader(xmlFilePath))
                {
                    return (ConsoleCommandControl_Records)serializer.Deserialize(reader);
                }
            }
            catch (InvalidOperationException ex)
            {
                // Catch and re-throw any errors during deserialization with additional context
                throw new Exception($"An error occurred while deserializing the XML file: {ex.Message}", ex);
            }
        }
    }
}
