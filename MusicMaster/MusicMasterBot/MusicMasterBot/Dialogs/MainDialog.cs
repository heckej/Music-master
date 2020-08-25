// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.9.2

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using MusicMasterBot.CognitiveModels;
using Tools;
using UserCommandLogic;

namespace MusicMasterBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly UserCommandRecognizer _luisCommandRecognizer;
        private readonly UserQuestionRecognizer _luisQuestionRecognizer;
        protected readonly ILogger Logger;
        private readonly ISongChooser _songChooser;
        private readonly IPlayer _musicPlayer;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(UserCommandRecognizer luisCommandRecognizer, UserQuestionRecognizer luisQuestionRecognizer, RequestSongDialog requestSongDialog, ILogger<MainDialog> logger, 
            ISongChooser songChooser, IPlayer musicPlayer)
            : base(nameof(MainDialog))
        {
            _luisCommandRecognizer = luisCommandRecognizer;
            _luisQuestionRecognizer = luisQuestionRecognizer;
            Logger = logger;
            _songChooser = songChooser;
            _musicPlayer = musicPlayer;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(requestSongDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisCommandRecognizer.IsConfigured || !_luisQuestionRecognizer.IsConfigured)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }

            // Use the text provided in FinalStepAsync or the default if it is the first time.
            var messageText = stepContext.Options?.ToString() ?? "What can I help you with today?\nSay something like \"Play Someone Like You by Adele\"";
            var promptMessage = MessageFactory.Text(messageText, "", InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userInput = stepContext.Context.Activity.Text;
            SongRequest songRequest;
            if (!_luisCommandRecognizer.IsConfigured)
            {
                songRequest = new SongRequest()
                {
                    Artist = _songChooser.GetKnownArtistFromSentence(userInput),
                    Title = _songChooser.GetKnownSongTitleFromSentence(userInput)
                };
                // LUIS is not configured, we just run the RequestSongDialog path with an empty SongDetailsInstance.
                return await stepContext.BeginDialogAsync(nameof(RequestSongDialog), songRequest, cancellationToken);
            }

            if (_luisQuestionRecognizer.IsConfigured)
            {
                var luisQuestionResult = await _luisQuestionRecognizer.RecognizeAsync<UserQuestion>(stepContext.Context, cancellationToken);
                var (intent, score) = luisQuestionResult.TopIntent();
                if (!(intent is UserQuestion.Intent.None))
                {
                    var feedbackScoreThreshold = 0.8;
                    Answer answer = new Answer()
                    {
                        Text = "",
                        Speak = ""
                    };
                    switch (intent)
                    {
                        case UserQuestion.Intent.CurrentSongInformationRequest:
                            var question = new Question()
                            {
                                Text = userInput,
                                Intent = intent
                            };
                            answer = new Answer(question);

                            // Which song 
                            /*switch (luisQuestionResult.SongDescription)
                            {
                                case "current song":
                                    break;
                                case "next song":
                                case "last song":
                                default:
                                    break;
                            }*/

                            // Which information requested
                            var song = _musicPlayer.GetPlayerStatus().GetCurrentSong();
                            (answer.Text, answer.Speak) = GenerateResponseToInformationRequest(luisQuestionResult.SongInformation, song);
                            break;
                        case UserQuestion.Intent.NegativeFeedback:
                            // check certainty percentage to avoid false positives
                            if (score > feedbackScoreThreshold)
                                break;
                            break;
                        case UserQuestion.Intent.PositiveFeedback:
                            // check certainty percentage to avoid false positives
                            if (score > feedbackScoreThreshold)
                                break;
                            break;
                        default:
                            break;
                    }
                    return await stepContext.NextAsync(answer, cancellationToken);
                }
            }

            // Call LUIS and gather any potential song details. (Note the TurnContext has the response to the prompt.)
            var luisCommandResult = await _luisCommandRecognizer.RecognizeAsync<UserCommand>(stepContext.Context, cancellationToken);
            songRequest = new SongRequest()
            {
                Intent = luisCommandResult.TopIntent().intent
            };

            var inputHint = InputHints.IgnoringInput;
            string messageText = "";
            string spokenMessageText = "";
            Song songToBePlayed = null;

            switch (luisCommandResult.TopIntent().intent)
            {
                case UserCommand.Intent.PlayByTitleArtist:

                    // Initialize SongRequest with any entities we may have found in the response.
                    songRequest.Title = luisCommandResult.SongTitle;
                    songRequest.Artist = luisCommandResult.SongArtist;
                    songToBePlayed = SelectSongToBePlayedByTitleArtist(songRequest, userInput);

                    if (songToBePlayed is null)
                    {
                        // Reset song request if both known, but not a pair
                        if (!(songRequest.Title is null || songRequest.Artist is null))
                            if (songRequest.Title.Length > songRequest.Artist.Length)
                                songRequest.Artist = null;
                            else
                                songRequest.Title = null;
                        // Run the RequestSongDialog giving it whatever details we have from the LUIS call, it will fill out the remainder.
                        return await stepContext.BeginDialogAsync(nameof(RequestSongDialog), songRequest, cancellationToken);
                    }
                    break;

                case UserCommand.Intent.PlayByArtist:

                    // Initialize SongRequest with any entities we may have found in the response.
                    songRequest.Artist = luisCommandResult.SongArtist;
                    if (songRequest.Artist is null)
                        songRequest.Artist = _songChooser.GetKnownArtistFromSentence(userInput);

                    var (bestMatchArtist, similarityRatioArtist) = _songChooser.GetClosestKnownArtist(luisCommandResult.SongArtist, _songChooser.ThresholdSimilarityRatio);
                    Console.WriteLine(songRequest.Artist + " => " + bestMatchArtist);

                    if (bestMatchArtist is null)
                    {
                        songRequest.Artist = _songChooser.ExpandSentenceToKnownArtist(userInput);

                        if (songRequest.Artist is null)
                        {
                            // Run the RequestSongDialog giving it whatever details we have from the LUIS call, it will fill out the remainder.
                            return await stepContext.BeginDialogAsync(nameof(RequestSongDialog), songRequest, cancellationToken);
                        }
                        bestMatchArtist = songRequest.Artist;
                    }
                    songRequest.Artist = bestMatchArtist;
                    songToBePlayed = _songChooser.ChooseRandomSongByArtist(songRequest.Artist);
                    messageText = "Random song by artist.";
                    break;

                case UserCommand.Intent.PlayByTitle:

                    // Initialize SongRequest with any entities we may have found in the response.
                    songRequest.Title = luisCommandResult.SongTitle;
                    if (songRequest.Title is null)
                        songRequest.Title = _songChooser.GetKnownSongTitleFromSentence(userInput);

                    var (bestMatchTitle, similarityRatioTitle) = _songChooser.GetClosestKnownSongTitle(luisCommandResult.SongTitle, _songChooser.ThresholdSimilarityRatio);

                    if (bestMatchTitle is null)
                    {
                        songRequest.Title = _songChooser.ExpandSentenceToKnownSongTitle(userInput);

                        if (songRequest.Title is null)
                        {
                            // Run the RequestSongDialog giving it whatever details we have from the LUIS call, it will fill out the remainder.
                            return await stepContext.BeginDialogAsync(nameof(RequestSongDialog), songRequest, cancellationToken);
                        }
                        bestMatchTitle = songRequest.Title;
                    }
                    songRequest.Title = bestMatchTitle;
                    songToBePlayed = _songChooser.GetSongByClosestTitle(songRequest.Title);
                    break;

                case UserCommand.Intent.PlayByGenre:

                    // Initialize SongRequest with any entities we may have found in the response.
                    songRequest.Genre = luisCommandResult.SongGenre;

                    var (bestMatchGenre, similarityRatioGenre) = _songChooser.GetClosestKnownGenre(luisCommandResult.SongGenre, _songChooser.ThresholdSimilarityRatio);

                    if (bestMatchGenre is null)
                    {
                        // Run the RequestSongByGenreDialog giving it whatever details we have from the LUIS call, it will fill out the remainder.
                        return await stepContext.BeginDialogAsync(nameof(RequestSongByGenreDialog), songRequest, cancellationToken);
                    }

                    songToBePlayed = _songChooser.ChooseRandomSongByGenre(bestMatchGenre);
                    messageText = "Random song by genre.";
                    break;

                case UserCommand.Intent.PlayRandom:
                    songToBePlayed = _songChooser.ChooseRandomSong();
                    messageText = "Random song.";
                    break;

                case UserCommand.Intent.PreviousSong:
                    _musicPlayer.PlayPrevious();
                    messageText = "Previous song.";
                    break;
                case UserCommand.Intent.NextSong:
                    _musicPlayer.PlayNext();
                    messageText = "Next song.";
                    break;
                case UserCommand.Intent.StartPlaying:
                    _musicPlayer.Resume();
                    messageText = "Music resumed.";
                    break;
                case UserCommand.Intent.StopPlaying:
                    _musicPlayer.Pause();
                    messageText = "Music paused.";
                    break;
                case UserCommand.Intent.VolumeDown:
                    _musicPlayer.VolumeDown();
                    messageText = "Volume decreased by 10%.";
                    break;
                case UserCommand.Intent.VolumeUp:
                    _musicPlayer.VolumeUp(20);
                    messageText = "Volume increased by 20%.";
                    break;

                default:
                    // Catch all for unhandled intents
                    messageText = $"Sorry, I didn't get that. Please try asking in a different way (intent was {luisCommandResult.TopIntent().intent})";
                    (_, spokenMessageText) = SentenceGenerator.CommandNotUnderstood();
                    inputHint = InputHints.ExpectingInput;
                    break;
            }

            var message = MessageFactory.Text(messageText, spokenMessageText, inputHint);
            await stepContext.Context.SendActivityAsync(message, cancellationToken);

            return await stepContext.NextAsync(songToBePlayed, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // If the child dialog ("RequestSongDialog") was cancelled, the user failed to confirm or if the intent wasn't PlayByTitleArtist
            // the Result here will be null.
            string messageText;
            string spokenMessageText;
            Activity message;
            switch (stepContext.Result)
            {
                case Song songToBePlayed:
                    // Now we have all the song details call the music player.
                    try
                    {
                        _musicPlayer.Play(songToBePlayed);

                        // If the call to the music player service was successful tell the user.
                        (messageText, spokenMessageText) = SentenceGenerator.PresentSongToBePlayed(songToBePlayed);
                        message = MessageFactory.Text(messageText, spokenMessageText, InputHints.IgnoringInput);
                    } catch(Exception e)
                    {
                        messageText = e.ToString();
                        spokenMessageText = "Sorry, something went wrong, so I cannot play the song.";
                        message = MessageFactory.Text(messageText, spokenMessageText, InputHints.IgnoringInput);
                    }
                    break;
                case Answer answer:
                    (messageText, spokenMessageText) = (answer.Text, answer.Speak);
                    message = MessageFactory.Text(messageText, spokenMessageText, InputHints.IgnoringInput);
                     break;
                default:
                    (messageText, spokenMessageText) = SentenceGenerator.CommandNotUnderstood();
                    message = MessageFactory.Text(messageText, spokenMessageText, InputHints.IgnoringInput);
                    break;
            }
            await stepContext.Context.SendActivityAsync(message, cancellationToken);

            // Restart the main dialog with a different message the second time around
            var (promptMessage, _) = SentenceGenerator.ProposeFurtherHelp();
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }

        private (string writtenText, string spokenText) GenerateResponseToInformationRequest(string[] requestInformation, Song song)
        {

            if (requestInformation.Contains("title") && requestInformation.Contains("artist"))
            {
                return SentenceGenerator.DescribeTitleArtistCurrentSong(song);
            }
            else if (requestInformation.Contains("title"))
            {
                return SentenceGenerator.DescribeTitleCurrentSong(song);
            }
            else if (requestInformation.Contains("artist"))
            {
                return SentenceGenerator.DescribeArtistCurrentSong(song);
            }
            else if (requestInformation.Contains("when") && requestInformation.Contains("album"))
            {
                return SentenceGenerator.DescribeAlbumYearCurrentSong(song);
            }
            else if (requestInformation.Contains("when"))
            {
                return SentenceGenerator.DescribeYearCurrentSong(song);
            }
            else if (requestInformation.Contains("album"))
            {
                return SentenceGenerator.DescribeAlbumCurrentSong(song);
            }
            return ("", "");
        }

        private Song SelectSongToBePlayedByTitleArtist(SongRequest songRequest, string userInput)
        {
            Song songToBePlayed;
            Console.WriteLine(songRequest.Artist + " ?=>? " + _songChooser.GetClosestKnownArtist(songRequest.Artist).bestMatch);
            Console.WriteLine(songRequest.Artist + " ?=>? " + _songChooser.GetKnownArtistFromSentence(userInput));
            Console.WriteLine(songRequest.Title + " ?=>? " + _songChooser.GetClosestKnownSongTitle(songRequest.Title).bestMatch);
            Console.WriteLine(songRequest.Artist + " ?=>? " + _songChooser.GetKnownSongTitleFromSentence(userInput));

            var closestKnownArtist = _songChooser.GetClosestKnownArtist(songRequest.Artist).bestMatch;
            var closestKnownTitle = _songChooser.GetClosestKnownSongTitle(songRequest.Title).bestMatch;
            if (songRequest.Artist is null || closestKnownArtist is null)
                songRequest.Artist = _songChooser.GetKnownArtistFromSentence(userInput);
            if (songRequest.Title is null || closestKnownTitle is null)
                songRequest.Title = _songChooser.GetKnownSongTitleFromSentence(userInput);

            songToBePlayed = _songChooser.GetSongByClosestArtistOrTitle(songRequest.Title, songRequest.Artist);

            if (songToBePlayed is null)
            {
                Console.WriteLine(songRequest.Artist + ", " + songRequest.Title);
                if (songRequest.Artist is null && closestKnownArtist is null)
                    songRequest.Artist = _songChooser.ExpandSentenceToKnownArtist(userInput);
                if (songRequest.Title is null && closestKnownTitle is null)
                    songRequest.Title = _songChooser.ExpandSentenceToKnownSongTitle(userInput);

                songToBePlayed = _songChooser.GetSongByClosestArtistOrTitle(songRequest.Title, songRequest.Artist);
            }
            return songToBePlayed;
        }
    }
}
