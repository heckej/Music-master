using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserCommandLogic;

namespace Tools.Players
{
    class LinuxPlayer : IPlayer
    {
        readonly string _cmus = "cmus-remote";
        readonly string _statusStopped = "status stopped";
        readonly string _statusPlaying = "status playing";
        readonly string _statusPaused = "status paused";

        public void Pause()
        {
            ExecuteCmus("-u");
        }

        public void Play(Song song)
        {
            Play(song.FilePath);
        }

        public void Play(string fileName)
        {
            ExecuteCmus("-q " + "\"" + fileName + "\"");
            PlayNext();
        }

        public void PlayNext()
        {
            ExecuteCmus("-n");
            if (ExecuteCmus("-Q").Contains(_statusStopped))
                Resume();
        }

        public void PlayPrevious()
        {
            ExecuteCmus("-r");
            ExecuteCmus("-r");
            if (ExecuteCmus("-Q").Contains(_statusStopped))
                Resume();
        }

        public void Resume()
        {
            ExecuteCmus("-p");
        }

        public void Stop()
        {
            ExecuteCmus("-s");
        }

        public void VolumeDown(double percentage = 10)
        {
            ExecuteCmus("-v -" + percentage + "%");
        }

        public void VolumeUp(double percentage = 10)
        {
            ExecuteCmus("-v +" + percentage + "%");
        }

        private string ExecuteCmus(string arg)
        {
            var res = new RunCmd().Run(_cmus, arg);
            /*if (res["debug"] != "" && res["debug"] != null)*/
               /* throw new Exception("Some debugging info: " + res["debug"] + "\nOutput: " + res["output"]);*/
            if (res["debug"].Contains("cmus is not running"))
            {
                throw new System.Exception("cmus not running.");
                /*res = new RunCmd().Run("cmus & \n", "");
                res = new RunCmd().Run(_cmus, arg);*/
            }
            return res["output"];
        }
    }
}
