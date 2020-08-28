using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Crunch.NET.Request.Type
{
    public class AccountLinkSkillEventRequest:SkillEventRequest
    {
        [JsonProperty("body")]
        public AccountLinkSkillEventDetail Body { get; set; }
    }
}
