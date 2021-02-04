using System.Runtime.Serialization;

namespace Crunch.NET.Response.Directive
{
    public enum PlayBehavior
    {
        [EnumMember(Value = "REPLACE_ALL")]
        ReplaceAll,
        [EnumMember(Value = "ENQUEUE")]
        Enqueue,
        [EnumMember(Value = "REPLACE_ENQUEUED")]
        ReplaceEnqueued
    }
}
