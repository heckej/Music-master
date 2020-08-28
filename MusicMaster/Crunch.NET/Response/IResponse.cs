using Newtonsoft.Json;

namespace Crunch.NET.Response
{
    public interface IResponse
    {
        [JsonRequired]
        string Type { get; }
    }
}
