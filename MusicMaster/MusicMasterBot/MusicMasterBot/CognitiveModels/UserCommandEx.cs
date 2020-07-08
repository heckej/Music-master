using System.Linq;

namespace MusicMasterBot.CognitiveModels
{
    // Extends the partial UserCommand class with methods and properties that simplify accessing entities in the luis results
    public partial class UserCommand
    {
        public string SongArtist
            => Entities.MusicArtist?.FirstOrDefault();

        public string SongTitle
            => Entities.MusicSongTitle?.FirstOrDefault();
    }
}
