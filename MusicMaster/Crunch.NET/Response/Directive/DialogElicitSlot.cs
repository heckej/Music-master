using System;
using Crunch.NET.Request;
using Newtonsoft.Json;

namespace Crunch.NET.Response.Directive
{
    public class DialogElicitSlot : IDirective
    {
        [JsonProperty("type")]
        public string Type => "Dialog.ElicitSlot";

        [JsonProperty("slotToElicit"), JsonRequired]
        public string SlotName { get; set; }

        [JsonProperty("updatedIntent", NullValueHandling = NullValueHandling.Ignore)]
        public Intent UpdatedIntent { get; set; }

        public DialogElicitSlot(string slotName)
        {
            SlotName = slotName;
        }

        internal DialogElicitSlot()
        {
        }
    }
}