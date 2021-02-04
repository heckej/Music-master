using Crunch.NET.Request.Type;
using Newtonsoft.Json;

namespace Crunch.NET.Request
{
    public class Context
    {
        [JsonProperty("System")]
        public AlexaSystem System { get; set; }

        [JsonProperty("AudioPlayer")]
        public PlaybackState AudioPlayer { get; set; }

        [JsonProperty("Geolocation")]
        public Geolocation Geolocation { get; set; }
    }
}
