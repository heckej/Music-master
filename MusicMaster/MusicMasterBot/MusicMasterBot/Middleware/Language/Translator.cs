using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tools;
using UserCommandLogic;

namespace MusicMasterBot.Middleware.Language
{
    /// <summary>
    /// Middleware for translating text between the user and bot.
    /// Uses the Microsoft Translator Text API.
    /// </summary>
    public class Translator : IMiddleware
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceMiddleware"/> class.
        /// </summary>
        public Translator(UserState userState)
        {
        }

        /// <summary>
        /// Processes an incoming activity.
        /// </summary>
        /// <param name="turnContext">Context object containing information for a single turn of conversation with a user.</param>
        /// <param name="next">The delegate to call to continue the bot middleware pipeline.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // User says something.
                var text = turnContext.Activity.AsMessageActivity().Text;
                if (turnContext.Activity.AsMessageActivity().Locale == "nl" )
                {
                    turnContext.Activity.AsMessageActivity().Text = TranslateFromDutch(text);
                }
            }

            turnContext.OnSendActivities(async (newContext, activities, nextSend) =>
            {
                // Read messages sent to the user

                    foreach (Activity currentActivity in activities.Where(a => a.Type == ActivityTypes.Message))
                    {
                        var text = currentActivity.AsMessageActivity().Text;
                }

                return await nextSend();
            });

            turnContext.OnUpdateActivity(async (newContext, activity, nextUpdate) =>
            {
                // Read messages sent to the user
                if (activity.Type == ActivityTypes.Message)
                {

                }

                return await nextUpdate();
            });

            await next(cancellationToken).ConfigureAwait(false);
        }

        private static IDictionary<string, string> _dutchToEnglish = InitDutchToEnglish();

        private static IDictionary<string, string> InitDutchToEnglish()
        {
            return new Dictionary<string, string>()
            {
                {"artiest", "artist"},
                {"titel", "title"},
                {"wat dacht je van wat muziek", "what about some music"},
                {"wat muziek", "some music"},
                {"nummer", "song"},
                {"liedje", "song"},
                {"lied", "song"},
                {"muziek", "music"},
                {"speel", "play"},
                {"voor mij", "me"},
                {"voor me", "me"},
                {"eens", ""},
                {"iets van", "something by"},
                {"iets van muziek", "some music"},
                {"luider", "louder"},
                {"omhoog", "up"},
                {"hoger", "up"},
                {"stilte", "silence"},
                {"stil", "silent"},
                {"let", "laat"},
                {"laat me iets horen", "let me hear something"},
                {"van", "by"},
                {"de", "the"},
                {"het", "it"},
                {"wat", "what"},
                {"who", "wie"},
                {"dit", "this"}
            };
        }

        private string TranslateFromDutch(string text)
        {
            text = text.ToLower();
            foreach ((string dutchWord, string englishWord) in _dutchToEnglish)
            {
                if (text.Contains(dutchWord))
                    text = text.Replace(dutchWord, englishWord);
            }
            return text;
        }
    }
}
