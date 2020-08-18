using Newtonsoft.Json;

namespace Crunch.NET.Request.Type
{
    public class Permission
    {
        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
}