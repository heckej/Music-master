using System;
using System.Threading.Tasks;

namespace NetCoreAudio.Interfaces
{
    /// <summary>
    /// https://github.com/mobiletechtracker/NetCoreAudio
    /// </summary>
    public interface IPlayer
    {
        event EventHandler PlaybackFinished;

        bool Playing { get; }
        bool Paused { get; }

        Task Play(string fileName);
        Task Pause();
        Task Resume();
        Task Stop();
    }
}