using System.Collections.Generic;

namespace ERAM_2_GEOJSON.Models
{
    public class GeoMapSymbol
    {
        public required string SymbolId { get; set; }
        public required string Latitude { get; set; }
        public required string Longitude { get; set; }
        public required List<string> FilterGroups { get; set; } = new List<string>(); // Added FilterGroups
        public List<string>? OveridingFilterGroups { get; set; } // Added OveridingFilterGroups
        public GeoMapText? GeoMapText { get; set; }
    }
}
