using System.Collections.Generic;
using Newtonsoft.Json;

namespace Crunch.NET.Response.Directive
{
    public class AudioItemSources
    {
		[JsonProperty("sources")]
		public List<AudioItemSource> Sources { get; set; } = new List<AudioItemSource>();
    }
}