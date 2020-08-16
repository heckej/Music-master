using System;
using System.Collections.Generic;
using System.Text;
using Crunch.NET.ConnectionTasks;
using Newtonsoft.Json;

namespace Crunch.NET.Response.Directive
{
    public class CompleteTaskDirective:IDirective
    {
        public CompleteTaskDirective() { }

        public CompleteTaskDirective(int statusCode, string statusMessage)
        {
            Status = new ConnectionStatus(statusCode,statusMessage);
        }

        [JsonProperty("type")]
        public string Type => "Tasks.CompleteTask";

        [JsonProperty("status")]
        public ConnectionStatus Status { get; set; } 
    }
}
