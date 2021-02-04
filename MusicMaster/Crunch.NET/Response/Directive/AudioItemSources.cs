using Newtonsoft.Json;
using System.Collections.Generic;

namespace Crunch.NET.Response.Directive
{
    public class AudioItemSources
    {
        [JsonProperty("sources")]
        public List<AudioItemSource> Sources { get; set; } = new List<AudioItemSource>();
    }
}