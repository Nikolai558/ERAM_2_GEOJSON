using System.Collections.Generic;

namespace ERAM_2_GEOJSON.Models
{
    public class GeoMapLine
    {
        public required string LineObjectId { get; set; }
        public required string StartLatitude { get; set; }
        public required string StartLongitude { get; set; }
        public required string EndLatitude { get; set; }
        public required string EndLongitude { get; set; }

        public List<int> OverridingLineFilterGroups { get; set; }
        public List<int> AppliedLineFilters { get; set; }

        public GeoMapLine()
        {
            OverridingLineFilterGroups = new List<int>();
            AppliedLineFilters = new List<int>();
        }
    }
}
