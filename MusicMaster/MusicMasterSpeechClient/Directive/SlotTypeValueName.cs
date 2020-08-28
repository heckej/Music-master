using Newtonsoft.Json;

namespace Crunch.NET.Response.Directive
{
    public class SlotTypeValueName
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("synonyms", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Synonyms { get; set; }
    }
}