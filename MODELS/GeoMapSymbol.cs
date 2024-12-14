using System.Collections.Generic;

namespace ERAM_2_GEOJSON.Models
{
    public class GeoMapSymbol
    {
        public required string SymbolId { get; set; }
        public required string Latitude { get; set; }
        public required string Longitude { get; set; }

        public List<int> OverridingSymbolFiltersGroups { get; set; }
        public List<int> AppliedSymbolFilters { get; set; }

        public int? OverridingSymbolBcgGroup { get; set; }
        public int? AppliedSymbolBcgGroup { get; set; }

        public int? OverridingSymbolFontSize { get; set; }
        public int? AppliedSymbolFontSize { get; set; }

        public string? OverridingSymbolStyle { get; set; } // Add this property
        public string? AppliedSymbolStyle { get; set; }   // Add this property

        public List<GeoMapText> TextObjects { get; set; }

        public GeoMapSymbol()
        {
            OverridingSymbolFiltersGroups = new List<int>();
            AppliedSymbolFilters = new List<int>();
            TextObjects = new List<GeoMapText>();
        }
    }
}
