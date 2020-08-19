using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tools;

namespace MusicMasterBot.TextToSpeech
{
    /// <summary>
    /// Middleware for translating text between the user and bot.
    /// Uses the Microsoft Translator Text API.
    /// </summary>
    public class VoiceMiddleware : IMiddleware
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceMiddleware"/> class.
        /// </summary>
        public VoiceMiddleware(UserState userState)
        {
        }

        private bool shouldSpeak = true;

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
                }

            turnContext.OnSendActivities(async (newContext, activities, nextSend) =>
            {
                // Read messages sent to the user
                if (shouldSpeak)
                {
                    /*List<Task> tasks = new List<Task>();
                    foreach (Activity currentActivity in activities.Where(a => a.Type == ActivityTypes.Message))
                    {
                        tasks.Add(Voice.Speak(""));
                    }

                    if (tasks.Any())
                    {
                        await Task.WhenAll(tasks).ConfigureAwait(false);
                    }*/
                    foreach (Activity currentActivity in activities.Where(a => a.Type == ActivityTypes.Message))
                    {
                        var text = currentActivity.AsMessageActivity().Speak;
                        if (text != null && !text.StartsWith("//"))
                        {
                            Console.WriteLine("Speaking: " + text);
                            var speak = Voice.Speak(text);
                            if (speak != null && speak.ContainsKey("debug") && speak.ContainsKey("output"))
                                Console.WriteLine("Speech result: debug: " + speak["debug"] + ", output: " + speak["output"]);
                        } else if (text != null)
                        {
                            currentActivity.AsMessageActivity().Text = text.Substring(2);
                        }
                    }                    
                }

                return await nextSend();
            });

            turnContext.OnUpdateActivity(async (newContext, activity, nextUpdate) =>
            {
                // Read messages sent to the user
                if (activity.Type == ActivityTypes.Message)
                {
                    if (shouldSpeak)
                    {
                        Voice.Speak(activity.AsMessageActivity().Text);
                    }
                }

                return await nextUpdate();
            });

            await next(cancellationToken).ConfigureAwait(false);
        }
    }
}
