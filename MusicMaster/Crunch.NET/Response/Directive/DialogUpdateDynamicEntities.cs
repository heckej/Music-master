using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Crunch.NET.Response.Directive
{
    public class DialogUpdateDynamicEntities : IDirective
    {
        [JsonProperty("type")]
        public string Type => "Dialog.UpdateDynamicEntities";

        [JsonProperty("updateBehavior"), JsonConverter(typeof(StringEnumConverter))]
        public UpdateBehavior UpdateBehavior { get; set; }

        [JsonProperty("types")]
        public List<SlotType> Types { get; set; } = new List<SlotType>();
    }
}
