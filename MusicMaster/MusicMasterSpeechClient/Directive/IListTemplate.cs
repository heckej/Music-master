using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Crunch.NET.Response.Directive
{
    public interface IListTemplate : ITemplate
    {
        [JsonProperty("listItems", Required = Required.Always)]
        List<ListItem> Items { get; set; }
    }
}
