using ERAM_2_GEOJSON.Models;
using ERAM_2_GEOJSON.MODELS;
using System.Collections.Generic;

namespace ERAM_2_GEOJSON.Models
{
    public class GeoMapObjectType
    {
        // Required properties
        public required string MapObjectType { get; set; }
        public required string MapGroupId { get; set; }

        // Line data
        public List<GeoMapLine> Lines { get; set; }
        public List<int>? DefaultLineFilters { get; set; }

        // Symbol data
        public List<GeoMapSymbol> Symbols { get; set; }
        public List<int>? DefaultSymbolFilters { get; set; }

        // Symbol > Text data
        public List<int>? DefaultTextFilters { get; set; }

        // Boolean properties to indicate data presence
        public bool HasLine => Lines?.Count > 0;
        public bool HasSymbol => Symbols?.Count > 0;
        public bool HasText
        {
            get
            {
                if (Symbols.Count > 0)
                {
                    foreach (var symbol in Symbols)
                    {
                        if (symbol.TextObjects?.Count > 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        // Constructor
        public GeoMapObjectType()
        {
            Lines = new List<GeoMapLine>();
            DefaultLineFilters = new List<int>();
            DefaultSymbolFilters = new List<int>();
            DefaultTextFilters = new List<int>();
            Symbols = new List<GeoMapSymbol>();
        }
    }
}
