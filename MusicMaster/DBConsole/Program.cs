using MusicData;
using System;

namespace DBConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var con = new DatabaseConnector("localhost", "media", "media", "media");
            var songs = con.GetSongTable().Result;
            foreach(var song in songs)
            {
                Console.WriteLine(song.Title + " by " + song.Artist + ", located at " + song.FilePath);
            }
        }
    }
}
