﻿using NetCoreAudio.Interfaces;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace NetCoreAudio.Players
{
    /// <summary>
    /// https://github.com/mobiletechtracker/NetCoreAudio
    /// </summary>
    internal class WindowsPlayer : IPlayer
    {
        [DllImport("winmm.dll")]
        private static extern int mciSendString(string command, StringBuilder stringReturn, int returnLength, IntPtr hwndCallback);

        [DllImport("winmm.dll")]
        private static extern int mciGetErrorString(int errorCode, StringBuilder errorText, int errorTextSize);

        private Timer _playbackTimer;
        private Stopwatch _playStopwatch;

        private string _fileName;

        public event EventHandler PlaybackFinished;

        public bool Playing { get; private set; }
        public bool Paused { get; private set; }

        public Task Play(string fileName)
        {
            _fileName = fileName;
            _playbackTimer = new Timer();
            _playbackTimer.AutoReset = false;
            _playStopwatch = new Stopwatch();
            ExecuteMsiCommand("Close All");
            //ExecuteMsiCommand($"Open {_fileName} Type MPEGVideo Alias myDevice");
            ExecuteMsiCommand($"Status {_fileName} Length");
            ExecuteMsiCommand($"Play {_fileName}");
            Paused = false;
            Playing = true;
            _playbackTimer.Elapsed += HandlePlaybackFinished;
            _playbackTimer.Start();
            _playStopwatch.Start();

            return Task.CompletedTask;
        }

        public Task Pause()
        {
            if (Playing && !Paused)
            {
                ExecuteMsiCommand($"Pause {_fileName}");
                Paused = true;
                _playbackTimer.Stop();
                _playStopwatch.Stop();
                _playbackTimer.Interval -= _playStopwatch.ElapsedMilliseconds;
            }

            return Task.CompletedTask;
        }

        public Task Resume()
        {
            if (Playing && Paused)
            {
                ExecuteMsiCommand($"Resume {_fileName}");
                Paused = false;
                _playbackTimer.Start();
                _playStopwatch.Reset();
                _playStopwatch.Start();
            }
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            if (Playing)
            {
                ExecuteMsiCommand($"Stop {_fileName}");
                Playing = false;
                Paused = false;
                _playbackTimer.Stop();
                _playStopwatch.Stop();
            }
            return Task.CompletedTask;
        }

        private void MarkPlaybackAsStopped()
        {
            Playing = false;
            _playbackTimer.Stop();
            _playbackTimer.Elapsed -= HandlePlaybackFinished;
        }

        private void HandlePlaybackFinished(object sender, ElapsedEventArgs e)
        {
            Playing = false;
            PlaybackFinished?.Invoke(this, e);
            _playbackTimer.Dispose();
            _playbackTimer = null;
        }

        private Task ExecuteMsiCommand(string commandString)
        {
            var sb = new StringBuilder();

            var result = mciSendString(commandString, sb, 1024 * 1024, IntPtr.Zero);

            /*if (result == 263)
            {
                string openCommand = $"Open {_fileName} Type MPEGVideo Alias myDevice";
                sb = new StringBuilder();
                result = mciSendString(openCommand, sb, 1024 * 1024, IntPtr.Zero);
                sb = new StringBuilder();
                result = mciSendString(commandString, sb, 1024 * 1024, IntPtr.Zero);
            }*/
            if (result != 0)
            {
                var errorSb = new StringBuilder($"Error executing MCI command '{commandString}'. Error code: {result}.");
                var sb2 = new StringBuilder(128);

                mciGetErrorString(result, sb2, 128);
                errorSb.Append($" Message: {sb2.ToString()}");

                throw new Exception(errorSb.ToString());
            }

            if (commandString.ToLower().StartsWith("status") && int.TryParse(sb.ToString(), out var length))
                _playbackTimer.Interval = length;

            return Task.CompletedTask;
        }
    }
}