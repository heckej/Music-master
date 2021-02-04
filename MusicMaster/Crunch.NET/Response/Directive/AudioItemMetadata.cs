using Newtonsoft.Json;

namespace Crunch.NET.Response.Directive
{
    public class AudioItemMetadata
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }

        [JsonProperty("art")]
        public AudioItemSources Art { get; set; } = new AudioItemSources();

        [JsonProperty("backgroundImage")]
        public AudioItemSources BackgroundImage { get; set; } = new AudioItemSources();
    }
}