using Newtonsoft.Json;

namespace Crunch.NET.Response.Directive
{
    public class StopDirective : IDirective
    {
        [JsonProperty("type")]
        public string Type => "AudioPlayer.Stop";
    }
}
