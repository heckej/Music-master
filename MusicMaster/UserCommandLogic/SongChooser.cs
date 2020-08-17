using System;
using System.Collections.Generic;

namespace UserCommandLogic
{
    public class SongChooser
    {
        /// input: detected intent, detected entities + values of entities (in Song object?)
        /// check whether known artist, title
        ///     if both unknown: add best matching songs (best matching artist, best matching title)
        ///     

        public Song ChooseRandomSong()
        {
            return new Song();
        }

        public Song ChooseSongByGenre(string genre)
        {
            return new Song();
        }

        public Song ChooseSongByArtist(string artist)
        {
            return new Song();
        }

        public Song GetSongData(string title, string artist)
        {
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
