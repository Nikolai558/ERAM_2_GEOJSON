using System.Collections.Generic;
using System.Xml.Linq;
using ERAM_2_GEOJSON.Models;

namespace ERAM_2_GEOJSON.Parsers
{
    public class XmlParser
    {
        public List<GeoMapRecord> Parse(string xmlFilePath)
        {
            XDocument xmlDoc = XDocument.Load(xmlFilePath);
            List<GeoMapRecord> geoMapRecords = new List<GeoMapRecord>();

            foreach (XElement geoMapRecordElement in xmlDoc.Descendants("GeoMapRecord"))
            {
                GeoMapRecord record = new GeoMapRecord
                {
                    GeomapId = geoMapRecordElement.Element("GeomapId")?.Value ?? "Unknown",
                    LabelLine1 = geoMapRecordElement.Element("LabelLine1")?.Value ?? "N/A",
                    LabelLine2 = geoMapRecordElement.Element("LabelLine2")?.Value ?? "N/A",
                    ObjectTypes = new List<GeoMapObjectType>()
                };

                foreach (XElement objectTypeElement in geoMapRecordElement.Elements("GeoMapObjectType"))
                {
                    GeoMapObjectType mapObjectType = new GeoMapObjectType
                    {
                        MapObjectType = objectTypeElement.Element("MapObjectType")?.Value ?? "Unknown",
                        MapGroupId = objectTypeElement.Element("MapGroupId")?.Value ?? "0",
                        Lines = new List<GeoMapLine>(),
                        Symbols = new List<GeoMapSymbol>()
                    };

                    // Handling DefaultLineProperties, DefaultSymbolProperties, DefaultTextProperties
                    mapObjectType.DefaultLineProperties = ParseDefaultLineProperties(objectTypeElement.Element("DefaultLineProperties"));
                    mapObjectType.DefaultSymbolProperties = ParseDefaultSymbolProperties(objectTypeElement.Element("DefaultSymbolProperties"));
                    mapObjectType.DefaultTextProperties = ParseDefaultTextProperties(objectTypeElement.Element("DefaultTextProperties"));

                    foreach (XElement lineElement in objectTypeElement.Elements("GeoMapLine"))
                    {
                        GeoMapLine mapLine = new GeoMapLine
                        {
                            LineObjectId = lineElement.Element("LineObjectId")?.Value ?? "Unknown",
                            StartLatitude = lineElement.Element("StartLatitude")?.Value ?? "N/A",
                            StartLongitude = lineElement.Element("StartLongitude")?.Value ?? "N/A",
                            EndLatitude = lineElement.Element("EndLatitude")?.Value ?? "N/A",
                            EndLongitude = lineElement.Element("EndLongitude")?.Value ?? "N/A",
                            FilterGroups = new List<string>()
                        };

                        // Determine if overriding filter groups are defined, otherwise use default
                        var lineFilters = lineElement.Elements("GeoLineFilters").Elements("FilterGroup");
                        if (lineFilters.Any())
                        {
                            mapLine.OverridingFilterGroups = new List<string>();
                            foreach (var filter in lineFilters)
                            {
                                mapLine.OverridingFilterGroups.Add(filter.Value);
                            }
                        }
                        else if (mapObjectType.DefaultLineProperties != null)
                        {
                            mapLine.FilterGroups = mapObjectType.DefaultLineProperties.GeoLineFilters;
                        }

                        mapObjectType.Lines.Add(mapLine);
                    }

                    foreach (XElement symbolElement in objectTypeElement.Elements("GeoMapSymbol"))
                    {
                        GeoMapSymbol geoMapSymbol = new GeoMapSymbol
                        {
                            SymbolId = symbolElement.Element("SymbolId")?.Value ?? "Unknown",
                            Latitude = symbolElement.Element("Latitude")?.Value ?? "N/A",
                            Longitude = symbolElement.Element("Longitude")?.Value ?? "N/A",
                            FilterGroups = new List<string>(),
                            GeoMapText = symbolElement.Element("GeoMapText") != null ? new GeoMapText
                            {
                                TextLine = symbolElement.Element("GeoMapText")?.Element("TextLine")?.Value ?? "N/A",
                                FilterGroups = new List<string>()
                            } : null
                        };

                        // Determine the appropriate FilterGroup for the symbol
                        var symbolFilters = symbolElement.Elements("GeoSymbolFilters").Elements("FilterGroup");

                        if (symbolFilters.Any())
                        {
                            geoMapSymbol.OverridingFilterGroups = new List<string>();
                            foreach (var filter in symbolFilters)
                            {
                                geoMapSymbol.OverridingFilterGroups.Add(filter.Value);
                            }
                        }
                        else if (mapObjectType.DefaultSymbolProperties != null)
                        {
                            geoMapSymbol.FilterGroups = mapObjectType.DefaultSymbolProperties.GeoSymbolFilters;
                        }

                        mapObjectType.Symbols.Add(geoMapSymbol);
                    }

                    record.ObjectTypes.Add(mapObjectType);
                }

                geoMapRecords.Add(record);
            }

            return geoMapRecords;
        }

        // Helper Methods to Parse Default Properties
        private DefaultLineProperties? ParseDefaultLineProperties(XElement? element)
        {
            if (element == null) return null;

            DefaultLineProperties defaultProperties = new DefaultLineProperties
            {
                GeoLineFilters = element.Elements("GeoLineFilters").Elements("FilterGroup").Select(e => e.Value).ToList()
            };

            return defaultProperties;
        }

        private DefaultSymbolProperties? ParseDefaultSymbolProperties(XElement? element)
        {
            if (element == null) return null;

            DefaultSymbolProperties defaultProperties = new DefaultSymbolProperties
            {
                SymbolStyle = element.Element("SymbolStyle")?.Value ?? "default",
                GeoSymbolFilters = element.Elements("GeoSymbolFilters").Elements("FilterGroup").Select(e => e.Value).ToList()
            };

            return defaultProperties;
        }

        private DefaultTextProperties? ParseDefaultTextProperties(XElement? element)
        {
            if (element == null) return null;

            DefaultTextProperties defaultProperties = new DefaultTextProperties
            {
                GeoTextFilters = element.Elements("GeoTextFilters").Elements("FilterGroup").Select(e => e.Value).ToList()
            };

            return defaultProperties;
        }
    }
}
