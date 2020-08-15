using System;
namespace Crunch.NET.Request
{
    public class IntentProperty
    {
        public IntentProperty(string entity, string property)
        {
            Entity = entity;
            Property = property;
        }

        public string Entity { get; }
        public string Property { get; }
    }
}
