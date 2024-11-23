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
                    ObjectTypes = new List<GeoMapObjectType>() // Initialize with an empty list to fulfill the 'required' property requirement
                };

                foreach (XElement objectTypeElement in geoMapRecordElement.Elements("GeoMapObjectType"))
                {
                    GeoMapObjectType mapObjectType = new GeoMapObjectType
                    {
                        MapObjectType = objectTypeElement.Element("MapObjectType")?.Value ?? "Unknown",
                        MapGroupId = objectTypeElement.Element("MapGroupId")?.Value ?? "0",
                        Lines = new List<GeoMapLine>(),     // Initialize as empty list for required property
                        Symbols = new List<GeoMapSymbol>()  // Initialize as empty list for required property
                    };

                    // Handling DefaultLineProperties, DefaultSymbolProperties, DefaultTextProperties
                    mapObjectType.DefaultLineProperties = ParseDefaultLineProperties(objectTypeElement.Element("DefaultLineProperties"));
                    mapObjectType.DefaultSymbolProperties = ParseDefaultSymbolProperties(objectTypeElement.Element("DefaultSymbolProperties"));
                    mapObjectType.DefaultTextProperties = ParseDefaultTextProperties(objectTypeElement.Element("TextDefaultProperties"));

                    foreach (XElement lineElement in objectTypeElement.Elements("GeoMapLine"))
                    {
                        GeoMapLine mapLine = new GeoMapLine
                        {
                            LineObjectId = lineElement.Element("LineObjectId")?.Value ?? "Unknown",
                            StartLatitude = lineElement.Element("StartLatitude")?.Value ?? "N/A",
                            StartLongitude = lineElement.Element("StartLongitude")?.Value ?? "N/A",
                            EndLatitude = lineElement.Element("EndLatitude")?.Value ?? "N/A",
                            EndLongitude = lineElement.Element("EndLongitude")?.Value ?? "N/A",
                            FilterGroups = new List<string>()  // Initialize as empty list for required property
                        };

                        // Determine the appropriate FilterGroup for the line
                        var lineFilters = lineElement.Elements("GeoLineFilters").Elements("FilterGroup");
                        if (!lineFilters.Any() && mapObjectType.DefaultLineProperties != null)
                        {
                            lineFilters = mapObjectType.DefaultLineProperties.GeoLineFilters.Select(x => new XElement("FilterGroup", x));
                        }

                        if (lineFilters != null)
                        {
                            foreach (var filter in lineFilters)
                            {
                                mapLine.FilterGroups.Add(filter.Value);
                            }
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
                            FilterGroups = new List<string>(),   // Initialize as empty list for required property
                            GeoMapText = symbolElement.Element("GeoMapText") != null ? new GeoMapText
                            {
                                TextLine = symbolElement.Element("GeoMapText")?.Element("TextLine")?.Value ?? "N/A",
                                FilterGroups = new List<string>() // Initialize as empty list for required property
                            } : null
                        };

                        // Determine the appropriate FilterGroup for the symbol
                        var symbolFilters = symbolElement.Elements("GeoSymbolFilters").Elements("FilterGroup");

                        // Check to see if the symbol does NOT have overiding filters
                        if (symbolFilters.Count() == 0 && mapObjectType.DefaultSymbolProperties != null)
                        {
                            foreach (var filter in mapObjectType.DefaultSymbolProperties.GeoSymbolFilters.Select(x => new XElement("FilterGroup", x)))
                            {
                                geoMapSymbol.FilterGroups.Add(filter.Value);
                        }
                        }

                        // Check to see if it does HAVE overiding filters AND that it has default symbol properties. 
                        if (symbolFilters.Count() != 0)
                        {
                            geoMapSymbol.OveridingFilterGroups = new List<string>(); // Initialize as empty list for required property
                            foreach (var filter in symbolFilters)
                            {
                                geoMapSymbol.OveridingFilterGroups.Add(filter.Value);
                            }
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
