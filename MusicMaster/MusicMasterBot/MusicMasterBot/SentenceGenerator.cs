using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserCommandLogic;

namespace MusicMasterBot
{
    public static class SentenceGenerator
    {
        private static Random _random = new Random();

        public static (string written, string spoken) DescribeTitleArtistCurrentSong(Song song)
        {
            IList<(string, string)> sentences = new List<(string, string)>();
            sentences.Add(($"This song is {song.Title} by {song.Artist}.", $"This song is {song.Title} by {song.Artist}."));
            sentences.Add(($"You are currently listening to {song.Title} by {song.Artist}.", $"{song.Title}, by {song.Artist}."));
            sentences.Add(($"The current song is {song.Title} by {song.Artist}.", $"{song.Title}, by {song.Artist}."));

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }

        public static (string written, string spoken) DescribeAlbumYearCurrentSong(Song song)
        {
            IList<(string, string)> sentences = new List<(string, string)>();
            sentences.Add(($"This song is from the album {song.Album}, made in {song.Year}.", $"This is a song from the album {song.Album}, from {song.Year}."));
            sentences.Add(($"The current album is {song.Album}. It was published in {song.Year}", $"The album is {song.Album}, from {song.Year}."));

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }

        public static (string written, string spoken) PresentSongToBePlayed(Song song)
        {
            IList<(string, string)> sentences = new List<(string, string)>();
            sentences.Add(($"I will now play {song.Title} by {song.Artist}.", $"I will now play {song.Title} by {song.Artist}."));
            sentences.Add(($"The next song you'll hear is {song.Title} by {song.Artist}.", $"The next song is {song.Title}, by {song.Artist}."));
            sentences.Add(($"The following song is called {song.Title} and was made by {song.Artist}.", $"The following song is {song.Title}, by {song.Artist}."));

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }

        public static (string written, string spoken) CommandNotUnderstood()
        {
            IList<(string, string)> sentences = new List<(string, string)>();
            sentences.Add(($"I didn't understand that.", $"I didn't understand that."));
            sentences.Add(($"What do you mean?", $"What do you mean?"));
            sentences.Add(($"Sorry, I didn't get that.", $"Sorry, I didn't get that."));
            sentences.Add(($"Sorry, I don't know what you mean.", $"Sorry, I don't know what you mean."));
            sentences.Add(($"Could you please repeat that?", $"Could you please repeat that?"));
            sentences.Add(($"Could you rephrase that?", $"Could you rephrase that?"));
            sentences.Add(($"Maybe try to ask it differently?", $"Maybe try to ask it differently?"));
            sentences.Add(($"I don't know what you're talking about.", $"I don't know what you're talking about."));

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }

        public static (string written, string spoken) ProposeFurtherHelp()
        {
            IList<(string, string)> sentences = new List<(string, string)>();
            sentences.Add(($"What else can I do for you?", $"What else can I do for you?"));
            sentences.Add(($"Is there anything else I can do?", $"Is there anything else I can do?"));
            sentences.Add(($"Anything else?", $"Anything else?"));
            sentences.Add(($"Can I still be of service?", $"Can I still be of service?"));
            sentences.Add(($"Shall I help you with something else?", $"Shall I help you with something else?"));
            sentences.Add(($"What shall I do next?", $"What shall I do next?"));
            sentences.Add(($"What do you want me to do next?", $"What do you want me to do next?"));
            sentences.Add(($"Music Master, at your service.", $"Music Master, at your service."));

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }
    }
}
