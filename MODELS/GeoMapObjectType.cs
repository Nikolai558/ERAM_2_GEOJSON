using System.Collections.Generic;

namespace ERAM_2_GEOJSON.Models
{
    public class GeoMapObjectType
    {
        public required string MapObjectType { get; set; }
        public required string MapGroupId { get; set; }

        public List<GeoMapLine> Lines { get; set; }
        public List<int>? DefaultLineFilters { get; set; }

        public int? DefaultLineBcgGroup { get; set; }
        public int? DefaultLineThickness { get; set; }
        public string? DefaultLineStyle { get; set; }

        public List<GeoMapSymbol> Symbols { get; set; }
        public List<int>? DefaultSymbolFilters { get; set; }

        public int? DefaultSymbolBcgGroup { get; set; }
        public int? DefaultSymbolFontSize { get; set; }
        public string? DefaultSymbolStyle { get; set; }

        public List<int>? DefaultTextFilters { get; set; }
        public int? DefaultTextBcgGroup { get; set; }
        public int? DefaultTextFontSize { get; set; }

        public bool? DefaultTextUnderline { get; set; }
        public bool? DefaultTextDisplaySetting { get; set; }

        public int? DefaultTextXPixelOffset { get; set; }
        public int? DefaultTextYPixelOffset { get; set; }

        public bool HasLine => Lines?.Count > 0;
        public bool HasSymbol => Symbols?.Count > 0;
        public bool HasText => Symbols?.Any(symbol => symbol.TextObjects?.Count > 0) ?? false;

        public GeoMapObjectType()
        {
            Lines = new List<GeoMapLine>();
            Symbols = new List<GeoMapSymbol>();
            DefaultLineFilters = new List<int>();
            DefaultSymbolFilters = new List<int>();
            DefaultTextFilters = new List<int>();
        }
    }
}
