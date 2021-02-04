using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Crunch.NET.Request.Type
{
    public class Error
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorType Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
