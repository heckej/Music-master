using Bot.Builder.Community.Adapters.Crunch;
using Microsoft.Extensions.Logging;
using MusicMasterBot.TextToSpeech;
using System;

namespace MusicMasterBot
{
    public class CrunchAdapterWithErrorHandler : CrunchAdapter
    {
        public CrunchAdapterWithErrorHandler(ILogger<CrunchAdapter> logger, VoiceMiddleware voiceMiddleware = null)
            : base(new CrunchAdapterOptions() { ShouldEndSessionByDefault = false, ValidateIncomingCrunchRequests = false, AlexaSkillId = "XXXX" }, logger)
        {

            if (voiceMiddleware != null)
            {
                // Add voice middleware to the adapter's middleware pipeline
                Use(voiceMiddleware);
            }

            //Adapter.Use(new AlexaIntentRequestToMessageActivityMiddleware());

            OnTurnError = async (turnContext, exception) =>
            {
                Console.WriteLine(exception);
                // Log any leaked exception from the application.
                logger.LogError($"Exception caught : {exception.Message}");

                // Send a catch-all apology to the user.
                await turnContext.SendActivityAsync("Sorry, it looks like something went wrong.");
            };
        }
    }
}
