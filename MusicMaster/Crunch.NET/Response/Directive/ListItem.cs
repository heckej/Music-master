using Crunch.NET.Response.Directive.Templates;
using Newtonsoft.Json;

namespace Crunch.NET.Response.Directive
{
    public class ListItem
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("image")]
        public TemplateImage Image { get; set; }

        [JsonProperty("textContent")]
        public TemplateContent Content { get; set; }
    }
}