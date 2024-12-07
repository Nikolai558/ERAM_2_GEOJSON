using System.Collections.Generic;

namespace ERAM_2_GEOJSON.MODELS
{
    public class GeoMapText
    {
        public List<int> OverridingTextFilterGroups { get; set; }
        public List<int> AppliedTextFilters { get; set; }
        public List<string> TextLines { get; set; }

        public GeoMapText()
        {
            OverridingTextFilterGroups = new List<int>();
            AppliedTextFilters = new List<int>();
            TextLines = new List<string>();
        }
    }
}
