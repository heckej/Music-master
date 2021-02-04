using Crunch.NET.Response.Directive.Templates;
using Newtonsoft.Json;

namespace Crunch.NET.Response.Directive
{
    public class HintDirective : IDirective
    {
        public HintDirective()
        {
        }

        public HintDirective(string hintText, string textType = TextType.Plain)
        {
            Hint = new Hint(hintText, textType);
        }

        [JsonProperty("type")]
        public string Type => "Hint";

        [JsonProperty("hint")]
        public Hint Hint { get; set; }
    }
}
