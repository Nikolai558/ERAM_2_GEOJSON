using System.Collections.Generic;

namespace ERAM_2_GEOJSON.Models
{
    public class GeoMapRecord
    {
        public required string GeomapId { get; set; }
        public required string BcgMenuName { get; set; }
        public required string FilterMenuName { get; set; }
        public string LabelLine1 { get; set; }
        public string LabelLine2 { get; set; }

        public List<GeoMapObjectType> ObjectTypes { get; set; }

        public GeoMapRecord()
        {
            ObjectTypes = new List<GeoMapObjectType>();
        }
    }
}
