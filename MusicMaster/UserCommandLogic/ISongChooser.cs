using MusicData;
using MusicMasterBot;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserCommandLogic
{
    public interface ISongChooser
    {

        double ThresholdSimilarityRatio { get; set; }

        void SetDatabaseConnection(DatabaseConnector databaseConnector);

        Song ChooseRandomSong();

        Song ChooseRandomSongByGenre(string genre);

        Song ChooseRandomSongByArtist(string artist);

        Song GetSongData(string title, string artist);

        Song GetSongByClosestTitle(string title, string artist);

        Song GetSongByClosestTitle(string title);

        Song GetSongByClosestArtist(string title, string artist);

        Song GetSongByClosestArtistOrTitle(string title, string artist);

        ISet<Song> GetAllSongsByClosestTitle(string title);

        (string bestMatch, double similarityRatio) GetClosestKnownArtist(string artist);

        (string bestMatch, double similarityRatio) GetClosestKnownArtist(string artist, double threshold);

        (string bestMatch, double similarityRatio) GetClosestKnownSongTitle(string songTitle);

        (string bestMatch, double similarityRatio) GetClosestKnownSongTitle(string songTitle, double threshold);

        (string bestMatch, double similarityRatio) GetClosestKnownGenre(string genre);

        (string bestMatch, double similarityRatio) GetClosestKnownGenre(string genre, double threshold);

        Song ChooseByRequest(SongRequest songRequest);

        bool IsKnownArtist(string artist);

        bool IsKnownSongTitle(string song);

        ISet<string> GetKnownArtists();

        ISet<string> GetKnownSongTitles();

        IList<Song> GetSongs();

        IDictionary<string, ISet<Song>> GetArtistsToSongs();

        string GetKnownArtistFromSentence(string sentence);

        string GetKnownSongTitleFromSentence(string sentence);
    }
}
