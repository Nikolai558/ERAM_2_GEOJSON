using System.Collections.Generic;

namespace ERAM_2_GEOJSON.Models
{
    public class GeoMapText
    {
        public List<int> OverridingTextFilterGroups { get; set; }
        public List<int> AppliedTextFilters { get; set; }

        public List<string> TextLines { get; set; }

        // New attributes
        public int? OverridingTextBcgGroup { get; set; }
        public int? AppliedTextBcgGroup { get; set; }

        public bool? OverridingTextDisplaySetting { get; set; }
        public bool? AppliedTextDisplaySetting { get; set; }

        public int? OverridingTextFontSize { get; set; }
        public int? AppliedTextFontSize { get; set; }

        public bool? OverridingTextUnderline { get; set; }
        public bool? AppliedTextUnderline { get; set; }

        public int? OverridingTextXPixelOffset { get; set; }
        public int? AppliedTextXPixelOffset { get; set; }

        public int? OverridingTextYPixelOffset { get; set; }
        public int? AppliedTextYPixelOffset { get; set; }

        public GeoMapText()
        {
            OverridingTextFilterGroups = new List<int>();
            AppliedTextFilters = new List<int>();
            TextLines = new List<string>();
        }
    }
}
