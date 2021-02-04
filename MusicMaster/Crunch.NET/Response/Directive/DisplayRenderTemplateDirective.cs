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