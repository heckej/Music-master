using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tools;
using UserCommandLogic;

namespace MusicMasterBot.TextToSpeech
{
    /// <summary>
    /// Middleware for translating text between the user and bot.
    /// Uses the Microsoft Translator Text API.
    /// </summary>
    public class VoiceMiddleware : IMiddleware
    {
        private readonly IPlayer _player;
        private readonly double _dimmedVolumeLevel = 40;
        private readonly ISongChooser _songChooser;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceMiddleware"/> class.
        /// </summary>
        public VoiceMiddleware(UserState userState, IPlayer player, ISongChooser songChooser)
        {
            _player = player;
            _songChooser = songChooser;
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

                    // Lower the volume for a moment.

                    var previousVolume = _player.GetPlayerStatus().GetVolume();
                    if (previousVolume.volumeRight > _dimmedVolumeLevel || previousVolume.volumeLeft > _dimmedVolumeLevel)
                        _player.SetVolume(_dimmedVolumeLevel);

                    foreach (Activity currentActivity in activities.Where(a => a.Type == ActivityTypes.Message))
                    {
                        var text = currentActivity.AsMessageActivity().Speak;
                        if (text != null)
                        {
                            Console.WriteLine("Speaking: " + text);
                            IList<string> words = new List<string>();
                            words.Add(text);
                            /*var artistName = _songChooser.GetKnownArtistFromSentence(text);
                            var songTitle = _songChooser.GetKnownSongTitleFromSentence(text);
                            if (!((artistName is null || !text.Contains(artistName)) || (!(songTitle is null) || !text.Contains(songTitle))))
                            {
                                // both not null
                                var indexArtist = text.IndexOf(artistName);
                                var indexTitle = text.IndexOf(songTitle);
                                if (indexArtist < indexTitle)
                                {
                                    // first artist, then title
                                    var endIndexArtist = indexArtist + artistName.Length;
                                    words.Add(text.Remove(indexArtist)); // add part before artist name
                                    words.Add(artistName); // add artist name

                                    text = text.Substring(endIndexArtist);

                                    var endIndexTitle = indexTitle + songTitle.Length;
                                    words.Add(text.Remove(indexTitle)); // add part before title
                                    words.Add(songTitle); // add title

                                    text = text.Substring(endIndexTitle);
                                } 
                                else
                                {
                                    // first title, then artist
                                    var endIndexTitle = indexTitle + songTitle.Length;
                                    words.Add(text.Remove(indexTitle)); // add part before title
                                    words.Add(songTitle); // add title

                                    text = text.Substring(endIndexTitle);

                                    var endIndexArtist = indexArtist + artistName.Length;
                                    words.Add(text.Remove(indexArtist)); // add part before artist name
                                    words.Add(artistName); // add artist name

                                    text = text.Substring(endIndexArtist);
                                }
                            }
                            else if (!(artistName is null) && text.Contains(artistName))
                            {
                                // only artist not null
                                var indexArtist = text.IndexOf(artistName);
                                var endIndexArtist = indexArtist + artistName.Length;
                                words.Add(text.Remove(indexArtist)); // add part before artist name
                                words.Add(artistName); // add artist name

                                text = text.Substring(endIndexArtist);
                            }
                            else if (!(songTitle is null) && text.Contains(songTitle))
                            {
                                // only title not null
                                var indexTitle = text.IndexOf(songTitle);
                                var endIndexTitle = indexTitle + songTitle.Length;
                                words.Add(text.Remove(indexTitle)); // add part before title
                                words.Add(songTitle); // add title

                                text = text.Substring(endIndexTitle);
                            }

                            words.Add(text); // add last part*/

                            foreach (var word in words)
                            {
                                var speak = Voice.Speak(word);
                                if (speak != null && speak.ContainsKey("debug") && speak.ContainsKey("output"))
                                    Console.WriteLine("Speech result: debug: " + speak["debug"] + ", output: " + speak["output"]);
                            }
                            
                        }
                    }

                    // Reset volume to previous values
                    _player.SetVolume(previousVolume.volumeLeft, previousVolume.volumeRight);
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
                        Voice.Speak(activity.AsMessageActivity().Speak);
                    }
                }

                return await nextUpdate();
            });

            await next(cancellationToken).ConfigureAwait(false);
        }
    }
}
