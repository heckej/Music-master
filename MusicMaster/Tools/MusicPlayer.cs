using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Tools.Players;
using UserCommandLogic;

namespace Tools
{
    public class MusicPlayer : IPlayer
    {
        private Song _currentSong;
        private IPlayer _player;

        public MusicPlayer()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                _player = new LinuxPlayer();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _player = new WindowsPlayer();
            else
                throw new Exception("No implementation exists for the current OS");
        }

        public void Play(Song song)
        {
            // play the song song.Title by song.Artist
            _currentSong = song;
            Play(song.FilePath);
        }

        public void Pause()
        {
            // pause current song
            _player.Pause();
        }

        public void Resume()
        {
            // resume playing
            _player.Resume();
        }

        public void Play(string fileName)
        {
            _player.Play(fileName);
        }

        public void Stop()
        {
            _player.Stop();
        }

        public void VolumeUp(double percentage = 10)
        {
            _player.VolumeUp(percentage);
        }

        public void VolumeDown(double percentage = 10)
        {
            _player.VolumeDown(percentage);
        }

        public void PlayNext()
        {
            _player.PlayNext();
        }

        public void PlayPrevious()
        {
            _player.PlayPrevious();
        }

        public Song CurrentSong => _currentSong;
    }
}
