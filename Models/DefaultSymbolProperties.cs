namespace ERAM_2_GEOJSON.Models
{
    public class DefaultSymbolProperties
    {
        public string SymbolStyle { get; set; } = "default";  // Example: Default symbol style
        public List<string> GeoSymbolFilters { get; set; } = new List<string>();
    }
}
