using Newtonsoft.Json;

namespace Crunch.NET.Response
{
    public class ProgressiveResponseHeader
    {
        public ProgressiveResponseHeader(string requestId)
        {
            RequestId = requestId;
        }

        [JsonProperty("requestId")]
        public string RequestId { get; }
    }
}
