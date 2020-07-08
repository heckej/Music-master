// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.9.2

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

using MusicMasterBot;
using MusicMasterBot.CognitiveModels;

namespace MusicMasterBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly UserCommandRecognizer _luisRecognizer;
        protected readonly ILogger Logger;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(UserCommandRecognizer luisRecognizer, RequestSongDialog bookingDialog, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;
            Logger = logger;

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
            var messageText = stepContext.Options?.ToString() ?? "What can I help you with today?\nSay something like \"Play Someone Like Me by Adele\"";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                // LUIS is not configured, we just run the RequestSongDialog path with an empty SongDetailsInstance.
                return await stepContext.BeginDialogAsync(nameof(RequestSongDialog), new SongDetails(), cancellationToken);
            }

            // Call LUIS and gather any potential song details. (Note the TurnContext has the response to the prompt.)
            var luisResult = await _luisRecognizer.RecognizeAsync<UserCommand>(stepContext.Context, cancellationToken);
            switch (luisResult.TopIntent().intent)
            {
                case UserCommand.Intent.PlayByTitleArtist:
                    await ShowWarningForUnknownSong(stepContext.Context, luisResult, cancellationToken);

                    // Initialize SongDetails with any entities we may have found in the response.
                    var songDetails = new SongDetails()
                    {
                        // Get title and artist from the entities.
                        Title = luisResult.SongTitle,
                        Artist = luisResult.SongArtist
                    };

                    // Run the BookingDialog giving it whatever details we have from the LUIS call, it will fill out the remainder.
                    return await stepContext.BeginDialogAsync(nameof(RequestSongDialog), songDetails, cancellationToken);

                case UserCommand.Intent.PlayRandom:
                    // We haven't implemented the PlayRandomSongDialog so we just display a TODO message.
                    var playRandomSongMessageText = "TODO: play random song here";
                    var playRandomSongMessage = MessageFactory.Text(playRandomSongMessageText, playRandomSongMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(playRandomSongMessage, cancellationToken);
                    break;

                case UserCommand.Intent.PreviousSong:
                    // We haven't implemented the PlayRandomSongDialog so we just display a TODO message.
                    var previousSongMessageText = "TODO: play previous song here";
                    var previousSongMessage = MessageFactory.Text(previousSongMessageText, previousSongMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(previousSongMessage, cancellationToken);
                    break;
                case UserCommand.Intent.NextSong:
                    // We haven't implemented the PlayRandomSongDialog so we just display a TODO message.
                    var nextSongMessageText = "TODO: play next song here";
                    var nextSongMessage = MessageFactory.Text(nextSongMessageText, nextSongMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(nextSongMessage, cancellationToken);
                    break;
                case UserCommand.Intent.StartPlaying:
                    // We haven't implemented the PlayRandomSongDialog so we just display a TODO message.
                    var startPlayingMessageText = "TODO: resume playing here";
                    var startPlayingMessage = MessageFactory.Text(startPlayingMessageText, startPlayingMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(startPlayingMessage, cancellationToken);
                    break;
                case UserCommand.Intent.StopPlaying:
                    // We haven't implemented the PlayRandomSongDialog so we just display a TODO message.
                    var stopPlayingMessageText = "TODO: stop playing here";
                    var stopPlayingMessage = MessageFactory.Text(stopPlayingMessageText, stopPlayingMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(stopPlayingMessage, cancellationToken);
                    break;
                case UserCommand.Intent.VolumeDown:
                    // We haven't implemented the PlayRandomSongDialog so we just display a TODO message.
                    var volumeDownMessageText = "TODO: lower volume here";
                    var volumeDownMessage = MessageFactory.Text(volumeDownMessageText, volumeDownMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(volumeDownMessage, cancellationToken);
                    break;
                case UserCommand.Intent.VolumeUp:
                    // We haven't implemented the PlayRandomSongDialog so we just display a TODO message.
                    var volumeUpMessageText = "TODO: increase volume here";
                    var volumeUpMessage = MessageFactory.Text(volumeUpMessageText, volumeUpMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(volumeUpMessage, cancellationToken);
                    break;

                default:
                    // Catch all for unhandled intents
                    var didntUnderstandMessageText = $"Sorry, I didn't get that. Please try asking in a different way (intent was {luisResult.TopIntent().intent})";
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        // Shows a warning if the requested MusicArtist or MusicSongTitle are recognized as entities but they are not in the entity list.
        private static async Task ShowWarningForUnknownSong(ITurnContext context, UserCommand luisResult, CancellationToken cancellationToken)
        {
            var unknownkArtists = new List<string>();
            var unknownSongTitles = new List<string>();

            if (unknownkArtists.Any() || unknownSongTitles.Any())
            {
                var messageText = $"Sorry but the following artists are not known: {string.Join(',', unknownkArtists)}";
                messageText += $"Sorry but the following song titles are not known: {string.Join(',', unknownSongTitles)}";
                var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await context.SendActivityAsync(message, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // If the child dialog ("RequestSongDialog") was cancelled, the user failed to confirm or if the intent wasn't PlayByTitleArtist
            // the Result here will be null.
            if (stepContext.Result is SongDetails result)
            {
                // Now we have all the song details call the music player.

                // If the call to the music player service was successful tell the user.

                var messageText = $"I will now play {result.Title} by {result.Artist}.";
                var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);
            }

            // Restart the main dialog with a different message the second time around
            var promptMessage = "What else can I do for you?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}
