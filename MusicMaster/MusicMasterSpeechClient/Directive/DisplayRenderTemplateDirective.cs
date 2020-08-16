using System;
using System.Collections.Generic;
using System.Text;
using Crunch.NET.Response.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Crunch.NET.Response.Directive
{
    public class DisplayRenderTemplateDirective : IDirective
    {
        [JsonProperty("type")]
        public string Type => "Display.RenderTemplate";

        [JsonProperty("template", Required = Required.Always)]
        public ITemplate Template { get; set; }
    }
}