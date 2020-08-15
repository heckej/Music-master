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

namespace MusicMasterBot.Dialogs
{
    public class RequestSongDialog : CancelAndHelpDialog
    {
        private const string ArtistStepMsgText = "Who is the artist?";
        private const string TitleStepMsgText = "What is the title of the song?";

        private IDictionary<string, ISet<string>> ArtistsToSongs = new Dictionary<string, ISet<string>>();
        private Dictionary<string, string> SongToFilePath = new Dictionary<string, string>();

        private ISet<string> KnownArtists;
        private ISet<string> KnownSongs;

        int _levDistBoundary = 10;
        double _levDistPercentage = 0.5;

        public RequestSongDialog()
            : base(nameof(RequestSongDialog))
        {

            SongToFilePath.Add("Someone Like You", "/home/jvh/Muziek/Adele/21 [Australian Bonus Track Edition]/Adele Someone Like You.wma");
            SongToFilePath.Add("Viva La Vida", "/home/jvh/Muziek/Coldplay/Viva la Vida/07 Viva la Vida.wma");
            SongToFilePath.Add("Let It Be", "/home/jvh/Muziek/The Beatles/The Beatles #1/The Beatles Let It Be.wma");
            SongToFilePath.Add("With Or Without You", "/home/jvh/Muziek/U2/U2 - With Or Without You.mp3");
            SongToFilePath.Add("Every Breath You Take", "/home/jvh/Muziek/The Police/The Police - Every Breath You Take.mp3");
            SongToFilePath.Add("Budapest", "/home/jvh/Muziek/George Ezra/George Ezra - Budapest (Official Video).mp3");
            SongToFilePath.Add("Angie", "/home/jvh/Muziek/The Rolling Stones/Rolling Stones - Angie.mp3");
            SongToFilePath.Add("Riverside", "/home/jvh/Muziek/Agnes Obel/Philharmonics/Agnes Obel Riverside.wma");

            ArtistsToSongs.Add("Adele", new HashSet<string>());
            ArtistsToSongs.Add("Coldplay", new HashSet<string>());
            ArtistsToSongs.Add("The Beatles", new HashSet<string>());
            ArtistsToSongs.Add("U2", new HashSet<string>());
            ArtistsToSongs.Add("The Police", new HashSet<string>());
            ArtistsToSongs.Add("George Ezra", new HashSet<string>());
            ArtistsToSongs.Add("The Rolling Stones", new HashSet<string>());
            ArtistsToSongs.Add("Agnes Obel", new HashSet<string>());

            ArtistsToSongs["Adele"].Add("Someone Like You");
            ArtistsToSongs["Coldplay"].Add("Viva La Vida");
            ArtistsToSongs["The Beatles"].Add("Let It Be");
            ArtistsToSongs["U2"].Add("With Or Without You");
            ArtistsToSongs["The Police"].Add("Every Breath You Take");
            ArtistsToSongs["George Ezra"].Add("Budapest");
            ArtistsToSongs["The Rolling Stones"].Add("Angie");
            ArtistsToSongs["Agnes Obel"].Add("Riverside");

            KnownArtists = ArtistsToSongs.Keys.ToHashSet();
            KnownSongs = SongToFilePath.Keys.ToHashSet();

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
                songRequest.Title = ArtistsToSongs[bestMatchArtist].First();
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
                {
                    songRequest.Title = bestMatchTitle;
                    songRequest.Artist = GetArtistOfSong(bestMatchTitle);
                }
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
                var songDetails = new Song()
                {
                    Title = songRequest.Title,
                    Artist = songRequest.Artist,
                    FilePath = SongToFilePath[songRequest.Title]
                };

                //return await stepContext.EndDialogAsync(songRequest, cancellationToken);
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
            return songTitle != null && artist != null && (ArtistsToSongs.ContainsKey(artist) && ArtistsToSongs[artist].Contains(songTitle));
        }

        private string GetArtistOfSong(string songTitle)
        {
            foreach (string artist in KnownArtists)
                if (IsSongOfArtist(songTitle, artist))
                    return artist;
            return null;
        }
    }
}
