using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Crunch.NET.Response
{
    public interface IEndSessionDirective: IDirective
    {
        [JsonIgnore]
        bool? ShouldEndSession { get; }
    }
}
