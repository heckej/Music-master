using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Crunch.NET.Response.Directive
{
    public static class ConnectionSendRequestFactory
    {
        public static List<IConnectionSendRequestHandler> Handlers = new List<IConnectionSendRequestHandler>
        {
            new AskForPermissionDirectiveHandler()
        };


        public static IDirective Create(JObject data)
        {
            var handler = Handlers.FirstOrDefault(h => h.CanCreate(data));

            if (handler == null)
            {
                throw new InvalidOperationException("Unable to parse Connections.SendRequest directive");
            }

            return handler.Create();
        }
    }
}
