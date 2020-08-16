using Newtonsoft.Json;

namespace Crunch.NET.Request.Type
{
    public class SkillEventPermissions
    {
        [JsonProperty("acceptedPermissions")]
        public Permission[] AcceptedPermissions { get; set; }
    }
}