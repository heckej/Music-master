// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.9.2

using System;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly UserCommandRecognizer _luisRecognizer;
        protected readonly ILogger Logger;
        private readonly ISongChooser _songChooser;
        private readonly IPlayer _musicPlayer;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(UserCommandRecognizer luisRecognizer, RequestSongDialog bookingDialog, ILogger<MainDialog> logger, 
            ISongChooser songChooser, IPlayer musicPlayer)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;
            Logger = logger;
            _songChooser = songChooser;
            _musicPlayer = musicPlayer;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(bookingDialog);
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
            if (!_luisRecognizer.IsConfigured)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }

            // Use the text provided in FinalStepAsync or the default if it is the first time.
            var messageText = stepContext.Options?.ToString() ?? "What can I help you with today?\nSay something like \"Play Someone Like You by Adele\"";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                // LUIS is not configured, we just run the RequestSongDialog path with an empty SongDetailsInstance.
                return await stepContext.BeginDialogAsync(nameof(RequestSongDialog), new SongRequest(), cancellationToken);
            }

            // Call LUIS and gather any potential song details. (Note the TurnContext has the response to the prompt.)
            var luisResult = await _luisRecognizer.RecognizeAsync<UserCommand>(stepContext.Context, cancellationToken);
            var songRequest = new SongRequest()
            {
                Intent = luisResult.TopIntent().intent
            };

            var inputHint = InputHints.IgnoringInput;
            string messageText = "";
            string spokenMessageText = "";
            Song songToBePlayed = null;

            switch (luisResult.TopIntent().intent)
            {
                case UserCommand.Intent.PlayByTitleArtist:

                    // Initialize SongRequest with any entities we may have found in the response.
                    songRequest.Title = luisResult.SongTitle;
                    songRequest.Artist = luisResult.SongArtist;

                    songToBePlayed = _songChooser.GetSongByClosestArtistOrTitle(luisResult.SongTitle, luisResult.SongArtist);

                    if (songToBePlayed is null)
                    {
                        // Run the RequestSongDialog giving it whatever details we have from the LUIS call, it will fill out the remainder.
                        return await stepContext.BeginDialogAsync(nameof(RequestSongDialog), songRequest, cancellationToken);
                    }
                    break;

                case UserCommand.Intent.PlayByArtist:

                    // Initialize SongRequest with any entities we may have found in the response.
                    songRequest.Artist = luisResult.SongArtist;

                    var (bestMatchArtist, similarityRatioArtist) = _songChooser.GetClosestKnownArtist(luisResult.SongArtist, _songChooser.ThresholdSimilarityRatio);

                    if (bestMatchArtist is null)
                    {
                        // Run the RequestSongDialog giving it whatever details we have from the LUIS call, it will fill out the remainder.
                        return await stepContext.BeginDialogAsync(nameof(RequestSongDialog), songRequest, cancellationToken);
                    }


                    songToBePlayed = _songChooser.ChooseRandomSongByArtist(bestMatchArtist);
                    messageText = "Random song by artist.";
                    break;

                case UserCommand.Intent.PlayByTitle:

                    // Initialize SongRequest with any entities we may have found in the response.
                    songRequest.Title = luisResult.SongTitle;

                    var (bestMatchTitle, similarityRatioTitle) = _songChooser.GetClosestKnownSongTitle(luisResult.SongTitle, _songChooser.ThresholdSimilarityRatio);

                    if (bestMatchTitle is null)
                    {
                        // Run the RequestSongDialog giving it whatever details we have from the LUIS call, it will fill out the remainder.
                        return await stepContext.BeginDialogAsync(nameof(RequestSongDialog), songRequest, cancellationToken);
                    }

                    songToBePlayed = _songChooser.GetSongByClosestTitle(songRequest.Title);
                    break;

                case UserCommand.Intent.PlayByGenre:

                    // Initialize SongRequest with any entities we may have found in the response.
                    songRequest.Genre = luisResult.SongGenre;

                    var (bestMatchGenre, similarityRatioGenre) = _songChooser.GetClosestKnownGenre(luisResult.SongGenre, _songChooser.ThresholdSimilarityRatio);

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
                    messageText = $"Sorry, I didn't get that. Please try asking in a different way (intent was {luisResult.TopIntent().intent})";
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
            if (stepContext.Result is Song songToBePlayed)
            {
                string messageText;
                string spokenMessageText;
                Activity message;
                // Now we have all the song details call the music player.
                try
                {
                    _musicPlayer.Play(songToBePlayed);

                    // If the call to the music player service was successful tell the user.
                    (messageText, spokenMessageText) = SentenceGenerator.PresentSongToBePlayed(songToBePlayed);
                    message = MessageFactory.Text(messageText, spokenMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(message, cancellationToken);
                } catch(Exception e)
                {
                    messageText = e.ToString();
                    spokenMessageText = "Sorry, something went wrong, so I cannot play the song.";
                    message = MessageFactory.Text(messageText, spokenMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(message, cancellationToken);
                }

            }

            // Restart the main dialog with a different message the second time around
            var (promptMessage, _) = SentenceGenerator.ProposeFurtherHelp();
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}
