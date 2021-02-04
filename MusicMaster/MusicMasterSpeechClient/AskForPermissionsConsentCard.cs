using Newtonsoft.Json;
using System.Collections.Generic;
namespace Crunch.NET.Response
{
    public class AskForPermissionsConsentCard : ICard
    {

        [JsonProperty("type")]
        [JsonRequired]
        public string Type
        {
            get { return "AskForPermissionsConsent"; }
        }

        [JsonProperty("permissions")]
        [JsonRequired]
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
