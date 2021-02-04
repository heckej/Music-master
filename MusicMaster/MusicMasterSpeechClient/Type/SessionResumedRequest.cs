using Newtonsoft.Json;

namespace Crunch.NET.Request.Type
{
    public class SessionResumedRequest : Request
    {
        [JsonProperty("originIpAddress")]
        public string OriginIpAddress { get; set; }

        [JsonProperty("cause")]
        public SessionResumedRequestCause Cause { get; set; }
    }
}
