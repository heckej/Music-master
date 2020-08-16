using Newtonsoft.Json;
using System;
using Crunch.NET.Helpers;

namespace Crunch.NET.Request.Type
{
    [JsonConverter(typeof(RequestConverter))]
    public abstract class Request
    {
        [JsonProperty("type",Required = Required.Always)]
        public string Type { get; set; }

        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("timestamp"),JsonConverter(typeof(MixedDateTimeConverter))]
        public DateTime Timestamp { get; set; }

        [JsonProperty("command")]
        public string Command { get; set; }
    }
}