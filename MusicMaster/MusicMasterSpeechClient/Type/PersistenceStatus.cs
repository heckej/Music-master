using System.Runtime.Serialization;

namespace Crunch.NET.Request.Type
{
    public enum PersistenceStatus
    {
        [EnumMember(Value = "PERSISTED")]
        Persisted,
        [EnumMember(Value = "NOT_PERSISTED")]
        NotPersisted
    }
}