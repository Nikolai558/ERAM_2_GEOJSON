namespace ERAM_2_GEOJSON.Models
{
    // Represents a symbol on a GeoMap with relevant properties such as coordinates, filters, and text
    public class GeoMapSymbol
    {
        // Unique identifier for the symbol
        public required string SymbolId { get; set; }

        // Latitude coordinate of the symbol
        public required string Latitude { get; set; }

        // Longitude coordinate of the symbol
        public required string Longitude { get; set; }

        // List of filter groups for the symbol (default properties)
        public List<string> FilterGroups { get; set; } = new List<string>();

        // List of overriding filter groups for the symbol
        public List<string>? OverridingFilterGroups { get; set; }

        // Represents optional text associated with the symbol
        public GeoMapText? GeoMapText { get; set; }
    }
}
