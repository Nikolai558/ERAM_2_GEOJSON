using System.Collections.Generic;

namespace ERAM_2_GEOJSON.Models
{
    public class GeoMapRecord
    {
        public required string GeomapId { get; set; }
        public string LabelLine1 { get; set; }
        public string LabelLine2 { get; set; }
        public required List<GeoMapObjectType> ObjectTypes { get; set; } = new List<GeoMapObjectType>();
    }
}
