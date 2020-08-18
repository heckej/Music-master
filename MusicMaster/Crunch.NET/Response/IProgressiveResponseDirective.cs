using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Crunch.NET.Response
{
    public interface IProgressiveResponseDirective
    {
        [JsonRequired]
        string Type { get; }
    }
}
