using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserCommandLogic;

namespace MusicData
{
    public class DatabaseConnector
    {
        MySqlConnectionStringBuilder builder;

        public bool SettingsHaveBeenSet { get; private set; } = false;

        public DatabaseConnector(string server, string database, string userID, string password)
        {
            builder = new MySqlConnectionStringBuilder
            {
                Server = server,
                Database = database,
                UserID = userID,
                Password = password,
                SslMode = MySqlSslMode.None,
            };
            SettingsHaveBeenSet = true;
        }

        public DatabaseConnector()
        {

        }

        public void Setup(string server, string database, string userID, string password)
        {
            builder = new MySqlConnectionStringBuilder
            {
                Server = server,
                Database = database,
                UserID = userID,
                Password = password,
                SslMode = MySqlSslMode.None,
            };
            SettingsHaveBeenSet = true;
        }

        public async Task<ISet<Song>> GetSongTable()
        {
            var songs = new HashSet<Song>();
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                Console.WriteLine("Opening connection");
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM song;";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string album;
                            int year;
                            if (reader.GetValue(2) is DBNull)
                                album = null;
                            else
                                album = reader.GetString(2);
                            if (reader.GetValue(3) is DBNull)
                                year = 0;
                            else
                                year = reader.GetInt32(3);
                            var song = new Song
                            {
                                TitleInDatabase = true,
                                ArtistInDatabase = true,
                                Title = reader.GetString(0),
                                Artist = reader.GetString(1),
                                Album = album,
                                Year = year,
                                FilePath = reader.GetString(4)
                            };
                            songs.Add(song);
                        }
                    }
                }

                Console.WriteLine("Closing connection");
            }
            return songs;
        }

        public async Task<IDictionary<string, ISet<string>>> GetArtistsToSongs()
        {
            var artistToSongs = new Dictionary<string, ISet<string>>();
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                Console.WriteLine("Opening connection");
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT DISTINCT song_title, song_artist FROM song;";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (!artistToSongs.ContainsKey(reader.GetString(1)))
                                artistToSongs.Add(reader.GetString(1), new HashSet<string>());
                            artistToSongs[reader.GetString(1)].Add(reader.GetString(0));
                        }
                    }
                }

                Console.WriteLine("Closing connection");
            }
            return artistToSongs;
        }

        public async Task<Dictionary<string, string>> GetSongToFilePath()
        {
            var songToFilePath = new Dictionary<string, string>();
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                Console.WriteLine("Opening connection");
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT DISTINCT song_title, path_to_song_file FROM song;";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (!songToFilePath.ContainsKey(reader.GetString(0)))
                                songToFilePath.Add(reader.GetString(0), reader.GetString(1));
                        }
                    }
                }

                Console.WriteLine("Closing connection");
            }
            return songToFilePath;
        }

        public async Task<ISet<string>> GetKnownArtists()
        {
            var artists = new HashSet<string>();
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                Console.WriteLine("Opening connection");
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT DISTINCT song_artist FROM song;";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            artists.Add(reader.GetString(0));
                        }
                    }
                }

                Console.WriteLine("Closing connection");
            }
            return artists;
        }

        public async Task<ISet<string>> GetKnownSongs()
        {
            var songTitles = new HashSet<string>();
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                Console.WriteLine("Opening connection");
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT DISTINCT song_title FROM song;";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            songTitles.Add(reader.GetString(0));
                        }
                    }
                }

                Console.WriteLine("Closing connection");
            }
            return songTitles;
        }

        public async Task<ISet<string>> GetRegisteredSongPaths()
        {
            ISet<string> paths = new HashSet<string>();
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                Console.WriteLine("Opening connection");
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT file_path_song_file FROM song;";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine(string.Format(
                                "Reading from table=({0})",
                                reader.GetString(0)));
                            paths.Add(reader.GetString(0));
                        }
                    }
                }

                Console.WriteLine("Closing connection");
            }
            return paths;
        }
    }
}
