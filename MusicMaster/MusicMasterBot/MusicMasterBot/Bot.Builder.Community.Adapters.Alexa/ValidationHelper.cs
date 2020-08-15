using System.Threading.Tasks;
using Crunch.NET.Request;
using Bot.Builder.Community.Adapters.Crunch.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Bot.Builder.Community.Adapters.Crunch
{
    internal class ValidationHelper
    {
        public static bool ValidateRequest(HttpRequest request, CrunchRequest skillRequest, string body, string alexaSkillId, ILogger logger)
        {
            return true;
        }
    }
}