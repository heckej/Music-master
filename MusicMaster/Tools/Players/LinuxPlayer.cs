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
            ExecuteCmus("-q " + fileName);
            ExecuteCmus("-n");
        }

        public void PlayNext()
        {
            ExecuteCmus("-n");
        }

        public void PlayPrevious()
        {
            ExecuteCmus("-r");
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
            if (res.Contains("cmus is not running"))
            {
                res = new RunCmd().Run("cmus & \n", "");
                res = new RunCmd().Run(_cmus, arg);
            }
        }
    }
}
