using NetCoreAudio.Interfaces;

namespace NetCoreAudio.Players
{
    /// <summary>
    /// https://github.com/mobiletechtracker/NetCoreAudio
    /// </summary>
    internal class LinuxPlayer : UnixPlayerBase, IPlayer
    {
        protected override string BashToolName
        {
            get
            {
                return "aplay";
            }
        }
    }
}