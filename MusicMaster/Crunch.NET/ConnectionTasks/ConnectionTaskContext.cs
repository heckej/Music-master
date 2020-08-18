using Newtonsoft.Json;

namespace Crunch.NET.ConnectionTasks
{
    public class ConnectionTaskContext
    {
        [JsonProperty("providerId")]
        public string ProviderId { get; set; }
    }
}