using Crunch.NET.Response.Converters;
using Crunch.NET.Response.Directive;
using Newtonsoft.Json;

namespace Crunch.NET.Response
{
    [JsonConverter(typeof(OutputSpeechConverter))]
    public interface IOutputSpeech : IResponse
    {
        PlayBehavior? PlayBehavior { get; set; }
    }
}