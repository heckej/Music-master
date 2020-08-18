using Newtonsoft.Json;

namespace Crunch.NET.Request
{
    public class ResolutionStatus
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
