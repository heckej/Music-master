using Newtonsoft.Json;

namespace Crunch.NET.Request
{
    public class Scope
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
