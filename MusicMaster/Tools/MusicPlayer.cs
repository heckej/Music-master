using MusicMasterBot;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tools
{
    public class MusicPlayer
    {
        Song _currentSong;
        bool _playing;

        public void Play(Song song)
        {
            // play the song song.Title by song.Artist
            _currentSong = song;
            _playing = true;
        }

        public void Pause()
        {
            // pause current song
            _playing = false;
        }

        public void Resume()
        {
            // resume playing
            _playing = true;
        }
    }
}
