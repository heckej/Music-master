using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Players
{
    class LinuxPlayer : IPlayer
    {
        string _cmus = "cmus-remote";

        public void Pause()
        {
            ExecuteCmus("-u");
        }

        public void Play(string fileName)
        {
            Stop();
            ExecuteCmus("-q " + "\"" + fileName + "\"");
            PlayNext();
        }

        public void PlayNext()
        {
            ExecuteCmus("-n");
            Resume();
            ExecuteCmus("-n");
        }

        public void PlayPrevious()
        {
            ExecuteCmus("-r");
            ExecuteCmus("-r");
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

        private void ExecuteCmus(string arg)
        {
            var res = new RunCmd().Run(_cmus, arg);
            /*if (res["debug"] != "" && res["debug"] != null)*/
               /* throw new Exception("Some debugging info: " + res["debug"] + "\nOutput: " + res["output"]);*/
            if (res["debug"].Contains("cmus is not running"))
            {
                throw new System.Exception("cmus not running.");
                res = new RunCmd().Run("cmus & \n", "");
                res = new RunCmd().Run(_cmus, arg);
            }
        }
    }
}
