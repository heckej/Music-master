using System.Collections.Generic;

namespace UserCommandLogic
{
    public class Song
    {
        public string Title { get; set; }

        public string Artist { get; set; }

        public string Album { get; set; }

        public int Year { get; set; }

        public ISet<string> Genres { get; set; }

        public string FilePath { get; set; }

        public ISet<Song> BestMatches { get; set; }

        public bool ArtistInDatabase { get; set; }

        public bool TitleInDatabase { get; set; }

        public void UpdateByDatabase()
        {
            /// find the song in the database based on title, artist; if not found, note the best matches for artist and title with their best matches for the other parameter respectively
        }
    }
}
