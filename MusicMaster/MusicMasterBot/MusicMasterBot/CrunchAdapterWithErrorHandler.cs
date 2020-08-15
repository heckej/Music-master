using Bot.Builder.Community.Adapters.Crunch;
using Microsoft.Extensions.Logging;

namespace MusicMasterBot
{
    public class CrunchAdapterWithErrorHandler : CrunchAdapter
    {
        public CrunchAdapterWithErrorHandler(ILogger<CrunchAdapter> logger)
            : base(new CrunchAdapterOptions() { ShouldEndSessionByDefault = false, ValidateIncomingCrunchRequests = false, AlexaSkillId = "XXXX" }, logger)
        {
            //Adapter.Use(new AlexaIntentRequestToMessageActivityMiddleware());

            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError($"Exception caught : {exception.Message}");

                // Send a catch-all apology to the user.
                await turnContext.SendActivityAsync("Sorry, it looks like something went wrong.");
            };
        }
    }
}
