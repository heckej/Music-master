using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Tools
{
    public class Voice
    {
        /// <see>https://www.c-sharpcorner.com/blogs/using-systemspeech-with-net-core-30</see>
        public static string Speak(string textToSpeech, bool wait = false)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return SpeakOnWindows(textToSpeech, wait);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return SpeakOnLinux(textToSpeech, wait);
            else
                throw new Exception("No implementation exists for the current OS");
        }

        private static string SpeakOnLinux(string textToSpeech, bool wait = false)
        {
            string param = "\"" + textToSpeech + "\"";
            return new RunCmd().RunPython3("/home/jvh/Music-master/MusicMaster/Tools/tts.py", param);
        }

        private static string SpeakOnWindows(string textToSpeech, bool wait = false)
        {
            /*string param = "\"" + textToSpeech + "\"";
            return new RunCmd().RunPython3("tts.py", param);*/
            // Command to execute PS  
            Execute($@"Add-Type -AssemblyName System.speech;  
            $speak = New-Object System.Speech.Synthesis.SpeechSynthesizer;                           
            $speak.Speak(""{textToSpeech}"");"); // Embedd text  

            void Execute(string command)
            {
                // create a temp file with .ps1 extension  
                var cFile = System.IO.Path.GetTempPath() + Guid.NewGuid() + ".ps1";

                //Write the .ps1  
                using var tw = new System.IO.StreamWriter(cFile, false, Encoding.UTF8);
                tw.Write(command);

                // Setup the PS  
                var start =
                    new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = "C:\\windows\\system32\\windowspowershell\\v1.0\\powershell.exe",  // CHUPA MICROSOFT 02-10-2019 23:45                    
                        LoadUserProfile = false,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Arguments = $"-executionpolicy bypass -File {cFile}",
                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                    };

                //Init the Process  
                var p = System.Diagnostics.Process.Start(start);
                // The wait may not work! :(  
                if (wait) p.WaitForExit();
            }
            return null;
        }

        public static string Listen()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return ListenOnLinux();
            else
                throw new Exception("No implementation exists for the current OS");
        }

        public static string ListenOnLinux()
        {
            return new RunCmd().RunPython3("/home/jvh/Music-master/MusicMaster/Tools/record.py", "");
        }

    }
}
