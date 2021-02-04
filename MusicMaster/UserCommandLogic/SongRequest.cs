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

        public Intent Intent { get; set; }

        private bool _titleInDatabase = false;
        private bool _artistInDatabase = false;
        private bool _genreInDatabase = false;
        private bool _albumInDatabase = false;

        public bool TitleInDatabase { get => _titleInDatabase; set => _titleInDatabase = value; }

        public bool ArtistInDatabase { get => _artistInDatabase; set => _artistInDatabase = value; }

        public bool GenreInDatabase { get => _genreInDatabase; set => _genreInDatabase = value; }

        public bool AlbumInDatabase { get => _albumInDatabase; set => _albumInDatabase = value; }
    }
}
