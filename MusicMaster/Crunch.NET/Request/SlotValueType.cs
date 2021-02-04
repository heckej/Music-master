using System.Runtime.Serialization;

namespace Crunch.NET.Request
{
    public enum SlotValueType
    {
        [EnumMember(Value = "Simple")]
        Simple,
        [EnumMember(Value = "List")]
        List
    }
}