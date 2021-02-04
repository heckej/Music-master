using Newtonsoft.Json;

namespace Crunch.NET.Response.Directive
{
    public class AudioItemSource
    {
        public AudioItemSource()
        {
        }

        public AudioItemSource(string url)
        {
            Url = url;
        }

        [JsonProperty("url"), JsonRequired]
        public string Url { get; set; }
    }
}
