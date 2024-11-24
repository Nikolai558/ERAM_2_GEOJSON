namespace ERAM_2_GEOJSON.Models
{
    // Represents a GeoMap line with various properties, including coordinates and filter groups
    public class GeoMapLine
    {
        // Unique identifier for the line
        public required string LineObjectId { get; set; }

        // Latitude and longitude for the start point
        public required string StartLatitude { get; set; }
        public required string StartLongitude { get; set; }

        // Latitude and longitude for the end point
        public required string EndLatitude { get; set; }
        public required string EndLongitude { get; set; }

        // Default filter groups applied to the line
        public List<string> FilterGroups { get; set; } = new List<string>();

        // Overriding filter groups if they are defined explicitly for the line
        public List<string>? OverridingFilterGroups { get; set; }
    }
}
