using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserCommandLogic;

namespace MusicMasterBot
{
    public static class SentenceGenerator
    {
        private static readonly Random _random = new Random();

        public static (string written, string spoken) DescribeTitleArtistCurrentSong(Song song)
        {
            IList<(string, string)> sentences = new List<(string, string)>
            {
                ($"This song is {song.Title} by {song.Artist}.", $"This song is {song.Title} by {song.Artist}."),
                ($"You are currently listening to {song.Title} by {song.Artist}.", $"{song.Title}, by {song.Artist}."),
                ($"The current song is {song.Title} by {song.Artist}.", $"{song.Title}, by {song.Artist}.")
            };

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }


        public static (string written, string spoken) DescribeTitleCurrentSong(Song song)
        {
            IList<(string, string)> sentences = new List<(string, string)>
            {
                ($"This song is {song.Title}.", $"This song is {song.Title}."),
                ($"You are currently listening to {song.Title}.", $"{song.Title}."),
                ($"The current song is {song.Title}.", $"{song.Title}.")
            };

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }


        public static (string written, string spoken) DescribeArtistCurrentSong(Song song)
        {
            IList<(string, string)> sentences = new List<(string, string)>
            {
                ($"This song was made by {song.Artist}.", $"This song was made by {song.Artist}."),
                ($"You are currently listening to a song by {song.Artist}.", $"The current artist is {song.Artist}."),
                ($"The current song is by {song.Artist}.", $"{song.Artist}.")
            };

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }

        public static (string written, string spoken) DescribeAlbumYearCurrentSong(Song song)
        {
            IList<(string, string)> sentences;
            if (song.Year is 0)
            {
                sentences = new List<(string, string)>
                {
                    ($"I don't know from which album this song is.", $"I don't know from which album this song is."),
                    ($"I honestly wouldn't know.", $"I honestly wouldn't know."),
                    ($"To be honest, I have no idea.", $"To be honest, I have no idea."),
                    ($"That's a big secret to me.", $"That's a big secret to me."),
                    ($"I'd be really smart if I knew that.", "I'd be really smart if I knew that."),
                    ($"I'm sorry. Apparently I don't know that much.", $"I'm sorry. Apparently I don't know that much.")
                };
            }
            else
            {
                sentences = new List<(string, string)>
                {
                    ($"This song is from the album {song.Album}, made in {song.Year}.", $"This is a song from the album {song.Album}, from {song.Year}."),
                    ($"The current album is {song.Album}. It was published in {song.Year}.", $"The album is {song.Album}, from {song.Year}.")
                };
            }

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }



        public static (string written, string spoken) DescribeAlbumCurrentSong(Song song)
        {
            IList<(string, string)> sentences;
            if (song.Album is null)
            {
                sentences = new List<(string, string)>
                {
                    ($"I don't know from which album this song is.", $"I don't know from which album this song is."),
                    ($"I honestly wouldn't know.", $"I honestly wouldn't know."),
                    ($"To be honest, I have no idea.", $"To be honest, I have no idea."),
                    ($"That's a big secret to me.", $"That's a big secret to me."),
                    ($"I'd be really smart if I knew that.", "I'd be really smart if I knew that."),
                    ($"I'm sorry. Apparently I don't know that much.", $"I'm sorry. Apparently I don't know that much.")
                };
            }
            else
            {
                sentences = new List<(string, string)>
                {
                    ($"This song is from the album {song.Album}.", $"This is a song from the album {song.Album}."),
                    ($"The current album is {song.Album}.", $"The album is {song.Album}.")
                };
            }

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }



