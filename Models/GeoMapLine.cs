namespace ERAM_2_GEOJSON.Models
{
    public class GeoMapLine
    {
        public required string LineObjectId { get; set; }
        public required string StartLatitude { get; set; }
        public required string StartLongitude { get; set; }
        public required string EndLatitude { get; set; }
        public required string EndLongitude { get; set; }
        public required List<string> FilterGroups { get; set; } = new List<string>(); // Added FilterGroups
    }
}
