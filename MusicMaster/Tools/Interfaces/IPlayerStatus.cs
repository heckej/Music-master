using System;
using System.Collections.Generic;
using System.Text;
using UserCommandLogic;

namespace Tools
{
    public interface IPlayerStatus
    {
        Song GetCurrentSong();
        Song GetNextSong();
        Song GetPreviousSong();
        double GetCurrentPosition();
        Status GetStatus();
        (double volumeLeft, double volumeRight) GetVolume();
    }
}
