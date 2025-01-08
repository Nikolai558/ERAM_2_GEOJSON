using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ERAM_2_GEOJSON.Models;

namespace ERAM_2_GEOJSON.Parsers
{
    public static class GeomapXmlParser
    {
        /// <summary>
        /// Parses the source Geomaps.xml file.
        /// </summary>
        /// <param name="filePath">Path to the Geomaps.xml file.</param>
        /// <returns>
        /// A tuple containing:
        /// - List of GeoMapRecord objects (Records)
        /// - Dictionary mapping BCG menu names to sets of GeoMap IDs (BcgMenuToGeomapIds)
        /// - Dictionary mapping filter menu names to sets of GeoMap IDs (FilterMenuToGeomapIds)
        /// </returns>
        public static (List<GeoMapRecord> Records, Dictionary<string, HashSet<string>> BcgMenuToGeomapIds, Dictionary<string, HashSet<string>> FilterMenuToGeomapIds) Parse(string filePath)
        {
            // Initialize the output collectionsd
            var geoMapRecords = new List<GeoMapRecord>();
            var bcgMenuToGeomapIds = new Dictionary<string, HashSet<string>>();
            var filterMenuToGeomapIds = new Dictionary<string, HashSet<string>>();

            // Load the XML file into an XmlDocument
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            // Select all nodes representing GeoMapRecord elements
            var geoMapRecordNodes = xmlDoc.SelectNodes("//GeoMapRecord");

            // Process each GeoMapRecord node
            foreach (XmlNode geoMapRecordNode in geoMapRecordNodes)
            {
                // Parse individual GeoMapRecord elements
                var geoMapRecord = new GeoMapRecord
                {
                    GeomapId = geoMapRecordNode["GeomapId"]?.InnerText ?? throw new Exception("GeomapId is required."),
                    BcgMenuName = geoMapRecordNode["BCGMenuName"]?.InnerText ?? throw new Exception("BCGMenuName is required."),
                    FilterMenuName = geoMapRecordNode["FilterMenuName"]?.InnerText ?? throw new Exception("FilterMenuName is required."),
                    LabelLine1 = geoMapRecordNode["LabelLine1"]?.InnerText ?? "LL1", // Default to "LL1" if missing
                    LabelLine2 = geoMapRecordNode["LabelLine2"]?.InnerText ?? "LL2", // Default to "LL2" if missing
                };

                // Populate the BCG menu-to-GeoMap ID mappings
                // Check if the BCGMenuName field of the current GeoMapRecord is not null, empty, or whitespace
                if (!string.IsNullOrWhiteSpace(geoMapRecord.BcgMenuName))
                {
                    // If the BCG menu name does not already exist in the bcgMenuToGeomapIds dictionary
                    if (!bcgMenuToGeomapIds.ContainsKey(geoMapRecord.BcgMenuName))

                        // Initialize a new HashSet for this BCG menu name
                        // HashSet ensures that GeoMap IDs are stored without duplicates
                        bcgMenuToGeomapIds[geoMapRecord.BcgMenuName] = new HashSet<string>();

                    // Add the GeoMap ID of the current record to the HashSet associated with the BCG menu name
                    // This establishes a many-to-one relationship where multiple GeoMap IDs can map to a single BCG menu
                    bcgMenuToGeomapIds[geoMapRecord.BcgMenuName].Add(geoMapRecord.GeomapId);
                }

                // Populate the filter menu-to-GeoMap ID mappings
                // Check if the FilterMenuName field of the current GeoMapRecord is not null, empty, or whitespace
                if (!string.IsNullOrWhiteSpace(geoMapRecord.FilterMenuName))
                {
                    // If the filter menu name does not already exist in the filterMenuToGeomapIds dictionary
                    if (!filterMenuToGeomapIds.ContainsKey(geoMapRecord.FilterMenuName))

                        // Initialize a new HashSet for this filter menu name
                        // HashSet ensures that GeoMap IDs are stored without duplicates
                        filterMenuToGeomapIds[geoMapRecord.FilterMenuName] = new HashSet<string>();

                    // Add the GeoMap ID of the current record to the HashSet associated with the filter menu name
                    // This establishes a many-to-one relationship where multiple GeoMap IDs can map to a single filter menu
                    filterMenuToGeomapIds[geoMapRecord.FilterMenuName].Add(geoMapRecord.GeomapId);
                }

                // Console.WriteLine($"\t\t\t{geoMapRecord.GeomapId}");

                // Parse associated GeoMapObjectType nodes
                // Select all GeoMapObjectType nodes within the current GeoMapRecord node
                var geoMapObjectTypeNodes = geoMapRecordNode.SelectNodes("GeoMapObjectType");

                foreach (XmlNode geoMapObjectTypeNode in geoMapObjectTypeNodes)
                {
                    // Create a new GeoMapObjectType instance and populate its properties/attributes
                    var geoMapObjectType = new GeoMapObjectType
                    {

                        // Get GeoMapObjectType attributes
                        MapObjectType = geoMapObjectTypeNode["MapObjectType"]?.InnerText ?? throw new Exception("MapObjectType is required."),
                        MapGroupId = geoMapObjectTypeNode["MapGroupId"]?.InnerText ?? throw new Exception("MapGroupId is required."),

                        // Get Line attributes
                        DefaultLineFilters = ParseFilterGroups(geoMapObjectTypeNode.SelectSingleNode("DefaultLineProperties/GeoLineFilters")),
                        DefaultLineStyle = ParseString(geoMapObjectTypeNode.SelectSingleNode("DefaultLineProperties/LineStyle")),
                        DefaultLineBcgGroup = ParseInteger(geoMapObjectTypeNode.SelectSingleNode("DefaultLineProperties/BCGGroup")),
                        DefaultLineThickness = ParseInteger(geoMapObjectTypeNode.SelectSingleNode("DefaultLineProperties/Thickness")),

                        // Get Symbol attributes
                        DefaultSymbolFilters = ParseFilterGroups(geoMapObjectTypeNode.SelectSingleNode("DefaultSymbolProperties/GeoSymbolFilters")),
                        DefaultSymbolStyle = ParseString(geoMapObjectTypeNode.SelectSingleNode("DefaultSymbolProperties/SymbolStyle")),
                        DefaultSymbolBcgGroup = ParseInteger(geoMapObjectTypeNode.SelectSingleNode("DefaultSymbolProperties/BCGGroup")),
                        DefaultSymbolFontSize = ParseInteger(geoMapObjectTypeNode.SelectSingleNode("DefaultSymbolProperties/FontSize")),

                        // Get Text attributes
                        DefaultTextFilters = ParseFilterGroups(geoMapObjectTypeNode.SelectSingleNode("TextDefaultProperties/GeoTextFilters")),
                        DefaultTextBcgGroup = ParseInteger(geoMapObjectTypeNode.SelectSingleNode("TextDefaultProperties/BCGGroup")),
                        DefaultTextFontSize = ParseInteger(geoMapObjectTypeNode.SelectSingleNode("TextDefaultProperties/FontSize")),
                        DefaultTextUnderline = ParseBoolean(geoMapObjectTypeNode.SelectSingleNode("TextDefaultProperties/Underline")),
                        DefaultTextDisplaySetting = ParseBoolean(geoMapObjectTypeNode.SelectSingleNode("TextDefaultProperties/DisplaySetting")),
                        DefaultTextXPixelOffset = ParseInteger(geoMapObjectTypeNode.SelectSingleNode("TextDefaultProperties/XPixelOffset")),
                        DefaultTextYPixelOffset = ParseInteger(geoMapObjectTypeNode.SelectSingleNode("TextDefaultProperties/YPixelOffset"))
                    };

                    // Parse GeoMapLine objects
                    // Select all GeoMapLine nodes within the current GeoMapObjectType node
                    var geoMapLineNodes = geoMapObjectTypeNode.SelectNodes("GeoMapLine");
                    foreach (XmlNode geoMapLineNode in geoMapLineNodes)
                    {
                        // Create a new GeoMapLine instance and populate its properties
                        var geoMapLine = new GeoMapLine
                        {
                            LineObjectId = geoMapLineNode["LineObjectId"]?.InnerText ?? throw new Exception("LineObjectId is required."),
                            StartLatitude = geoMapLineNode["StartLatitude"]?.InnerText ?? throw new Exception("StartLatitude is required."),
                            StartLongitude = geoMapLineNode["StartLongitude"]?.InnerText ?? throw new Exception("StartLongitude is required."),
                            EndLatitude = geoMapLineNode["EndLatitude"]?.InnerText ?? throw new Exception("EndLatitude is required."),
                            EndLongitude = geoMapLineNode["EndLongitude"]?.InnerText ?? throw new Exception("EndLongitude is required."),
                            OverridingLineFilterGroups = ParseFilterGroups(geoMapLineNode.SelectSingleNode("GeoLineFilters")),
                            OverridingLineBcgGroup = ParseInteger(geoMapLineNode.SelectSingleNode("BCGGroup")),
                            OverridingLineThickness = ParseInteger(geoMapLineNode.SelectSingleNode("Thickness"))
                        };

                        // Determine the effective (applied) filters for the line
                        // If specific filter groups are defined at the line level, use them; otherwise, use the default filters
                        geoMapLine.AppliedLineFilters = geoMapLine.OverridingLineFilterGroups.Count > 0
                            ? geoMapLine.OverridingLineFilterGroups
                            : geoMapObjectType.DefaultLineFilters;

                        // Check if AppliedLineFilters is null or empty and assign a value of 0 if so.
                        // In CRC, a null filter value indicates "Always Display" and it appears that is what RW ERAM does, too.
                        if (geoMapLine.AppliedLineFilters == null || !geoMapLine.AppliedLineFilters.Any())
                        {
                            geoMapLine.AppliedLineFilters = new List<int> { 0 };
                        }

                        // Use the overriding style if defined; otherwise, use the default style
                        geoMapLine.AppliedLineStyle = geoMapLine.OverridingLineStyle ?? geoMapObjectType.DefaultLineStyle;

                        // Determine the effective (applied) Brightness Control Group for the line
                        geoMapLine.AppliedLineBcgGroup = geoMapLine.OverridingLineBcgGroup ?? geoMapObjectType.DefaultLineBcgGroup;

                        // Determine the effective (applied) thickness for the line
                        geoMapLine.AppliedLineThickness = geoMapLine.OverridingLineThickness ?? geoMapObjectType.DefaultLineThickness;

                        // Add the parsed GeoMapLine to the list of lines in the current GeoMapObjectType
                        geoMapObjectType.Lines.Add(geoMapLine);
                    }

                    // Parse GeoMapSymbol objects
                    // Select all GeoMapSymbol nodes within the current GeoMapObjectType node
                    var geoMapSymbolNodes = geoMapObjectTypeNode.SelectNodes("GeoMapSymbol");
                    foreach (XmlNode geoMapSymbolNode in geoMapSymbolNodes)
                    {
                        // Create a new GeoMapSymbol instance and populate its properties
                        var geoMapSymbol = new GeoMapSymbol
                        {
                            SymbolId = geoMapSymbolNode["SymbolId"]?.InnerText ?? throw new Exception("SymbolId is required."),
                            Latitude = geoMapSymbolNode["Latitude"]?.InnerText ?? throw new Exception("Latitude is required."),
                            Longitude = geoMapSymbolNode["Longitude"]?.InnerText ?? throw new Exception("Longitude is required."),
                            OverridingSymbolFiltersGroups = ParseFilterGroups(geoMapSymbolNode.SelectSingleNode("GeoSymbolFilters")),
                            OverridingSymbolBcgGroup = ParseInteger(geoMapSymbolNode.SelectSingleNode("BCGGroup")),
                            OverridingSymbolFontSize = ParseInteger(geoMapSymbolNode.SelectSingleNode("FontSize")),
                            OverridingSymbolStyle = geoMapSymbolNode["SymbolStyle"]?.InnerText
                        };

                        // Assign Applied attributes, with fallback to defaults
                        // Determine the effective (applied) filters for the symbol
                        // Use symbol-specific (overriding) filters if defined; otherwise, use the default filters
                        geoMapSymbol.AppliedSymbolFilters = geoMapSymbol.OverridingSymbolFiltersGroups.Count > 0
                            ? geoMapSymbol.OverridingSymbolFiltersGroups
                            : geoMapObjectType.DefaultSymbolFilters;

                        // Check if AppliedSymbolFilters is null or empty and assign a value of 0 if so.
                        // In CRC, a null filter value indicates "Always Display" and it appears that is what RW ERAM does, too.
                        if (geoMapSymbol.AppliedSymbolFilters == null || !geoMapSymbol.AppliedSymbolFilters.Any())
                        {
                            geoMapSymbol.AppliedSymbolFilters = new List<int> { 0 };
                        }

                        // Determine the effective (applied) style for the symbol
                        // Use the overriding style if defined; otherwise, use the default style
                        geoMapSymbol.AppliedSymbolStyle = geoMapSymbol.OverridingSymbolStyle ?? geoMapObjectType.DefaultSymbolStyle;

                        // Determine the effective (applied) Brightness Control Group for the symbol
                        geoMapSymbol.AppliedSymbolBcgGroup = geoMapSymbol.OverridingSymbolBcgGroup ?? geoMapObjectType.DefaultSymbolBcgGroup;

                        // Determine the effective (applied) font size for the symbol
                        geoMapSymbol.AppliedSymbolFontSize = geoMapSymbol.OverridingSymbolFontSize ?? geoMapObjectType.DefaultSymbolFontSize;

                        // Ensure AppliedSymbolStyle is assigned, falling back to the DefaultSymbolStyle
                        geoMapSymbol.AppliedSymbolStyle = geoMapSymbol.OverridingSymbolStyle ?? geoMapObjectType.DefaultSymbolStyle;

                        // Parse GeoMapText within GeoMapSymbol
                        // Select all GeoMapText nodes within the current GeoMapSymbol node
                        var geoMapTextNodes = geoMapSymbolNode.SelectNodes("GeoMapText");
                        foreach (XmlNode geoMapTextNode in geoMapTextNodes)
                        {
                            // Create a new GeoMapText instance and populate its properties
                            var geoMapText = new GeoMapText
                            {
                                OverridingTextFilterGroups = ParseFilterGroups(geoMapTextNode.SelectSingleNode("GeoTextFilters")),
                                OverridingTextFontSize = ParseInteger(geoMapTextNode.SelectSingleNode("FontSize")),
                                OverridingTextUnderline = ParseBoolean(geoMapTextNode.SelectSingleNode("Underline")),
                                OverridingTextDisplaySetting = ParseBoolean(geoMapTextNode.SelectSingleNode("DisplaySetting")),
                                OverridingTextXPixelOffset = ParseInteger(geoMapTextNode.SelectSingleNode("XPixelOffset")),
                                OverridingTextYPixelOffset = ParseInteger(geoMapTextNode.SelectSingleNode("YPixelOffset"))
                            };

                            // Determine the effective (applied) filters for the text
                            // Use text-specific (overriding) filters if defined; otherwise, use the default filters
                            geoMapText.AppliedTextFilters = geoMapText.OverridingTextFilterGroups.Count > 0
                                ? geoMapText.OverridingTextFilterGroups
                                : geoMapObjectType.DefaultTextFilters;

                            // Check if AppliedTextFilters is null or empty and assign a value of 0 if so.
                            // In CRC, a null filter value indicates "Always Display" and it appears that is what RW ERAM does, too.
                            if (geoMapText.AppliedTextFilters == null || !geoMapText.AppliedTextFilters.Any())
                            {
                                geoMapText.AppliedTextFilters = new List<int> { 0 };
                            }

                            // Determine the effective (applied) Brightness Control Group for the text
                            // Use the overriding BCG group if defined; otherwise, use the default BCG group
                            geoMapText.AppliedTextBcgGroup = geoMapText.OverridingTextBcgGroup ?? geoMapObjectType.DefaultTextBcgGroup;

                            // Determine the effective (applied) font size for the text
                            // Use the overriding font size if defined; otherwise, use the default font size
                            geoMapText.AppliedTextFontSize = geoMapText.OverridingTextFontSize ?? geoMapObjectType.DefaultTextFontSize;

                            // Determine the effective (applied) underline setting for the text
                            // Use the overriding underline setting if defined; otherwise, use the default underline setting
                            geoMapText.AppliedTextUnderline = geoMapText.OverridingTextUnderline ?? geoMapObjectType.DefaultTextUnderline;

                            // Determine the effective (applied) display setting for the text
                            // Use the overriding display setting if defined; otherwise, use the default display setting
                            geoMapText.AppliedTextDisplaySetting = geoMapText.OverridingTextDisplaySetting ?? geoMapObjectType.DefaultTextDisplaySetting;

                            // Determine the effective (applied) horizontal pixel offset for the text
                            // Use the overriding X offset if defined; otherwise, use the default X offset
                            geoMapText.AppliedTextXPixelOffset = geoMapText.OverridingTextXPixelOffset ?? geoMapObjectType.DefaultTextXPixelOffset;

                            // Determine the effective (applied) vertical pixel offset for the text
                            // Use the overriding Y offset if defined; otherwise, use the default Y offset
                            geoMapText.AppliedTextYPixelOffset = geoMapText.OverridingTextYPixelOffset ?? geoMapObjectType.DefaultTextYPixelOffset;

                            // Parse individual text lines within the current GeoMapText node
                            var textLineNodes = geoMapTextNode.SelectNodes("GeoTextStrings/TextLine");
                            foreach (XmlNode textLineNode in textLineNodes)
                            {
                                // Add each text line's inner content to the GeoMapText object's TextLines collection
                                geoMapText.TextLines.Add(textLineNode.InnerText);
                            }

                            // Add the parsed GeoMapText object to the current GeoMapSymbol's TextObjects collection
                            geoMapSymbol.TextObjects.Add(geoMapText);
                        }

                        // Add the parsed GeoMapSymbol object to the current GeoMapObjectType's Symbols collection
                        geoMapObjectType.Symbols.Add(geoMapSymbol);
                    }

                    // Add the parsed GeoMapObjectType to the current GeoMapRecord's ObjectTypes collection
                    geoMapRecord.ObjectTypes.Add(geoMapObjectType);
                }

                // Add the fully parsed GeoMapRecord to the geoMapRecords collection
                geoMapRecords.Add(geoMapRecord);
            }

            // Return the parsed results:
            // - geoMapRecords: List of GeoMapRecord objects containing all parsed data
            // - bcgMenuToGeomapIds: Mapping of BCG menu names to their associated GeoMap IDs
            // - filterMenuToGeomapIds: Mapping of filter menu names to their associated GeoMap IDs
            return (geoMapRecords, bcgMenuToGeomapIds, filterMenuToGeomapIds);
        }

