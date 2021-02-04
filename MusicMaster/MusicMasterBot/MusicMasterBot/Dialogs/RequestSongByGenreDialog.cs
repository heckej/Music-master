// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.9.2

using Metrics;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using MusicMasterBot.CognitiveModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MusicMasterBot.Dialogs
{
    public class RequestSongByGenreDialog : CancelAndHelpDialog
    {
        private const string GenreStepMsgText = "What is the requested genre?";

        static string[] _genres = new string[] {
                "Adele", "Coldplay", "The Beatles", "U2", "The Police", "George Ezra", "Billy Joel", "The Rolling Stones", "Agnes Obel", "Ed Sheeran"
            };
        private ISet<string> KnownGenres = new HashSet<string>(_genres);

        static string[] _songs = new string[] {
                "Someone Like You", "Viva La Vida", "Let It Be", "Bloody Sunday", "Every Breath You Take", "Shotgun", "Piano man", "Angie", "Riverside", "Castle On The Hill"
            };

        private IDictionary<string, ISet<string>> GenresToSongs = new Dictionary<string, ISet<string>>();


        private Dictionary<string, string> SongToFilePath = new Dictionary<string, string>();
        readonly double _levDistPercentage = 0.5;

        public RequestSongByGenreDialog()
            : base(nameof(RequestSongByGenreDialog))
        {
            for (int i = 0; i < _genres.Length; i++)
            {
                GenresToSongs.Add(_genres[i], new HashSet<string>());
                GenresToSongs[_genres[i]].Add(_songs[i]);
            }

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                GenreStepAsync,
                ConfirmStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> GenreStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var songRequest = (SongRequest)stepContext.Options;

            var (bestMatchGenre, levDist) = GetBestMatch(songRequest.Genre, KnownGenres);
            if (bestMatchGenre != null && levDist < _levDistPercentage * bestMatchGenre.Length)
                songRequest.Title = bestMatchGenre;
            else
            {
                var promptMessage = MessageFactory.Text(GenreStepMsgText, GenreStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(songRequest.Title, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var songRequest = (SongRequest)stepContext.Options;

            if (songRequest.Intent == UserCommand.Intent.PlayByGenre)
            {
                songRequest.Genre = (string)stepContext.Result;
                var (bestMatchGenre, levDist) = GetBestMatch(songRequest.Genre, KnownGenres);
                if (levDist < _levDistPercentage * bestMatchGenre.Length)
                    songRequest.Genre = bestMatchGenre;
            }

            var messageText = $"Please confirm, I have you requesting a song of the genre '{songRequest.Genre}'. Is this correct? ";

            if (!KnownGenres.Contains(songRequest.Genre))
                messageText += $"I don't know the genre '{songRequest.Genre}'. ";


            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var songDetails = (SongRequest)stepContext.Options;

                return await stepContext.EndDialogAsync(songDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private (string bestMatch, int levenshteinDistance) GetBestMatch(string value, ISet<string> setToMatch)
        {
            if (value == null)
                return (null, 0);
            value = value.ToLower();
            string bestMatch = null;
            int bestLevDist = int.MaxValue;
            int currentLevDist;
            foreach (string setValue in setToMatch)
            {
                if (value == setValue.ToLower())
                    return (setValue, 0);
                currentLevDist = LevenshteinDistance.Compute(value, setValue.ToLower());
                if (setValue.ToLower().Contains(value))
                    currentLevDist -= value.Length;
                else if (value.Contains(setValue.ToLower()))
                    currentLevDist -= setValue.Length;
                if (currentLevDist < bestLevDist)
                {
                    bestLevDist = currentLevDist;
                    bestMatch = setValue;
                }
            }
            return (bestMatch, bestLevDist);
        }

        private bool IsSongOfArtist(string songTitle, string artist)
        {
            return songTitle == null || artist == null || (GenresToSongs.ContainsKey(artist) && GenresToSongs[artist].Contains(songTitle));
        }
    }
}
