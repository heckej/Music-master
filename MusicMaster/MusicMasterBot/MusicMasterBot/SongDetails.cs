using System.Collections.Generic;

namespace MusicMasterBot
{
    public class SongDetails
    {
        public string Title { get; set; }

        public string Artist { get; set; }

        public string Album { get; set; }

        public int Year { get; set; }

        public ISet<string> Genres { get; set; }
    }
}
