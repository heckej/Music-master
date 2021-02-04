﻿using Newtonsoft.Json;

namespace Crunch.NET.Request
{
    public class AlexaSystem
    {
        [JsonProperty("apiAccessToken")]
        public string ApiAccessToken { get; set; }

        [JsonProperty("apiEndpoint")]
        public string ApiEndpoint { get; set; }

        [JsonProperty("application")]
        public Application Application { get; set; }

        [JsonProperty("person", NullValueHandling = NullValueHandling.Ignore)]
        public Person Person { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("device")]
        public Device Device { get; set; }
    }
}
