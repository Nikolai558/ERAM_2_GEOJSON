namespace ERAM_2_GEOJSON.Models
{
    public class GeoMapObjectType
    {
        public required string MapObjectType { get; set; }
        public required string MapGroupId { get; set; }
        public required List<GeoMapLine> Lines { get; set; } = new List<GeoMapLine>();
        public required List<GeoMapSymbol> Symbols { get; set; } = new List<GeoMapSymbol>();

        // Add the default properties
        public DefaultLineProperties? DefaultLineProperties { get; set; }
        public DefaultSymbolProperties? DefaultSymbolProperties { get; set; }
        public DefaultTextProperties? DefaultTextProperties { get; set; }
    }
}
