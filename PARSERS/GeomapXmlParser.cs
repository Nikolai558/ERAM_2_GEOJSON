using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ERAM_2_GEOJSON.Models;

namespace ERAM_2_GEOJSON.Parsers
{
    public static class GeomapXmlParser
    {
        public static List<GeoMapRecord> Parse(string filePath)
        {
            var geoMapRecords = new List<GeoMapRecord>();
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            var geoMapRecordNodes = xmlDoc.SelectNodes("//GeoMapRecord");

            foreach (XmlNode geoMapRecordNode in geoMapRecordNodes)
            {
                var geoMapRecord = new GeoMapRecord
                {
                    GeomapId = geoMapRecordNode["GeomapId"]?.InnerText ?? throw new Exception("GeomapId is required."),
                    LabelLine1 = geoMapRecordNode["LabelLine1"]?.InnerText ?? "LL1",
                    LabelLine2 = geoMapRecordNode["LabelLine2"]?.InnerText ?? "LL2",
                };

                var geoMapObjectTypeNodes = geoMapRecordNode.SelectNodes("GeoMapObjectType");
                foreach (XmlNode geoMapObjectTypeNode in geoMapObjectTypeNodes)
                {
                    var geoMapObjectType = new GeoMapObjectType
                    {
                        MapObjectType = geoMapObjectTypeNode["MapObjectType"]?.InnerText ?? throw new Exception("MapObjectType is required."),
                        MapGroupId = geoMapObjectTypeNode["MapGroupId"]?.InnerText ?? throw new Exception("MapGroupId is required."),
                        DefaultLineFilters = ParseFilterGroups(geoMapObjectTypeNode.SelectSingleNode("DefaultLineProperties/GeoLineFilters")),
                        DefaultSymbolFilters = ParseFilterGroups(geoMapObjectTypeNode.SelectSingleNode("DefaultSymbolProperties/GeoSymbolFilters")),
                        DefaultTextFilters = ParseFilterGroups(geoMapObjectTypeNode.SelectSingleNode("TextDefaultProperties/GeoTextFilters")),
                        DefaultLineStyle = ParseString(geoMapObjectTypeNode.SelectSingleNode("DefaultLineProperties/LineStyle")),
                        DefaultLineBcgGroup = ParseInteger(geoMapObjectTypeNode.SelectSingleNode("DefaultLineProperties/BCGGroup")),
                        DefaultLineThickness = ParseInteger(geoMapObjectTypeNode.SelectSingleNode("DefaultLineProperties/Thickness")),
                        DefaultSymbolBcgGroup = ParseInteger(geoMapObjectTypeNode.SelectSingleNode("DefaultSymbolProperties/BCGGroup")),
                        DefaultSymbolFontSize = ParseInteger(geoMapObjectTypeNode.SelectSingleNode("DefaultSymbolProperties/FontSize")),
                        DefaultTextBcgGroup = ParseInteger(geoMapObjectTypeNode.SelectSingleNode("TextDefaultProperties/BCGGroup")),
                        DefaultTextFontSize = ParseInteger(geoMapObjectTypeNode.SelectSingleNode("TextDefaultProperties/FontSize")),
                        DefaultTextUnderline = ParseBoolean(geoMapObjectTypeNode.SelectSingleNode("TextDefaultProperties/Underline")),
                        DefaultTextDisplaySetting = ParseBoolean(geoMapObjectTypeNode.SelectSingleNode("TextDefaultProperties/DisplaySetting")),
                        DefaultTextXPixelOffset = ParseInteger(geoMapObjectTypeNode.SelectSingleNode("TextDefaultProperties/XPixelOffset")),
                        DefaultTextYPixelOffset = ParseInteger(geoMapObjectTypeNode.SelectSingleNode("TextDefaultProperties/YPixelOffset"))
                    };

                    // Parse GeoMapLine objects
                    var geoMapLineNodes = geoMapObjectTypeNode.SelectNodes("GeoMapLine");
                    foreach (XmlNode geoMapLineNode in geoMapLineNodes)
                    {
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

                        geoMapLine.AppliedLineFilters = geoMapLine.OverridingLineFilterGroups.Count > 0
                            ? geoMapLine.OverridingLineFilterGroups
                            : geoMapObjectType.DefaultLineFilters;

                        geoMapLine.AppliedLineStyle = geoMapLine.OverridingLineStyle ?? geoMapObjectType.DefaultLineStyle;
                        geoMapLine.AppliedLineBcgGroup = geoMapLine.OverridingLineBcgGroup ?? geoMapObjectType.DefaultLineBcgGroup;
                        geoMapLine.AppliedLineThickness = geoMapLine.OverridingLineThickness ?? geoMapObjectType.DefaultLineThickness;

                        geoMapObjectType.Lines.Add(geoMapLine);
                    }

                    // Parse GeoMapSymbol objects
                    var geoMapSymbolNodes = geoMapObjectTypeNode.SelectNodes("GeoMapSymbol");
                    foreach (XmlNode geoMapSymbolNode in geoMapSymbolNodes)
                    {
                        var geoMapSymbol = new GeoMapSymbol
                        {
                            SymbolId = geoMapSymbolNode["SymbolId"]?.InnerText ?? throw new Exception("SymbolId is required."),
                            Latitude = geoMapSymbolNode["Latitude"]?.InnerText ?? throw new Exception("Latitude is required."),
                            Longitude = geoMapSymbolNode["Longitude"]?.InnerText ?? throw new Exception("Longitude is required."),
                            OverridingSymbolFiltersGroups = ParseFilterGroups(geoMapSymbolNode.SelectSingleNode("GeoSymbolFilters")),
                            OverridingSymbolBcgGroup = ParseInteger(geoMapSymbolNode.SelectSingleNode("BCGGroup")),
                            OverridingSymbolFontSize = ParseInteger(geoMapSymbolNode.SelectSingleNode("FontSize")),

                            // Add OverridingSymbolStyle logic
                            OverridingSymbolStyle = geoMapSymbolNode["SymbolStyle"]?.InnerText
                        };

                        // Assign Applied attributes, with fallback to defaults
                        geoMapSymbol.AppliedSymbolFilters = geoMapSymbol.OverridingSymbolFiltersGroups.Count > 0
                            ? geoMapSymbol.OverridingSymbolFiltersGroups
                            : geoMapObjectType.DefaultSymbolFilters;

                        geoMapSymbol.AppliedSymbolBcgGroup = geoMapSymbol.OverridingSymbolBcgGroup ?? geoMapObjectType.DefaultSymbolBcgGroup;
                        geoMapSymbol.AppliedSymbolFontSize = geoMapSymbol.OverridingSymbolFontSize ?? geoMapObjectType.DefaultSymbolFontSize;

                        // Assign AppliedSymbolStyle with fallback to DefaultSymbolStyle
                        geoMapSymbol.AppliedSymbolStyle = geoMapSymbol.OverridingSymbolStyle ?? geoMapObjectType.DefaultSymbolStyle;

                        // Parse GeoMapText within GeoMapSymbol
                        var geoMapTextNodes = geoMapSymbolNode.SelectNodes("GeoMapText");
                        foreach (XmlNode geoMapTextNode in geoMapTextNodes)
                        {
                            var geoMapText = new GeoMapText
                            {
                                OverridingTextFilterGroups = ParseFilterGroups(geoMapTextNode.SelectSingleNode("GeoTextFilters")),
                                OverridingTextFontSize = ParseInteger(geoMapTextNode.SelectSingleNode("FontSize")),
                                OverridingTextUnderline = ParseBoolean(geoMapTextNode.SelectSingleNode("Underline")),
                                OverridingTextDisplaySetting = ParseBoolean(geoMapTextNode.SelectSingleNode("DisplaySetting")),
                                OverridingTextXPixelOffset = ParseInteger(geoMapTextNode.SelectSingleNode("XPixelOffset")),
                                OverridingTextYPixelOffset = ParseInteger(geoMapTextNode.SelectSingleNode("YPixelOffset"))
                            };

                            geoMapText.AppliedTextFilters = geoMapText.OverridingTextFilterGroups.Count > 0
                                ? geoMapText.OverridingTextFilterGroups
                                : geoMapObjectType.DefaultTextFilters;

                            geoMapText.AppliedTextBcgGroup = geoMapText.OverridingTextBcgGroup ?? geoMapObjectType.DefaultTextBcgGroup;
                            geoMapText.AppliedTextFontSize = geoMapText.OverridingTextFontSize ?? geoMapObjectType.DefaultTextFontSize;
                            geoMapText.AppliedTextUnderline = geoMapText.OverridingTextUnderline ?? geoMapObjectType.DefaultTextUnderline;
                            geoMapText.AppliedTextDisplaySetting = geoMapText.OverridingTextDisplaySetting ?? geoMapObjectType.DefaultTextDisplaySetting;
                            geoMapText.AppliedTextXPixelOffset = geoMapText.OverridingTextXPixelOffset ?? geoMapObjectType.DefaultTextXPixelOffset;
                            geoMapText.AppliedTextYPixelOffset = geoMapText.OverridingTextYPixelOffset ?? geoMapObjectType.DefaultTextYPixelOffset;

                            var textLineNodes = geoMapTextNode.SelectNodes("GeoTextStrings/TextLine");
                            foreach (XmlNode textLineNode in textLineNodes)
                            {
                                geoMapText.TextLines.Add(textLineNode.InnerText);
                            }

                            geoMapSymbol.TextObjects.Add(geoMapText);
                        }

                        geoMapObjectType.Symbols.Add(geoMapSymbol);
                    }

                    geoMapRecord.ObjectTypes.Add(geoMapObjectType);
                }

                geoMapRecords.Add(geoMapRecord);
            }

            return geoMapRecords;
        }

        private static List<int> ParseFilterGroups(XmlNode? filterGroupNode)
        {
            var filterGroups = new List<int>();
            if (filterGroupNode != null)
            {
                foreach (XmlNode filterNode in filterGroupNode.SelectNodes("FilterGroup"))
                {
                    if (int.TryParse(filterNode.InnerText, out int filterValue))
                    {
                        filterGroups.Add(filterValue);
                    }
                }
            }
            return filterGroups;
        }
        private static string? ParseString(XmlNode? node)
        {
            return node?.InnerText;
        }

        private static int? ParseInteger(XmlNode? node)
        {
            return node != null && int.TryParse(node.InnerText, out int value) ? value : null;
        }

        private static bool? ParseBoolean(XmlNode? node)
        {
            return node != null && bool.TryParse(node.InnerText, out bool value) ? value : null;
        }
    }
}
