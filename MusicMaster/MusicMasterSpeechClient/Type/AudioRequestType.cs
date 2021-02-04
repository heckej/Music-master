using System.Runtime.Serialization;

namespace Crunch.NET.Request.Type
{
    public enum AudioRequestType
    {
        [EnumMember(Value = "PlaybackStarted")]
        PlaybackStarted,
        [EnumMember(Value = "PlaybackFinished")]
        PlaybackFinished,
        [EnumMember(Value = "PlaybackStopped")]
        PlaybackStopped,
        [EnumMember(Value = "PlaybackNearlyFinished")]
        PlaybackNearlyFinished,
        [EnumMember(Value = "PlaybackFailed")]
        PlaybackFailed,
        [EnumMember(Value = "Unknown")]
        Unknown
    }
}
