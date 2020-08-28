// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.9.2

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Metrics;
using System.Security.Policy;
using MusicMasterBot.CognitiveModels;
using System.Linq;
using UserCommandLogic;
using Crunch.NET.Response.Ssml;
using System;

namespace MusicMasterBot.Dialogs
{
    public class RequestSongDialog : CancelAndHelpDialog
    {
        private readonly ISongChooser _songChooser;

        public RequestSongDialog(ISongChooser songChooser)
            : base(nameof(RequestSongDialog))
        {
            _songChooser = songChooser;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ArtistStepAsync,
                TitleStepAsync,
                ConfirmStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ArtistStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var songRequest = (SongRequest)stepContext.Options;
            if (songRequest.Intent == UserCommand.Intent.PlayByArtist || songRequest.Intent == UserCommand.Intent.PlayByTitleArtist)
            {
                var (bestMatchArtist, _) = _songChooser.GetClosestKnownArtist(songRequest.Artist, _songChooser.ThresholdSimilarityRatio);
                Console.WriteLine(songRequest.Artist + " => " + bestMatchArtist);

                if (bestMatchArtist is null)
                {
                    (bestMatchArtist, _) = _songChooser.GetClosestKnownArtist(songRequest.Artist);
                    Console.WriteLine(songRequest.Artist + " => " + bestMatchArtist);
                    var (messageText, spokenMessageText) = SentenceGenerator.AskArtistName(bestMatchArtist);
                    var promptMessage = MessageFactory.Text(messageText, spokenMessageText, InputHints.ExpectingInput);
                    return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
                }
            }

            return await stepContext.NextAsync(songRequest.Artist, cancellationToken);
        }

        private async Task<DialogTurnResult> TitleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var songRequest = (SongRequest)stepContext.Options;
            if (songRequest.Intent == UserCommand.Intent.PlayByArtist || songRequest.Intent == UserCommand.Intent.PlayByTitleArtist)
            {
                songRequest.Artist = (string)stepContext.Result;
                var knownArtistInResult = _songChooser.GetKnownArtistFromSentence(songRequest.Artist);

                if (!(knownArtistInResult is null))
                    songRequest.Artist = knownArtistInResult;
                else
                {
                    knownArtistInResult = _songChooser.ExpandSentenceToKnownArtist(songRequest.Artist);
                    if (!(knownArtistInResult is null))
                        songRequest.Artist = knownArtistInResult;
                }

                /*var (bestMatchArtist, _) = _songChooser.GetClosestKnownArtist(songRequest.Artist, _songChooser.ThresholdSimilarityRatio);
                if (!(bestMatchArtist is null))
                    songRequest.Artist = bestMatchArtist;*/
            }
            if (songRequest.Intent == UserCommand.Intent.PlayByTitle || songRequest.Intent == UserCommand.Intent.PlayByTitleArtist)
            {
                var (bestMatchTitle, _) = _songChooser.GetClosestKnownSongTitle(songRequest.Title, _songChooser.ThresholdSimilarityRatio);
                Console.WriteLine(songRequest.Title + " => " + bestMatchTitle);
                if (bestMatchTitle is null)
                    bestMatchTitle = _songChooser.GetKnownSongTitleFromSentence(songRequest.Title);
                if (bestMatchTitle is null)
                {
                    (bestMatchTitle, _) = _songChooser.GetClosestKnownSongTitle(songRequest.Title);
                    Console.WriteLine(songRequest.Title + " => " + bestMatchTitle);
                    var (messageText, spokenMessageText) = SentenceGenerator.AskSongTitle(bestMatchTitle);
                    var promptMessage = MessageFactory.Text(messageText, spokenMessageText, InputHints.ExpectingInput);
                    return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
                }
            }
            
            return await stepContext.NextAsync(songRequest.Title, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var songRequest = (SongRequest)stepContext.Options;

            if (songRequest.Intent == UserCommand.Intent.PlayByTitle || songRequest.Intent == UserCommand.Intent.PlayByTitleArtist)
            {
                songRequest.Title = (string)stepContext.Result;
                var knownSongTitle = _songChooser.GetKnownSongTitleFromSentence(songRequest.Title);
                Console.WriteLine(songRequest.Title + " => " + knownSongTitle);

                if (!(knownSongTitle is null) && !(_songChooser.GetSongByClosestTitle(knownSongTitle, songRequest.Artist) is null))
                    songRequest.Title = knownSongTitle;
                else
                {
                    knownSongTitle = _songChooser.ExpandSentenceToKnownSongTitle(songRequest.Title);
                    Console.WriteLine(songRequest.Title + " => " + knownSongTitle);
                    if (!(knownSongTitle is null))
                        songRequest.Title = knownSongTitle;
                }

                /*var (bestMatchTitle, _) = _songChooser.GetClosestKnownSongTitle(songRequest.Title);
                if (!(bestMatchTitle is null))
                    songRequest.Title = bestMatchTitle;*/
            }

            string messageText = "";
            string spokenMessageText =  "";

            var (bestMatchTitle, _) = _songChooser.GetClosestKnownSongTitle(songRequest.Title, _songChooser.ThresholdSimilarityRatio);
            Console.WriteLine(songRequest.Title + " => " + bestMatchTitle);

            var (bestMatchArtist, _) = _songChooser.GetClosestKnownArtist(songRequest.Artist, _songChooser.ThresholdSimilarityRatio);
            Console.WriteLine(songRequest.Artist + " => " + bestMatchArtist);

            if (bestMatchArtist is null && (songRequest.Intent is UserCommand.Intent.PlayByArtist || songRequest.Intent is UserCommand.Intent.PlayByTitleArtist))
            {
                var (messageText1, spokenMessageText1) = SentenceGenerator.UnknownArtist(songRequest.Artist);
                messageText += messageText1 + " ";
                spokenMessageText += spokenMessageText1 + " ";
            }
                
            if (bestMatchTitle is null && (songRequest.Intent is UserCommand.Intent.PlayByTitle || songRequest.Intent is UserCommand.Intent.PlayByTitleArtist))
            {
                var (messageText1, spokenMessageText1) = SentenceGenerator.UnknownSongTitle(songRequest.Title);
                messageText += messageText1;
                spokenMessageText += spokenMessageText1;
            }

            var message = MessageFactory.Text(messageText, spokenMessageText, InputHints.IgnoringInput);
            await stepContext.Context.SendActivityAsync(message, cancellationToken);

            return await stepContext.NextAsync(songRequest, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var songRequest = (SongRequest)stepContext.Result;
            var songDetails = _songChooser.ChooseByRequest(songRequest);
            return await stepContext.EndDialogAsync(songDetails, cancellationToken);
        }
    }
}
