using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Crunch.NET.Request
{
    public class Permissions
    {
        [JsonProperty("consentToken"), Obsolete("ConsentToken is deprecated, please use SkillRequest.Context.System.ApiAccessToken")]
        public string ConsentToken { get; set; }

        [JsonProperty("scopes", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, Scope> Scopes { get; set; }
    }
}