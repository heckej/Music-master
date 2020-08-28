using Microsoft.Extensions.Logging;

namespace Bot.Builder.Community.Adapters.Crunch.Core
{
    public class CrunchRequestMapperOptions
    {
        public string ChannelId { get; set; } = "alexa";
        public string ServiceUrl { get; set; } = null;
        public string DefaultIntentSlotName { get; set; } = "phrase";
        public bool ShouldEndSessionByDefault { get; set; } = true;
    }
}
