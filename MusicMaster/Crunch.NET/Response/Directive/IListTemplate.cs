using Newtonsoft.Json;
using System.Collections.Generic;

namespace Crunch.NET.Response.Directive
{
    public interface IListTemplate : ITemplate
    {
        [JsonProperty("listItems", Required = Required.Always)]
        List<ListItem> Items { get; set; }
    }
}