        // Parses a collection of filter group IDs from a given XmlNode
        private static List<int> ParseFilterGroups(XmlNode? filterGroupNode)
        {
            var filterGroups = new List<int>();
            if (filterGroupNode != null)
            {
                // Iterate through all child nodes named "FilterGroup"
                foreach (XmlNode filterNode in filterGroupNode.SelectNodes("FilterGroup"))
                {
                    // Attempt to parse the inner text of each "FilterGroup" node as an integer
                    if (int.TryParse(filterNode.InnerText, out int filterValue))
                    {
                        filterGroups.Add(filterValue); // Add valid integers to the list
                    }
                }
            }
            // Return the list of parsed filter group IDs (empty if none are valid)
            return filterGroups;
        }

        // Parses the inner text of a given XmlNode as a string
        private static string? ParseString(XmlNode? node)
        {
            // Return the inner text if the node is not null, otherwise return null
            return node?.InnerText;
        }

        // Parses the inner text of a given XmlNode as an integer
        private static int? ParseInteger(XmlNode? node)
        {
            // Attempt to parse the inner text as an integer, return the value if successful, otherwise null
            return node != null && int.TryParse(node.InnerText, out int value) ? value : null;
        }

        // Parses the inner text of a given XmlNode as a boolean
        private static bool? ParseBoolean(XmlNode? node)
        {
            // Attempt to parse the inner text as a boolean, return the value if successful, otherwise null
            return node != null && bool.TryParse(node.InnerText, out bool value) ? value : null;
        }
    }
}
