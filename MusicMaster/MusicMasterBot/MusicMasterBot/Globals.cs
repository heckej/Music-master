using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserCommandLogic;

namespace MusicMasterBot
{
    public static class Globals
    {
        public static IDictionary<string, ISet<string>> ArtistsToSongs { get; set; }
        public static Dictionary<string, string> SongToFilePath { get; set; }

        public static ISet<string> KnownArtists { get; set; }
        public static ISet<string> KnownSongs { get; set; }
        public static IList<Song> Songs { get; set; }
        public static SongChooser SongChooser { get; set; }
    }
}