        public static (string written, string spoken) DescribeYearCurrentSong(Song song)
        {
            IList<(string, string)> sentences;
            if (song.Year == 0)
            {
                sentences = new List<(string, string)>
                {
                    ($"I don't know when this song has been made.", $"I don't know when this song has been made.."),
                    ($"I honestly wouldn't know.", $"I honestly wouldn't know."),
                    ($"To be honest, I have no idea.", $"To be honest, I have no idea."),
                    ($"That's a big secret to me.", $"That's a big secret to me."),
                    ($"I'd be really smart if I knew that.", "I'd be really smart if I knew that."),
                    ($"I'm sorry. Apparently I don't know that much.", $"I'm sorry. Apparently I don't know that much.")
                };
            }
            else
            {
                sentences = new List<(string, string)>
                {
                    ($"This song was made in {song.Year}.", $"This is a song from {song.Year}."),
                    ($"It was published in {song.Year}.", $"{song.Year}.")
                };
            }

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }

        public static (string written, string spoken) PresentSongToBePlayed(Song song)
        {
            IList<(string, string)> sentences = new List<(string, string)>
            {
                ($"I will now play {song.Title} by {song.Artist}.", $"I will now play {song.Title} by {song.Artist}."),
                ($"The next song you'll hear is {song.Title} by {song.Artist}.", $"The next song is {song.Title}, by {song.Artist}."),
                ($"The following song is called {song.Title} and was made by {song.Artist}.", $"The following song is {song.Title}, by {song.Artist}.")
            };

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }

        public static (string written, string spoken) CommandNotUnderstood()
        {
            IList<(string, string)> sentences = new List<(string, string)>
            {
                ($"I didn't understand that.", $"I didn't understand that."),
                ($"What do you mean?", $"What do you mean?"),
                ($"Sorry, I didn't get that.", $"Sorry, I didn't get that."),
                ($"Sorry, I don't know what you mean.", $"Sorry, I don't know what you mean."),
                ($"Could you please repeat that?", $"Could you please repeat that?"),
                ($"Could you rephrase that?", $"Could you rephrase that?"),
                ($"Maybe try to ask it differently?", $"Maybe try to ask it differently?"),
                ($"I don't know what you're talking about.", $"I don't know what you're talking about.")
            };

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }

        public static (string written, string spoken) ProposeFurtherHelp()
        {
            IList<(string, string)> sentences = new List<(string, string)>
            {
                ($"What else can I do for you?", $"What else can I do for you?"),
                ($"Is there anything else I can do?", $"Is there anything else I can do?"),
                ($"Anything else?", $"Anything else?"),
                ($"Can I still be of service?", $"Can I still be of service?"),
                ($"Shall I help you with something else?", $"Shall I help you with something else?"),
                ($"What shall I do next?", $"What shall I do next?"),
                ($"What do you want me to do next?", $"What do you want me to do next?"),
                ($"Music Master, at your service.", $"Music Master, at your service.")
            };

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }

        public static (string written, string spoken) AskArtistName(string titleGuess=null)
        {
            IList<(string, string)> sentences = new List<(string, string)>
            {
                ($"Who is the artist?", $"Who is the artist?"),
                ($"What is the name of the artist?", $"What is the name of the artist?"),
                ($"Which artist made the song?", $"Which artist made the song?"),
                ($"Which artist do you want to hear?", $"Which artist do you want to hear?"),
                ($"Who sings the song?", $"Who sings the song?"),
                ($"Which singer or band do you want to hear?", $"Which singer or band do you want to hear?")
            };

            int r = _random.Next(sentences.Count);
            var (sentenceWritten1, sentenceSpoken1) = sentences.ElementAt(r);
            var (sentenceWritten2, sentenceSpoken2) = SayGuess(titleGuess);
            return (sentenceWritten1 + " " + sentenceWritten2, sentenceSpoken1 + " " + sentenceSpoken2);
        }

        public static (string written, string spoken) AskSongTitle(string artistGuess=null)
        {
            IList<(string, string)> sentences = new List<(string, string)>
            {
                ($"What is the title of the song?", $"What is the title of the song?"),
                ($"Which song do you mean?", $"Which song do you mean?"),
                ($"What is the song called?", $"What is the song called?"),
                ($"Which song do you want to hear?", $"Which song do you want to hear?"),
                ($"What is it called?", $"What is it called?"),
                ($"What is the title?", $"What is the title?")
            };

            int r = _random.Next(sentences.Count);
            var (sentenceWritten1, sentenceSpoken1) = sentences.ElementAt(r);
            var (sentenceWritten2, sentenceSpoken2) = SayGuess(artistGuess);
            return (sentenceWritten1 + " " + sentenceWritten2, sentenceSpoken1 + " " + sentenceSpoken2);
        }

