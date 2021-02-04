﻿using Newtonsoft.Json;

namespace Crunch.NET.Response.Directive.Templates
{
    public class TemplateContent
    {
        [JsonProperty("primaryText", Required = Required.Always)]
        public TemplateText Primary { get; set; }

        [JsonProperty("secondaryText", NullValueHandling = NullValueHandling.Ignore)]
        public TemplateText Secondary { get; set; }

        [JsonProperty("tertiaryText", NullValueHandling = NullValueHandling.Ignore)]
        public TemplateText Tertiary { get; set; }
    }
}
