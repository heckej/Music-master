using Newtonsoft.Json;

namespace Crunch.NET.Request.Type
{
    public class GeolocationSpeed
    {
        [JsonProperty("speedInMetersPerSecond")]
        public double? Speed { get; set; }
        [JsonProperty("accuracyInMetresPerSecond")]
        public double? Accuracy { get; set; }
    }
}