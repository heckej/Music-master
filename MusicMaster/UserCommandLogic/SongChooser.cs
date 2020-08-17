using System;
using System.Collections.Generic;
using System.Linq;

namespace UserCommandLogic
{
    public class SongChooser
    {
        /// input: detected intent, detected entities + values of entities (in Song object?)
        /// check whether known artist, title
        ///     if both unknown: add best matching songs (best matching artist, best matching title)
        ///     

        private IList<Song> _songs;
        public static Random _random = new Random();

        public SongChooser(IList<Song> songs)
        {
            _songs = songs;
        }

        public Song ChooseRandomSong()
        {
            int r = _random.Next(_songs.Count);
            return _songs.ElementAt(r);
        }

        public Song ChooseSongByGenre(string genre)
        {
            return new Song();
        }

        public Song ChooseSongByArtist(string artist)
        {
            IList<Song> songsByArtist = new List<Song>();
            foreach(var song in _songs)
            {
                if (song.Artist == artist)
                    songsByArtist.Add(song);
            }
            int r = _random.Next(songsByArtist.Count);
            return songsByArtist.ElementAt(r);
        }

        public Song GetSongData(string title, string artist)
        {
            foreach(var song in _songs)
            {
                if (song.Artist == artist && song.Title == title)
                    return song;
            }
            return new Song();
        }

        public Song GetSongByClosestTitle(string title, string artist)
        {
            return new Song();
        }

        public ISet<Song> GetAllSongsByClosestTitle(string title)
        {
            return new HashSet<Song>();
        }

        public Song GetSongByClosestArtist(string title, string artist)
        {
            return new Song();
        }
    }
}
