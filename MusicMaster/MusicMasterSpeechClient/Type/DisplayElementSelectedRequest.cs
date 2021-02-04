using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Crunch.NET.Request.Type
{
    public class DisplayElementSelectedRequest : Request
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
