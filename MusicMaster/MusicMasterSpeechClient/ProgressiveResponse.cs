﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Crunch.NET.Request;
using Crunch.NET.Response.Directive;
using Newtonsoft.Json.Linq;

namespace Crunch.NET.Response
{
    public class ProgressiveResponse
    {
        public HttpClient Client { get; set; }
        public ProgressiveResponseHeader Header { get; set; }

        public static bool IsSupported(CrunchRequest request)
        {
            return !string.IsNullOrWhiteSpace(request?.Request) &&
                   !string.IsNullOrWhiteSpace(request.Context);
        }


        public ProgressiveResponse(CrunchRequest request) : this(request, new HttpClient())
        {
            
        }

        public ProgressiveResponse(CrunchRequest request,HttpClient client) : this(
            request?.Request,
            request?.Context,
            request?.Context,client)
        {
        }

        public ProgressiveResponse(string requestId, string authToken, string baseAddress):this(requestId,authToken,baseAddress,new HttpClient())
        {

        }

        public ProgressiveResponse(string requestId, string authToken,string baseAddress, HttpClient client)
        {
            Client = client;
            if (!string.IsNullOrWhiteSpace(baseAddress))
            {
                Client.BaseAddress = new Uri(baseAddress, UriKind.Absolute);
            }

            if (!string.IsNullOrWhiteSpace(authToken))
            {
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }

            if (!string.IsNullOrWhiteSpace(requestId))
            {
                Header = new ProgressiveResponseHeader(requestId);
            }
        }

        public ProgressiveResponse(ProgressiveResponseHeader header,HttpClient client)
        {
            Client = client;
            Header = header;
        }

        public ProgressiveResponse()
        {

        }

        public Task<HttpResponseMessage> SendSpeech(Ssml.Speech ssml)
        {
            return Send(new VoicePlayerSpeakDirective(ssml));
        }

        public Task<HttpResponseMessage> SendSpeech(string ssml)
        {
            return Send(new VoicePlayerSpeakDirective(ssml));
        }

        public bool CanSend()
        {
            return Header != null && Client != null;
        }

        public Task<HttpResponseMessage> Send(IProgressiveResponseDirective directive)
        {
            if (directive == null || !CanSend())
            {
                return Task.FromResult((HttpResponseMessage)null);
            }

            var request = new ProgressiveResponseRequest
            {
                Header = Header,
                Directive = directive
            };
            var json = JObject.FromObject(request).ToString();
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            return Client.PostAsync(new Uri("/v1/directives", UriKind.Relative), httpContent);
        }
    }
}