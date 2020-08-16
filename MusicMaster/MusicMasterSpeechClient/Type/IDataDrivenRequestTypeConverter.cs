using Newtonsoft.Json.Linq;

namespace Crunch.NET.Request.Type
{
    public interface IDataDrivenRequestTypeConverter : IRequestTypeConverter
    {
        Request Convert(JObject data);
    }
}