using Crunch.NET.Response.Converters;
using Newtonsoft.Json;

namespace Crunch.NET.Response
{
    [JsonConverter(typeof(CardConverter))]
    public interface ICard : IResponse
    {
    }
}