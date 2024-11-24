namespace ERAM_2_GEOJSON.Models
{
    // Represents the default properties for GeoMap symbols
    public class DefaultSymbolProperties
    {
        // Default symbol style, initialized to "default" to prevent null values
        public string SymbolStyle { get; set; } = "default";

        // A list of filter groups for the GeoMap symbols, initialized to an empty list
        public List<string> GeoSymbolFilters { get; set; } = new List<string>();
    }
}
