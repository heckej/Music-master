using Newtonsoft.Json;

namespace Crunch.NET.Request
{
    public class Application
    {
        [JsonProperty("applicationId")]
        public string ApplicationId { get; set; }
    }
}
