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
        static string endPoint = "http://localhost:3978/api/crunch";
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        };

        static void Main(string[] args)
        {
            string userInput;
            string language;
            while (true)
            {
                // listen to mic input
                Console.WriteLine("Enter a command.");
                Console.Write(">> ");
                //userInput = Console.ReadLine();
                var listenResult = Tools.Voice.Listen();
                //if (!listenResult["debug"].Contains("ALSA lib"))
                    Console.WriteLine(listenResult["debug"]);
                var results = listenResult["output"].Split("\n");
                Console.WriteLine("output contains " + results.Length + " elements");
                foreach (var res in results)
                    Console.WriteLine(res);
                language = results[0];
                userInput = results[1];
                Console.WriteLine(userInput);
                // print input

                // process input
                var response = ProcessUserRequest(userInput, language).Result;

                /*if (!response.StartsWith("//"))
                    Tools.Voice.Speak(response);
                else
                    response = response.Substring(2);*/

                // say result out loud
                Console.WriteLine("<< " + response);

            }
        }

        private static async Task<string> ProcessUserRequest(string userRequest, string language="en")
        {
            if (language != "en")
                language = "nl";
            var request = new CrunchRequest()
            {
                Context = "none",
                Request = userRequest,
                Session = "abc123",
                Version = "1.0",
                Language = language
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
            try {
                return musicMasterResponse.Response.Reprompt.OutputSpeech.Text;
            } catch (Exception e) {
                Console.WriteLine(e);
                return "";
            }
        }
    }
}
