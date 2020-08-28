using Newtonsoft.Json;

namespace Crunch.NET.Request.Type
{
    public class AccountLinkSkillEventDetail
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
    }
}