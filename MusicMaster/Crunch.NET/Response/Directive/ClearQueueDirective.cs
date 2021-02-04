using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Crunch.NET.Response.Directive
{
    public class ClearQueueDirective : IDirective
    {
        [JsonProperty("type")]
        public string Type => "AudioPlayer.ClearQueue";

        [JsonProperty("clearBehavior")]
        [JsonRequired]
        [JsonConverter(typeof(StringEnumConverter))]
        public ClearBehavior ClearBehavior { get; set; }
    }
}
