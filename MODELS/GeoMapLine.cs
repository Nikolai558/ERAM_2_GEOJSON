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

        // Existing filter logic
        public List<int> OverridingLineFilterGroups { get; set; }
        public List<int> AppliedLineFilters { get; set; }

        // New attributes
        public int? OverridingLineBcgGroup { get; set; }
        public int? AppliedLineBcgGroup { get; set; }

        public int? OverridingLineThickness { get; set; }
        public int? AppliedLineThickness { get; set; }

        public string? OverridingLineStyle { get; set; }
        public string? AppliedLineStyle { get; set; }

        public GeoMapLine()
        {
            OverridingLineFilterGroups = new List<int>();
            AppliedLineFilters = new List<int>();
        }
    }
}
