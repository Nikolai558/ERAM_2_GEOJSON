namespace ERAM_2_GEOJSON.Models
{
    // Represents text associated with a GeoMap symbol
    public class GeoMapText
    {
        // Text content for the GeoMap symbol
        public required string TextLine { get; set; }

        // List of filter groups for the text, initialized to an empty list
        public List<string> FilterGroups { get; set; } = new List<string>();
    }
}
