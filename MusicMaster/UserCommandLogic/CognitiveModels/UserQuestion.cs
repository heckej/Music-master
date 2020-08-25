// <auto-generated>
// Code generated by LUISGen D:\Downloads\Music-master.json -cs Luis.UserQuestion -o D:\webdev\Music-master\MusicMaster
// Tool github: https://github.com/microsoft/botbuilder-tools
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
namespace MusicMasterBot.CognitiveModels
{
    public partial class UserQuestion: IRecognizerConvert
    {
        [JsonProperty("text")]
        public string Text;

        [JsonProperty("alteredText")]
        public string AlteredText;

        public enum Intent {
            CurrentSongInformationRequest,
            NegativeFeedback,
            PositiveFeedback,
            None
        };
        [JsonProperty("intents")]
        public Dictionary<Intent, IntentScore> Intents;

        public class _Entities
        {
            // Simple entities
            public string[] Negative;

            public string[] Positve;

            public string[][] SongDescription;

            public string[][] SongInformation;

            // Instance
            public class _Instance
            {
                public InstanceData[] Negative;
                public InstanceData[] Positve;
                public InstanceData[] SongDescription;
                public InstanceData[] SongInformation;
            }
            [JsonProperty("$instance")]
            public _Instance _instance;
        }
        [JsonProperty("entities")]
        public _Entities Entities;

        [JsonExtensionData(ReadData = true, WriteData = true)]
        public IDictionary<string, object> Properties {get; set; }

        public void Convert(dynamic result)
        {
            var app = JsonConvert.DeserializeObject<UserQuestion>(
                JsonConvert.SerializeObject(
                    result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            Text = app.Text;
            AlteredText = app.AlteredText;
            Intents = app.Intents;
            Entities = app.Entities;
            Properties = app.Properties;
        }

        public (Intent intent, double score) TopIntent()
        {
            Intent maxIntent = Intent.None;
            var max = 0.0;
            foreach (var entry in Intents)
            {
                if (entry.Value.Score > max)
                {
                    maxIntent = entry.Key;
                    max = entry.Value.Score.Value;
                }
            }
            return (maxIntent, max);
        }
    }
}