        public static (string written, string spoken) SayGuess(string guess=null)
        {
            var (guessSentenceWritten, guessSentenceWrittenSpoken) = ("", "");
            if (!(guess is null))
            {
                IList<(string, string)> guessSentences = new List<(string, string)>
                {
                    ($"I thought you said {guess}.", $"I thought you said {guess}."),
                    ($"I understood {guess}.", $"I understood {guess}."),
                    ($"I thought I heard {guess}.", $"I thought I heard {guess}."),
                    ($"I sounded like {guess}.", $"I sounded like {guess}.")
                };

                int r = _random.Next(guessSentences.Count);
                (guessSentenceWritten, guessSentenceWrittenSpoken) = guessSentences.ElementAt(r);
            }
            return (guessSentenceWritten, guessSentenceWrittenSpoken);
        }

        public static (string written, string spoken) UnknownArtist(string artist)
        {
            if (artist is null)
                return UnknownArtist();
            IList<(string, string)> sentences = new List<(string, string)>
            {
                ($"I don't know the artist {artist}.", $"I don't know the artist {artist}."),
                ($"I have no songs by {artist}.", $"I have no songs by {artist}."),
                ($"The artist {artist}? Never heard of.", $"The artist {artist}? Never heard of."),
                ($"I have never heard of the artist {artist}.", $"I have never heard of the artist {artist}."),
                ($"The name {artist} doesn't remind me of anything.", $"The name {artist} doesn't remind me of anything."),
                ($"Unfortunately I don't know anyone called {artist}.", $"Unfortunately I don't know anyone called {artist}.")
            };

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }

        public static (string written, string spoken) UnknownArtist()
        {
            IList<(string, string)> sentences = new List<(string, string)>
            {
                ($"I don't know that artist.", $"I don't know that artist."),
                ($"I have no songs by that artist.", $"I have no songs by that artist."),
                ($"That artist? Never heard of.", $"That artist? Never heard of."),
                ($"I have never heard of the artist you just said.", $"I have never heard of the artist you just said."),
                ($"The name of that artist doesn't remind me of anything.", $"The name of that artist doesn't remind me of anything."),
                ($"Unfortunately I don't know any artist called called like that.", $"Unfortunately I don't know any artist called called like that.")
            };

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }

        public static (string written, string spoken) UnknownSongTitle(string title)
        {
            if (title is null)
                return UnknownSongTitle();
            IList<(string, string)> sentences = new List<(string, string)>
            {
                ($"I don't know the song {title}.", $"I don't know the song {title}."),
                ($"{title} is not in my playlist.", $"{title} is not in my playlist."),
                ($"The song {title}? Never heard of.", $"The song {title}? Never heard of."),
                ($"I have never heard of the song {title}.", $"I have never heard of the song {title}."),
                ($"The song {title} doesn't remind me of anything.", $"The song {title} doesn't remind me of anything."),
                ($"Unfortunately I don't know any song called {title}.", $"Unfortunately I don't know any song called {title}.")
            };

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }

        public static (string written, string spoken) UnknownSongTitle()
        {
            IList<(string, string)> sentences = new List<(string, string)>
            {
                ($"I don't know that song.", $"I don't know that song."),
                ($"That song is not in my playlist.", $"That song is not in my playlist."),
                ($"That song? Never heard of.", $"That song? Never heard of."),
                ($"I have never heard of the song title you just said.", $"I have never heard of the song title you just said."),
                ($"The title of that song doesn't remind me of anything.", $"The title of that song doesn't remind me of anything."),
                ($"Unfortunately I don't know any song called called like that.", $"Unfortunately I don't know any song called called like that.")
            };

            int r = _random.Next(sentences.Count);
            return sentences.ElementAt(r);
        }
    }
}
