using Newtonsoft.Json;

namespace Models
{
    public class WeatherModel
    {
        [JsonProperty(PropertyName = "number")]
        public int Number;
        [JsonProperty(PropertyName = "name")]
        public string Name;
        [JsonProperty(PropertyName = "temperature")]
        public string Temperature;
        [JsonProperty(PropertyName = "icon")]
        public string Icon;
    }
}
