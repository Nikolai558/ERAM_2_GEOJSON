namespace ERAM_2_GEOJSON.Models
{
    // Represents a GeoMap object type containing lines, symbols, and associated properties
    public class GeoMapObjectType
    {
        public required string MapObjectType { get; set; } // Type of GeoMap object (e.g., approach control)
        public required string MapGroupId { get; set; }    // Identifier for the map group

        // Lists of lines and symbols associated with the GeoMap object type
        public required List<GeoMapLine> Lines { get; set; } = new List<GeoMapLine>();
        public required List<GeoMapSymbol> Symbols { get; set; } = new List<GeoMapSymbol>();

        // Default properties for lines, symbols, and text objects within this GeoMap object type
        public DefaultLineProperties? DefaultLineProperties { get; set; }
        public DefaultSymbolProperties? DefaultSymbolProperties { get; set; }
        public DefaultTextProperties? DefaultTextProperties { get; set; }
    }
}
