using Newtonsoft.Json;

namespace Crunch.NET.Request.Type
{
    public class SkillEnablementSkillEventRequest : SkillEventRequest
    {
        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public SkillEventPersistenceStatus Body { get; set; }
    }
}
