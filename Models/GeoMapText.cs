using System.Collections.Generic;

namespace ERAM_2_GEOJSON.Models
{
    public class GeoMapText
    {
        public required string TextLine { get; set; }
        public required List<string> FilterGroups { get; set; } = new List<string>(); // Added FilterGroups
    }
}
