using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserCommandLogic;

namespace Tools
{
    public class CmusStatus : IPlayerStatus
    {
        private string _statusString;
        private IDictionary<string, string> _status;
        private static readonly ISet<string> _twoArgumentStatus = InitializeTwoArgumentStatus();

        private static ISet<string> InitializeTwoArgumentStatus()
        {
            return new HashSet<string>()
            {
                "tag", "set"
            };
        }

        private void UpdateStatus()
        {
            var newStatusString = ExecuteCmus("-Q");
            if (newStatusString != _statusString)
            {
                _statusString = newStatusString;
                _status = ParseCmusStatus(_statusString); // avoid redoing some work
            }
        }

        public static IDictionary<string, string> ParseCmusStatus(string statusString)
        {
            var status = new Dictionary<string, string>();
            var statusLines = statusString.Replace("\r", "").Split('\n');
            foreach (var line in statusLines)
            {
                var splittedLine = line.Split(new char[] { ' ' }, 2);
                if (_twoArgumentStatus.Contains(splittedLine[0]))
                    splittedLine = splittedLine[1].Split(new char[] { ' ' }, 2);
                if (splittedLine.Length > 1)
                    status.Add(splittedLine[0], splittedLine[1].Trim());
            }
            return status;
        }

        public double GetCurrentPosition()
        {
            UpdateStatus();
            return Double.Parse(_status["position"]);
        }

        public Song GetCurrentSong()
        {
            UpdateStatus();
            return new Song()
            {
                Title = _status["title"],
                Artist = _status["artist"],
                Album = _status["album"],
                Year = Int32.Parse(_status["date"]),
                Genres = new HashSet<string>()
                {
                    _status["genre"]
                },
                FilePath = _status["file"]
            };
        }

        public Song GetNextSong()
        {
            throw new NotImplementedException();
        }

        public Song GetPreviousSong()
        {
            throw new NotImplementedException();
        }

        public Status GetStatus()
        {
            UpdateStatus();
            return (_status["status"]) switch
            {
                "stopped" => Status.Stopped,
                "playing" => Status.Playing,
                "paused" => Status.Paused,
                _ => Status.None,
            };
        }

        private string ExecuteCmus(string arg)
        {
            var res = new RunCmd().Run("cmus-remote", arg);
            if (res["debug"].Contains("cmus is not running"))
            {
                throw new System.Exception("cmus not running.");
            }
            return res["output"];
        }

        public (double volumeLeft, double volumeRight) GetVolume()
        {
            UpdateStatus();
            double volumeLeft = 0;
            double volumeRight = 0;
            if (_status.ContainsKey("vol_left"))
                volumeLeft = Double.Parse(_status["vol_left"]);
            if (_status.ContainsKey("vol_right"))
                volumeRight = Double.Parse(_status["vol_right"]);
            return (volumeLeft, volumeRight);
        }
    }
}
