using Newtonsoft.Json;

namespace Crunch.NET.Response.Directive.Templates
{
    public class TemplateText
    {
        [JsonProperty("text", Required = Required.Always)]
        public string Text { get; set; }

        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; set; }
    }
}
