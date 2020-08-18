using System;
using System.Collections.Generic;
using System.Text;
using UserCommandLogic;

namespace Tools.Players
{
    class WindowsPlayer : IPlayer
    {
        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Play(Song song)
        {
            Play(song.FilePath);
        }

        public void Play(string fileName)
        {
            throw new NotImplementedException();
        }

        public void PlayNext()
        {
            throw new NotImplementedException();
        }

        public void PlayPrevious()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void VolumeDown(double percentage = 10)
        {
            throw new NotImplementedException();
        }

        public void VolumeUp(double percentage = 10)
        {
            throw new NotImplementedException();
        }
    }
}
