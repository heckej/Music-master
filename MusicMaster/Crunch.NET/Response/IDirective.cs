using Crunch.NET.Response.Converters;
using Newtonsoft.Json;

namespace Crunch.NET.Response
{
    [JsonConverter(typeof(DirectiveConverter))]
    public interface IDirective
    {
        [JsonRequired]
        string Type { get; }
    }
}