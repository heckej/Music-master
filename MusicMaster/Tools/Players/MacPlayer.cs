using NetCoreAudio.Interfaces;

namespace NetCoreAudio.Players
{
    /// <summary>
    /// https://github.com/mobiletechtracker/NetCoreAudio
    /// </summary>
    internal class MacPlayer : UnixPlayerBase, IPlayer
    {
        protected override string BashToolName
        {
            get
            {
                return "afplay";
            }
        }
    }
}