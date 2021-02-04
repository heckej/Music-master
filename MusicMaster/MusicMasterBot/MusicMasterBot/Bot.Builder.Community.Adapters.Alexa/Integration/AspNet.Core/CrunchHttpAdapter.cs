using Microsoft.Extensions.Logging;

namespace Bot.Builder.Community.Adapters.Crunch.Integration.AspNet.Core
{
    public class CrunchHttpAdapter : CrunchAdapter, ICrunchHttpAdapter
    {
        public CrunchHttpAdapter(bool validateRequests, ILogger logger = null)
            : base(new CrunchAdapterOptions() { ValidateIncomingCrunchRequests = validateRequests }, logger)
        {
        }

        public CrunchHttpAdapter(CrunchAdapterOptions options = null, ILogger logger = null)
            : base(options, logger)
        {
        }
    }
}