using Crunch.NET.Request;
using Crunch.NET.Response;
using Crunch.NET.Response.Ssml;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MusicMasterSpeechClient
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static readonly string endPoint = "http://localhost:3978/api/crunch";
        static readonly string mention1 = "music master";
        static readonly string mention2 = "musicmaster";
        static readonly double activeListeningTime = 5000;

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        };

        static void Main()
        {
            var response = ProcessUserRequest("").Result;
            Console.WriteLine("<< " + response);

            string userInput;
            double timeLeft = 0;
            IDictionary<string, string> listenResult;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            while (true)
            {
                timeLeft -= watch.ElapsedMilliseconds;

                // listen to mic input
                Console.Write("> ");
                /*userInput = Console.ReadLine();*/
                listenResult = Tools.Voice.Listen();
                userInput = listenResult["output"];
                Console.WriteLine(userInput);
                var firstTimeNotAccepted = true;

                while(userInput.Contains("not understood!") || (!userInput.ToLower().Contains(mention1) && !userInput.ToLower().Contains(mention2) && timeLeft <= 0))
                {
                    if (firstTimeNotAccepted && !userInput.Contains("not understood!"))
                    {
                        Console.WriteLine("<< Say \'Music Master\' to wake me.");
                        Console.Write("> ");
                        firstTimeNotAccepted = false;
                    } else if (!userInput.Contains("not understood!"))
                    {
                        Console.WriteLine(userInput);
                        Console.Write("> ");
                    }
                    
                    listenResult = Tools.Voice.Listen();
                    userInput = listenResult["output"];
                    timeLeft -= watch.ElapsedMilliseconds;
                }

                Console.WriteLine(userInput);
                Console.WriteLine("Debug info: " + listenResult["debug"]);

                // print input
                Console.WriteLine(">> " + userInput);

                var filteredUserInput = userInput.Replace(mention1, "").Replace(mention2, "");

                // process input
                if (!userInput.Contains("not understood!") && filteredUserInput.Replace(" ", "").Length != 0)
                {
                    response = ProcessUserRequest(userInput).Result;

                    if (response != null && !response.StartsWith("//"))
                    {
                        Tools.Voice.Speak(response);
                        Console.WriteLine("Speaking...");
                    }
                    if (response != null)
                        response = response.Replace("//", "");

                    // say result out loud
                    Console.WriteLine("<< " + response);
                }
                else
                {
                    Console.WriteLine("Not understood!");
                }
                timeLeft = activeListeningTime;
                watch.Restart();

            }
        }

        private static async Task<string> ProcessUserRequest(string userRequest)
        {
            var request = new CrunchRequest()
            {
                Context = "none",
                Request = userRequest,
                Session = "abc123",
                Version = "1.0"
            };
            var content = System.Text.Json.JsonSerializer.Serialize<CrunchRequest>(request);
            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            var httpRequest = new HttpRequestMessage
            {
                Content = httpContent,
                Method = HttpMethod.Post
            };
            httpRequest.Headers.Add("Authorization", "none");
            var response = await client.PostAsync(endPoint, httpContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            var musicMasterResponse =  JsonConvert.DeserializeObject<CrunchResponse>(responseContent, JsonSerializerSettings);

            return musicMasterResponse?.Response?.Reprompt?.OutputSpeech?.Text;
        }
    }
}
