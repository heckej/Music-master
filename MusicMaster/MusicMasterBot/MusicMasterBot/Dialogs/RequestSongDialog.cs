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

namespace MusicMasterBot.Dialogs
{
    public class RequestSongDialog : CancelAndHelpDialog
    {
        private const string ArtistStepMsgText = "Who is the artist?";
        private const string TitleStepMsgText = "What is the title of the song?";

        static string[] _artists = new string[] {
                "Adele", "Coldplay", "The Beatles", "U2", "The Police", "George Ezra", "Billy Joel", "The Rolling Stones", "Agnes Obel", "Ed Sheeran"
            };
        private ISet<string> KnownArtists = new HashSet<string>(_artists);

        static string[] _songs = new string[] {
                "Someone Like You", "Viva La Vida", "Let It Be", "Bloody Sunday", "Every Breath You Take", "Shotgun", "Piano man", "Angie", "Riverside", "Castle On The Hill"
            };
        private ISet<string> KnownSongs = new HashSet<string>(_songs);

        private IDictionary<string, ISet<string>> ArtistsToSongs = new Dictionary<string, ISet<string>>();


        private Dictionary<string, string> SongToFilePath = new Dictionary<string, string>();

        int _levDistBoundary = 10;
        double _levDistPercentage = 0.5;

        public RequestSongDialog()
            : base(nameof(RequestSongDialog))
        {
            for (int i=0; i<_artists.Length;i++)
            {
                ArtistsToSongs.Add(_artists[i], new HashSet<string>());
                ArtistsToSongs[_artists[i]].Add(_songs[i]);
            }

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
                var (bestMatchArtist, levDist) = GetBestMatch(songRequest.Artist, KnownArtists);
                if (bestMatchArtist != null && levDist < _levDistPercentage * bestMatchArtist.Length)
                    songRequest.Artist = bestMatchArtist;
                else
                {
                    var promptMessage = MessageFactory.Text(ArtistStepMsgText, ArtistStepMsgText, InputHints.ExpectingInput);
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
                var (bestMatchArtist, levDistArtist) = GetBestMatch(songRequest.Artist, KnownArtists);
                songRequest.Artist = bestMatchArtist;
            }
            if (songRequest.Intent == UserCommand.Intent.PlayByTitle || songRequest.Intent == UserCommand.Intent.PlayByTitleArtist)
            {
                var (bestMatchTitle, levDist) = GetBestMatch(songRequest.Title, KnownSongs);
                if (bestMatchTitle != null && levDist < _levDistBoundary && IsSongOfArtist(bestMatchTitle, songRequest.Artist))
                    songRequest.Title = bestMatchTitle;
                if (songRequest.Title == null || levDist > _levDistPercentage * bestMatchTitle.Length)
                {
                    var promptMessage = MessageFactory.Text(TitleStepMsgText, TitleStepMsgText, InputHints.ExpectingInput);
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
                var (bestMatchTitle, levDist) = GetBestMatch(songRequest.Title, KnownSongs);
                if (levDist < _levDistPercentage * bestMatchTitle.Length && IsSongOfArtist(bestMatchTitle, songRequest.Artist))
                    songRequest.Title = bestMatchTitle;
            }

            var messageText = $"Please confirm, I have you requesting: {songRequest.Title} by {songRequest.Artist}. Is this correct? ";

            if (!KnownArtists.Contains(songRequest.Artist))
                messageText += $"I don't know any artist called '{songRequest.Artist}'. ";
            if (!IsSongOfArtist(songRequest.Title, songRequest.Artist))
                messageText += $"I don't know the song '{songRequest.Title}' by '{songRequest.Artist}'.";


            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var songRequest = (SongRequest)stepContext.Options;
                
                // get song of the given genre
                var songDetails = (Song)

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
            foreach(string setValue in setToMatch)
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
            return songTitle == null || artist == null || (ArtistsToSongs.ContainsKey(artist) && ArtistsToSongs[artist].Contains(songTitle));
        }
    }
}
