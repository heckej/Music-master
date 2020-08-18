using Newtonsoft.Json;

namespace Crunch.NET.Response
{
    public class LinkAccountCard : ICard
    {
        [JsonProperty("type")]
        public string Type
        {
            get { return "LinkAccount"; }
        }
    }
}