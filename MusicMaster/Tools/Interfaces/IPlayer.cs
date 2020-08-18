using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserCommandLogic;

namespace Tools
{
    public interface IPlayer
    {
        void Play(string fileName);
        void Play(Song song);
        void Pause();
        void Resume();
        void Stop();
        void VolumeUp(double percentage = 10);
        void VolumeDown(double percentage = 10);
        void PlayNext();
        void PlayPrevious();
    }
}
