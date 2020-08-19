using MusicData;
using System;
using System.Collections.Generic;
using System.Linq;
using Metrics;
using System.Collections.Immutable;
using System.ComponentModel;
using MusicMasterBot;
using MusicMasterBot.CognitiveModels;
using Microsoft.EntityFrameworkCore.Storage;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace UserCommandLogic
{
    public class SongChooser : ISongChooser
    {
        /// input: detected intent, detected entities + values of entities (in Song object?)
        /// check whether known artist, title
        ///     if both unknown: add best matching songs (best matching artist, best matching title)
        ///     

        private IList<Song> _songs;
        public DatabaseConnector DatabaseConnector { get; set; }
        private static readonly Random _random = new Random();
        private ISet<string> _knownArtists;
        private ISet<string> _knownSongTitles;
        private IDictionary<string, ISet<Song>> _artistsToSongs = new Dictionary<string, ISet<Song>>();

        private double _thresholdSimilarityRatio = 0.80;

        double ISongChooser.ThresholdSimilarityRatio { get => _thresholdSimilarityRatio; set => _thresholdSimilarityRatio = value; }

        public SongChooser()
        {
            _songs = new List<Song>();
            _knownArtists = new HashSet<string>();
            _knownSongTitles = new HashSet<string>();
        }

        public SongChooser(DatabaseConnector databaseConnector)
        {
            if (databaseConnector.SettingsHaveBeenSet)
                SetDatabaseConnection(databaseConnector);
            DatabaseConnector = databaseConnector;
        }

        public void SetDatabaseConnection(DatabaseConnector databaseConnector)
        {
            if (!databaseConnector.SettingsHaveBeenSet)
                throw new ArgumentException("DatabaseConnector settings have to be set.");
            try
            {
                _songs = databaseConnector.GetSongTable().Result.ToList();
                LinkArtistsToSongs();
                _knownArtists = databaseConnector.GetKnownArtists().Result;
                _knownSongTitles = databaseConnector.GetKnownSongs().Result;
            }
            catch (MySqlException e)
            {
                Debug.WriteLine("No connection established with database, so no songs have been loaded. Please rerun the code to try again:\n" + e.ToString());
                _songs = new List<Song>();
                _knownArtists = new HashSet<string>();
                _knownSongTitles = new HashSet<string>();
            }
            catch (AggregateException e)
            {
                Debug.WriteLine("No connection established with database, so no songs have been loaded. Please rerun the code to try again:\n" + e.ToString());
                _songs = new List<Song>();
                _knownArtists = new HashSet<string>();
                _knownSongTitles = new HashSet<string>();
            }
            DatabaseConnector = databaseConnector;
        }

        public Song ChooseRandomSong()
        {
            int r = _random.Next(_songs.Count);
            return _songs.ElementAt(r);
        }

        public Song ChooseRandomSongByGenre(string genre)
        {
            return null;
        }

        public Song ChooseRandomSongByArtist(string artist)
        {
            var (bestMatch, _) = GetClosestKnownArtist(artist);
            if (bestMatch is null)
                return null;
            if (!IsKnownArtistWithSongs(artist))
                throw new ArgumentException("No songs known by artist " + artist);

            IList<Song> songsByArtist = _artistsToSongs[bestMatch].ToList();
            int r = _random.Next(songsByArtist.Count);
            return songsByArtist.ElementAt(r);
        }

        public Song ChooseByRequest(SongRequest songRequest)
        {
            Song song = null;
            switch (songRequest.Intent)
            {
                case UserCommand.Intent.PlayByTitle:
                    song = GetSongByClosestTitle(songRequest.Title);
                    break;
                case UserCommand.Intent.PlayByArtist:
                    song = ChooseRandomSongByArtist(GetClosestKnownArtist(songRequest.Artist).bestMatch);
                    break;
                case UserCommand.Intent.PlayByTitleArtist:
                    song = GetSongByClosestArtistOrTitle(songRequest.Title, songRequest.Artist);
                    break;
            }
            return song;
        }

        public Song GetSongData(string title, string artist)
        {
            if (IsKnownArtistWithSongs(artist))
                foreach (var song in _artistsToSongs[artist])
                    if (song.Title == title)
                        return song;
            return null;
        }

        public Song GetSongByClosestTitle(string title, string artist)
        {
            var closestSongs = GetAllSongsByClosestTitle(title);
            return SelectSongByClosestArtist(artist, closestSongs);
        }

        public Song GetSongByClosestTitle(string title)
        {
            return SelectSongByClosestTitle(title, _songs.ToImmutableHashSet());
        }

        private Song SelectSongByClosestArtist(string artist, ISet<Song> songs)
        {
            var artists = new HashSet<string>();
            foreach (var song in songs)
                artists.Add(song.Artist);
            var (bestMatch, similarityRatio) = LevenshteinDistance.GetBestMatch(artist, artists, true);
            if (similarityRatio >= _thresholdSimilarityRatio)
                foreach (var song in songs)
                    if (song.Artist == bestMatch)
                        return song;
            return null;
        }

        public ISet<Song> GetAllSongsByClosestTitle(string title)
        {
            return GetAllSongsByClosestTitle(title, _knownSongTitles);
        }

        public ISet<Song> GetAllSongsByClosestTitle(string title, ISet<string> setOfTitles, double ratio=0.80)
        {
            var results = new HashSet<Song>();
            var titleMatches = LevenshteinDistance.GetAllBestMatches(title, setOfTitles, ratio, true);
            foreach (var song in _songs)
            {
                foreach (var (songTitle, _) in titleMatches)
                    if (song.Title == songTitle)
                        results.Add(song);
            }
            return results;
        }

        public Song GetSongByClosestArtist(string title, string artist)
        {
            var (closestArtist, similarityRatio) = GetClosestKnownArtist(artist);
            if (similarityRatio < _thresholdSimilarityRatio)
                return null;
            var songsByArtist = _artistsToSongs[closestArtist];
            return SelectSongByClosestTitle(title, songsByArtist);
        }

        private Song SelectSongByClosestTitle(string title, ISet<Song> songs)
        {
            var titles = new HashSet<string>();
            foreach (var song in songs)
                titles.Add(song.Title);
            var (bestMatch, similarityRatio) = LevenshteinDistance.GetBestMatch(title, titles, true);
            if (similarityRatio >= _thresholdSimilarityRatio)
                foreach (var song in songs)
                    if (song.Title == bestMatch)
                        return song;
            return null;
        }

        public Song GetSongByClosestArtistOrTitle(string title, string artist)
        {
            var (bestMatchArtist, similarityRatioArtist) = GetClosestKnownArtist(artist, _thresholdSimilarityRatio);
            var (bestMatchTitle, similarityRatioTitle) = GetClosestKnownSongTitle(title, _thresholdSimilarityRatio);
            if (similarityRatioArtist >= similarityRatioTitle)
                return GetSongByClosestArtist(title, bestMatchArtist);
            return GetSongByClosestTitle(bestMatchTitle, artist);
        }

        public (string bestMatch, double similarityRatio) GetClosestKnownArtist(string artist)
        {
            if (_knownArtists.Contains(artist))
                return (artist, 1.0);
            return LevenshteinDistance.GetBestMatch(artist, _knownArtists, true);
        }

        public (string bestMatch, double similarityRatio) GetClosestKnownArtist(string artist, double threshold)
        {
            var (bestMatch, similarityRatio) = GetClosestKnownArtist(artist);
            if (similarityRatio >= threshold)
                return (bestMatch, similarityRatio);
            return (null, 0);
        }

        public (string bestMatch, double similarityRatio) GetClosestKnownSongTitle(string songTitle)
        {
            return LevenshteinDistance.GetBestMatch(songTitle, _knownSongTitles, true);
        }

        public (string bestMatch, double similarityRatio) GetClosestKnownSongTitle(string songTitle, double threshold)
        {
            var (bestMatch, similarityRatio) = GetClosestKnownArtist(songTitle);
            if (similarityRatio >= threshold)
                return (bestMatch, similarityRatio);
            return (null, 0);
        }

        private void LinkArtistsToSongs()
        {
            _artistsToSongs = new Dictionary<string, ISet<Song>>();
            foreach (var song in _songs)
            {
                if (!_artistsToSongs.ContainsKey(song.Artist))
                    _artistsToSongs.Add(song.Artist, new HashSet<Song>());
                _artistsToSongs[song.Artist].Add(song);
            }
        }

        private ISet<string> ConvertSongsToTitles(ISet<Song> songs)
        {
            var songTitles = new HashSet<string>();
            foreach (var song in songs)
                songTitles.Add(song.Title);
            return songTitles;
        }

        private void VerifyArtistHasSongs(string artist)
        {
            if (!_artistsToSongs.ContainsKey(artist))
                throw new ArgumentException("The given 'artist' must be a known artist.");
            if (_artistsToSongs[artist] == null || _artistsToSongs[artist].Count == 0)
                throw new Exception("There are no songs known for this artist.");
        }

        private bool IsKnownArtistWithSongs(string artist)
        {
            return _artistsToSongs.ContainsKey(artist) && _artistsToSongs[artist] != null && _artistsToSongs[artist].Count > 0;
        }

        public (string bestMatch, double similarityRatio) GetClosestKnownGenre(string genre)
        {
            throw new NotImplementedException();
        }

        public (string bestMatch, double similarityRatio) GetClosestKnownGenre(string genre, double threshold)
        {
            throw new NotImplementedException();
        }

        public Song ChooseByRequest(SongRequest songRequest)
        {
            Song song = null;
            switch (songRequest.Intent)
            {
                case UserCommand.Intent.PlayByTitle:
                    song = GetSongByClosestTitle(songRequest.Title);
                    break;
                case UserCommand.Intent.PlayByArtist:
                    song = ChooseRandomSongByArtist(songRequest.Artist);
                    break;
                case UserCommand.Intent.PlayByTitleArtist:
                    song = GetSongByClosestArtistOrTitle(songRequest.Title, songRequest.Artist);
                    break;
            }
            return song;
        }
    }
}
