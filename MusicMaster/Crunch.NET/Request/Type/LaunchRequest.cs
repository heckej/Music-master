using Newtonsoft.Json;

namespace Crunch.NET.Request.Type
{
    public class LaunchRequest : Request
    {
        [JsonProperty("task",NullValueHandling = NullValueHandling.Ignore)]
        public LaunchRequestTask Task { get; set; }
    }
}