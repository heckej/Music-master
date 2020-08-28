using Crunch.NET.Request.Type;
using Newtonsoft.Json;

namespace Crunch.NET.Request
{
    public class CrunchRequest
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("session")]
        public string Session { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }

        [JsonProperty("request")]
        public string Request { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        public System.Type GetRequestType()
        {
            return null;
        }
    }
}