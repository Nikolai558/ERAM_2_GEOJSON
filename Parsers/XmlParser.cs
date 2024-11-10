using System.Collections.Generic;
using System.Xml.Linq;
using ERAM_2_GEOJSON.Models;

namespace ERAM_2_GEOJSON.Parsers
{
    public class XmlParser
    {
        public List<GeoMapRecord> Parse(string filePath)
        {
            List<GeoMapRecord> records = new List<GeoMapRecord>();
            XDocument xmlDoc = XDocument.Load(filePath);

            foreach (var geoMapRecord in xmlDoc.Descendants("GeoMapRecord"))
            {
                GeoMapRecord record = new GeoMapRecord
                {
                    GeomapId = geoMapRecord.Element("GeomapId")?.Value ?? "Unknown",
                    LabelLine1 = geoMapRecord.Element("LabelLine1")?.Value ?? "N/A",
                    LabelLine2 = geoMapRecord.Element("LabelLine2")?.Value ?? "N/A"
                };

                foreach (var objectType in geoMapRecord.Elements("GeoMapObjectType"))
                {
                    GeoMapObjectType mapObjectType = new GeoMapObjectType
                    {
                        MapObjectType = objectType.Element("MapObjectType")?.Value ?? "Unknown",
                        MapGroupId = objectType.Element("MapGroupId")?.Value ?? "Unknown"
                    };

                    // Parse GeoMapLine elements within GeoMapObjectType
                    foreach (var line in objectType.Elements("GeoMapLine"))
                    {
                        GeoMapLine mapLine = new GeoMapLine
                        {
                            LineObjectId = line.Element("LineObjectId")?.Value ?? "Unknown",
                            StartLatitude = line.Element("StartLatitude")?.Value ?? "N/A",
                            StartLongitude = line.Element("StartLongitude")?.Value ?? "N/A",
                            EndLatitude = line.Element("EndLatitude")?.Value ?? "N/A",
                            EndLongitude = line.Element("EndLongitude")?.Value ?? "N/A"
                        };

                        // Determine the appropriate FilterGroup for the line
                        var lineFilters = line.Elements("GeoLineFilters").Elements("FilterGroup");
                        if (!lineFilters.Any())
                        {
                            lineFilters = objectType.Element("DefaultLineProperties")?.Elements("GeoLineFilters")?.Elements("FilterGroup");
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

                    // Parse GeoMapSymbol elements within GeoMapObjectType
                    foreach (var symbol in objectType.Elements("GeoMapSymbol"))
                    {
                        GeoMapSymbol geoMapSymbol = new GeoMapSymbol
                        {
                            SymbolId = symbol.Element("SymbolId")?.Value ?? "Unknown",
                            Latitude = symbol.Element("Latitude")?.Value ?? "N/A",
                            Longitude = symbol.Element("Longitude")?.Value ?? "N/A"
                        };

                        // Determine the appropriate FilterGroup for the symbol
                        var symbolFilters = symbol.Elements("GeoSymbolFilters").Elements("FilterGroup");
                        if (!symbolFilters.Any())
                        {
                            symbolFilters = objectType.Element("DefaultSymbolProperties")?.Elements("GeoSymbolFilters")?.Elements("FilterGroup");
                        }

                        if (symbolFilters != null)
                        {
                            foreach (var filter in symbolFilters)
                            {
                                geoMapSymbol.FilterGroups.Add(filter.Value);
                            }
                        }

                        // Parse GeoMapText if present within GeoMapSymbol
                        var geoMapTextElement = symbol.Element("GeoMapText");
                        if (geoMapTextElement != null)
                        {
                            GeoMapText geoMapText = new GeoMapText
                            {
                                TextLine = geoMapTextElement.Element("GeoTextStrings")?.Element("TextLine")?.Value ?? "N/A"
                            };

                            // Determine the appropriate FilterGroup for the text
                            var textFilters = geoMapTextElement.Elements("GeoTextFilters").Elements("FilterGroup");
                            if (!textFilters.Any())
                            {
                                textFilters = objectType.Element("TextDefaultProperties")?.Elements("GeoTextFilters")?.Elements("FilterGroup");
                            }

                            if (textFilters != null)
                            {
                                foreach (var filter in textFilters)
                                {
                                    geoMapText.FilterGroups.Add(filter.Value);
                                }
                            }

                            geoMapSymbol.GeoMapText = geoMapText;
                        }

                        mapObjectType.Symbols.Add(geoMapSymbol);
                    }

                    record.ObjectTypes.Add(mapObjectType);
                }

                records.Add(record);
            }

            return records;
        }
    }
}
