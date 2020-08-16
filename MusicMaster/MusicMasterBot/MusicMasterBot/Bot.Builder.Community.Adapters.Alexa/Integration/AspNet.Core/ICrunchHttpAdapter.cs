using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;

namespace Bot.Builder.Community.Adapters.Crunch.Integration.AspNet.Core
{
    public interface ICrunchHttpAdapter
    {
        Task ProcessAsync(HttpRequest httpRequest, HttpResponse httpResponse, IBot bot, CancellationToken cancellationToken = default(CancellationToken));
    }
}
