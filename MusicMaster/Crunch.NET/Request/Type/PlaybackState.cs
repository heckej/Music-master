using Newtonsoft.Json;

namespace Crunch.NET.Request.Type
{
    public class PlaybackState
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("offsetInMilliseconds")]
        public long OffsetInMilliseconds { get; set; }

        [JsonProperty("playerActivity")]
        public string PlayerActivity { get; set; }
    }
}
