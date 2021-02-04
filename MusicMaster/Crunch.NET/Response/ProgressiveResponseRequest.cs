using Newtonsoft.Json;

namespace Crunch.NET.Response
{
    public class ProgressiveResponseRequest
    {
        public ProgressiveResponseRequest()
        {
        }

        public ProgressiveResponseRequest(ProgressiveResponseHeader header, IProgressiveResponseDirective directive)
        {
            Header = header;
            Directive = directive;
        }

        [JsonProperty("header")]
        public ProgressiveResponseHeader Header { get; set; }

        [JsonProperty("directive")]
        public IProgressiveResponseDirective Directive { get; set; }
    }
}
