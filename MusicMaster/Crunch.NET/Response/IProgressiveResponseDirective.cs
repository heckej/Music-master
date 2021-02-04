using Newtonsoft.Json;

namespace Crunch.NET.Response
{
    public interface IProgressiveResponseDirective
    {
        [JsonRequired]
        string Type { get; }
    }
}
