using System.Runtime.Serialization;

namespace Crunch.NET.Request.Type
{
    public enum LocationServiceStatus
    {
        [EnumMember(Value = "RUNNING")]
        Running,
        [EnumMember(Value = "STOPPED")]
        Stopped
    }
}