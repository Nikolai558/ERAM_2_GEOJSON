namespace ERAM_2_GEOJSON.Models
{
    // Represents a GeoMap record, containing information about the map, labels, and object types
    public class GeoMapRecord
    {
        public required string GeomapId { get; set; }  // Unique identifier for the GeoMap
        public string? LabelLine1 { get; set; }        // First label line (optional)
        public string? LabelLine2 { get; set; }        // Second label line (optional)
        public required List<GeoMapObjectType> ObjectTypes { get; set; } = new List<GeoMapObjectType>(); // List of object types in the GeoMap
    }
}
