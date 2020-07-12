using System.Collections.Generic;
using static MusicMasterBot.CognitiveModels.UserCommand;

namespace MusicMasterBot
{
    public class SongRequest
    {
        public string Title { get; set; }

        public string Artist { get; set; }

        public string Album { get; set; }

        public int Year { get; set; }

        public string Genre { get; set; }

        public  Intent Intent { get; set; }
    }
}
