using Newtonsoft.Json;

namespace Crunch.NET.Request.Type
{
    public class LocationServices
    {
        [JsonProperty("access")]
        public LocationServiceAccess Access { get; set; }

        [JsonProperty("status")]
        public LocationServiceStatus Status { get; set; }
    }
}
