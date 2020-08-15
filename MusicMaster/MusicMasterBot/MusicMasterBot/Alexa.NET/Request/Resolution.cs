using Newtonsoft.Json;

namespace Crunch.NET.Request
{
    public class Resolution
    {
        [JsonProperty("resolutionsPerAuthority")]
        public ResolutionAuthority[] Authorities { get; set; }
    }
}
