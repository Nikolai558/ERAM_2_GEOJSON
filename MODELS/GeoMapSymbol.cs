using System.Collections.Generic;

namespace ERAM_2_GEOJSON.MODELS
{
    public class GeoMapSymbol
    {
        public required string SymbolId { get; set; }
        public required string Latitude { get; set; }
        public required string Longitude { get; set; }
        public List<int> OverridingSymbolFiltersGroups { get; set; }
        public List<int> AppliedSymbolFilters { get; set; }
        public List<GeoMapText> TextObjects { get; set; }

        public GeoMapSymbol()
        {
            OverridingSymbolFiltersGroups = new List<int>();
            AppliedSymbolFilters = new List<int>();
            TextObjects = new List<GeoMapText>();
        }
    }
}
