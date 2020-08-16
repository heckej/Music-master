using Newtonsoft.Json;

namespace Crunch.NET.Response.Directive
{
    public class SlotType
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("values")]
        public SlotTypeValue[] Values { get; set; }
    }
}