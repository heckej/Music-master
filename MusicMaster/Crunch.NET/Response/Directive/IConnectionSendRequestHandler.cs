using Newtonsoft.Json.Linq;

namespace Crunch.NET.Response.Directive
{
    public interface IConnectionSendRequestHandler
    {
        bool CanCreate(JObject data);

        ConnectionSendRequest Create();
    }
}