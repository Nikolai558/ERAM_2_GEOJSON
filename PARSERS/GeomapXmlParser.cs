using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ERAM_2_GEOJSON.Models;
using ERAM_2_GEOJSON.MODELS;

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
                            OverridingLineFilterGroups = ParseFilterGroups(geoMapLineNode.SelectSingleNode("GeoLineFilters"))
                        };

                        // If the GeoMapLine object has overriding Filter Groups, it will assign those values to this set of coordinates,
                        // otherwise, uses the Default Line Filters from the GeoMapObjectType.
                        geoMapLine.AppliedLineFilters = geoMapLine.OverridingLineFilterGroups.Count > 0
                            ? geoMapLine.OverridingLineFilterGroups
                            : geoMapObjectType.DefaultLineFilters;

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
                            OverridingSymbolFiltersGroups = ParseFilterGroups(geoMapSymbolNode.SelectSingleNode("GeoSymbolFilters"))
                        };

                        // If the GeoMapSymbol object has overriding Filter Groups, it will assign those values to this set of coordinates,
                        // otherwise, uses the Default Symbol Filters from the GeoMapObjectType.
                        geoMapSymbol.AppliedSymbolFilters = geoMapSymbol.OverridingSymbolFiltersGroups.Count > 0
                            ? geoMapSymbol.OverridingSymbolFiltersGroups
                            : geoMapObjectType.DefaultSymbolFilters;


                        // Parse GeoMapText within GeoMapSymbol
                        var geoMapTextNodes = geoMapSymbolNode.SelectNodes("GeoMapText");
                        foreach (XmlNode geoMapTextNode in geoMapTextNodes)
                        {
                            var geoMapText = new GeoMapText
                            {
                                OverridingTextFilterGroups = ParseFilterGroups(geoMapTextNode.SelectSingleNode("GeoTextFilters"))
                            };

                            // If the GeoMapText object has overriding Filter Groups, it will assign those values to this set of coordinates foudn in GeoMapSymbol,
                            // otherwise, uses the Default Text Filters from the GeoMapObjectType.
                            geoMapText.AppliedTextFilters = geoMapText.OverridingTextFilterGroups.Count > 0
                                ? geoMapText.OverridingTextFilterGroups
                                : geoMapObjectType.DefaultTextFilters;

                            // Parse GeoTextStrings inside GeoMapText
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

        // Finds the FilterGroup values for the current Line/Symbol/Symbol>Text object
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
    }
}
