using Metrics;
using MusicData;
using MusicMasterBot;
using MusicMasterBot.CognitiveModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace UserCommandLogic
{
    public class SongChooser : ISongChooser
    {
        /// input: detected intent, detected entities + values of entities (in Song object?)
        /// check whether known artist, title
        ///     if both unknown: add best matching songs (best matching artist, best matching title)
        ///     

        private IList<Song> _songs;
        private static readonly Random _random = new Random();
        private ISet<string> _knownArtists;
        private ISet<string> _knownSongTitles;
        private IDictionary<string, ISet<Song>> _artistsToSongs = new Dictionary<string, ISet<Song>>();
        private double _thresholdSimilarityRatio = 0.80;
        double ISongChooser.ThresholdSimilarityRatio { get => _thresholdSimilarityRatio; set => _thresholdSimilarityRatio = value; }


        /// <summary>
        /// A database connector to be used by this song chooser to retrieve data from the database.
        /// </summary>
        public DatabaseConnector DatabaseConnector { get; set; }

        /// <summary>
        /// Initialisesa new song chooser partially.
        /// </summary>
        /// <remarks>The resulting song chooser object hasn't been initialised completely.
        /// To complete initialisation, set a database connector using <code>SetDatabaseConnection()</code>.</remarks>
        public SongChooser()
        {
            _songs = new List<Song>();
            _knownArtists = new HashSet<string>();
            _knownSongTitles = new HashSet<string>();
        }


        /// <summary>
        /// Initialisesa new song chooser with the given <paramref name="databaseConnector"/> as its database connector.
        /// </summary>
        /// <remarks>The resulting song chooser object may not be initialised completely, namely if the given <paramref name="databaseConnector"/>
        /// hasn't been set. To complete initialisation, set up the <paramref name="databaseConnector"/>.</remarks>
        public SongChooser(DatabaseConnector databaseConnector)
        {
            if (databaseConnector.SettingsHaveBeenSet)
                SetDatabaseConnection(databaseConnector);
            DatabaseConnector = databaseConnector;
        }


        /// <summary>
        /// Sets the given <paramref name="databaseConnector"/> as the <code>DatabaseConnector</code> of this song chooser.
        /// </summary>
        /// <param name="databaseConnector">The database connector to be set as the <code>DatabaseConnector</code> of this song chooser</param>
        /// <remarks>In case no connection could be established with the database, no data will be available from the </remarks>
        /// <exception cref="ArgumentException">The settings of the given <paramref name="databaseConnector"/> have not been set.</exception>
        /// <exception cref="MySqlException">An error occurred while contacting the database referenced by the given 
        /// <paramref name="databaseConnector"/>.</exception>
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
                throw;
            }
            catch (AggregateException e)
            {
                Debug.WriteLine("No connection established with database, so no songs have been loaded. Please rerun the code to try again:\n" + e.ToString());
                throw e.InnerException;
            }
            DatabaseConnector = databaseConnector;
        }


        /// <summary>
        /// Returns a song from the database, if any available.
        /// </summary>
        public Song ChooseRandomSong()
        {
            if (_songs.Count > 0)
            {
                int r = _random.Next(_songs.Count);
                return _songs.ElementAt(r);
            }
            return null;
        }


        /// <summary>
        /// Returns a song from the database that has the given <paramref name="genre"/> as one of its genres, if any available.
        /// </summary>
        /// <param name="genre">The genre to be used to choose a song</param>
        /// <exception cref="NotImplementedException">This method has not been implemented.</exception>
        public Song ChooseRandomSongByGenre(string genre)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Chooses a song from the database with a given <paramref name="artist"/> as its artist, if any available.
        /// </summary>
        /// <param name="artist"></param>
        /// <returns>If there is no closest known artist, 
        ///             then return null.
        ///          Else, return a random song by this artist.</returns>
        /// <exception cref="ArgumentException">There are no songs of this artist in the database.</exception>
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


        /// <summary>
        /// Chooses a song from the database based on a given <paramref name="songRequest"/>, if any available
        /// </summary>
        /// <param name="songRequest">A song request to be used to choose a song from the database.</param>
        /// <returns>If the Intent of the given <paramref name="songRequest"/> is <code>PlayByTitle</code>
        ///             then a song is chosen by the title closest to the title of this <paramref name="songRequest"/>,
        ///          Else if the Intent is <code>PlayByArtist</code>
        ///             then a random song is chosen by the artist of this <paramref name="songRequest"/>
        ///          Else if the Intent is <code>PlayByTitleArtist</code>
        ///             then a song is selected based on the artist or title closest to those of this <paramref name="songRequest"/>
        ///          Else 
        ///             return null
        /// </returns>
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


        /// <summary>
        /// Selects a song based on the given <paramref name="title"/> and <paramref name="artist"/>
        /// </summary>
        /// <param name="title">The title of the song to be returned</param>
        /// <param name="artist">The artist of the song to be returned</param>
        /// <returns>If the given <paramref name="title"/>-<paramref name="artist"/> pair belongs
        ///             to a song in the database
        ///                 return the related song
        ///          Else
        ///                 return null
        /// </returns>
        public Song GetSongData(string title, string artist)
        {
            if (IsKnownArtistWithSongs(artist))
                foreach (var song in _artistsToSongs[artist])
                    if (song.Title == title)
                        return song;
            return null;
        }


        /// <summary>
        /// Selects a song from the database with its title closest to the given <paramref name="title"/> and its artist
        /// closest to the given <paramref name="artist"/> (order of priority: title, artist)
        /// </summary>
        /// <param name="title">The title to be used to select a song</param>
        /// <param name="artist">The artist to be used to select a song</param>
        /// <returns>If the given <paramref name="artist"/> (or comparable name) has the given <paramref name="title"/> 
        /// (or a comparable title) among its songs
        ///             then that song is returned.
        ///          Else
        ///             return null
        /// </returns>
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

        public ISet<Song> GetAllSongsByClosestTitle(string title, ISet<string> setOfTitles, double ratio = 0.80)
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
            artist = StringComparator.PreprocessString(artist);
            var artists = StringComparator.PreprocessCollection((HashSet<string>)_knownArtists);
            var match = LevenshteinDistance.GetBestMatch(artist, artists, true);
            foreach (var artistName in _knownArtists)
                if (StringComparator.PreprocessString(artistName) == match.bestMatch)
                    return (artistName, match.levenshteinDistance);
            return (null, 0);
        }

        public (string bestMatch, double similarityRatio) GetClosestKnownArtist(string artist, double threshold)
        {
            var (bestMatch, similarityRatio) = GetClosestKnownArtist(artist);
            if (similarityRatio >= threshold)
                return (bestMatch, similarityRatio);
            return (null, 0);
        }

        public (string bestMatch, double similarityRatio) GetClosestKnownSongTitle(string title)
        {
            title = StringComparator.PreprocessString(title);
            var titles = StringComparator.PreprocessCollection((HashSet<string>)_knownSongTitles);
            var match = LevenshteinDistance.GetBestMatch(title, titles, true);
            foreach (var songTitle in _knownSongTitles)
                if (StringComparator.PreprocessString(songTitle) == match.bestMatch)
                    return (songTitle, match.levenshteinDistance);
            return (null, 0);
        }

        public (string bestMatch, double similarityRatio) GetClosestKnownSongTitle(string songTitle, double threshold)
        {
            if (_knownSongTitles.Contains(songTitle))
                return (songTitle, 1.0);
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
            return _artistsToSongs.ContainsKey(artist) && !(_artistsToSongs[artist] is null) && _artistsToSongs[artist].Count > 0;
        }

        public (string bestMatch, double similarityRatio) GetClosestKnownGenre(string genre)
        {
            throw new NotImplementedException();
        }

        public (string bestMatch, double similarityRatio) GetClosestKnownGenre(string genre, double threshold)
        {
            throw new NotImplementedException();
        }

        public bool IsKnownArtist(string artist)
        {
            return _knownArtists.Contains(artist);
        }

        public bool IsKnownSongTitle(string song)
        {
            return _knownSongTitles.Contains(song);
        }

        public ISet<string> GetKnownArtists()
        {
            return _knownArtists;
        }

        public ISet<string> GetKnownSongTitles()
        {
            return _knownSongTitles;
        }

        public IList<Song> GetSongs()
        {
            return _songs;
        }

        public IDictionary<string, ISet<Song>> GetArtistsToSongs()
        {
            return _artistsToSongs;
        }

        public string GetKnownArtistFromSentence(string sentence)
        {
            var sorted = from s in _knownArtists
                         orderby s.Length descending
                         select s;
            sentence = StringComparator.PreprocessString(sentence);
            foreach (var artist in sorted)
                if (!(sentence is null) && sentence.Contains(StringComparator.PreprocessString(artist)))
                {
                    Console.WriteLine("From sentence: " + artist);
                    return artist;
                }
            return null;
        }

        public string GetKnownSongTitleFromSentence(string sentence)
        {
            Console.Write("-> " + sentence);
            var sorted = from s in _knownSongTitles
                         orderby s.Length descending
                         select s;
            sentence = StringComparator.PreprocessString(sentence);
            Console.WriteLine(" =>  " + sentence);
            foreach (var songTitle in sorted)
                if (!(sentence is null) && sentence.Contains(StringComparator.PreprocessString(songTitle)))
                    return songTitle;
            return null;
        }

        public string ExpandSentenceToKnownArtist(string sentence)
        {
            return ExpandSentenceToKnownFromCollection(sentence, _knownArtists);
        }

        public string ExpandSentenceToKnownSongTitle(string sentence)
        {
            return ExpandSentenceToKnownFromCollection(sentence, _knownSongTitles);
        }

        private string ExpandSentenceToKnownFromCollection(string sentence, IEnumerable<string> collection)
        {
            var sorted = from s in collection
                         orderby s.Length ascending
                         select s;
            sentence = StringComparator.PreprocessString(sentence);
            foreach (var value in sorted)
            {
                var processedValue = StringComparator.PreprocessString(value);
                if (!(processedValue is null) && processedValue.Contains(sentence))
                {
                    return value;
                }
            }
            return null;
        }
    }
}
