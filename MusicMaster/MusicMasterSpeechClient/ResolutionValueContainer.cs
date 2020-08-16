using Newtonsoft.Json;

namespace Crunch.NET.Request
{
    public class ResolutionValueContainer
    {
        [JsonProperty("value")]
        public ResolutionValue Value { get; set; }
    }
}
