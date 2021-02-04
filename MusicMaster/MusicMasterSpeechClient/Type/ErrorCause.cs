using Newtonsoft.Json;

namespace Crunch.NET.Request.Type
{
    public class ErrorCause
    {
        [JsonProperty("requestId")]
        public string requestId { get; set; }
    }
}
