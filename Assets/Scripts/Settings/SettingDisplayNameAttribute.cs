using System;

namespace Assets.Helpers
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class SettingDisplayNameAttribute : Attribute
    {
        public string Name { get; }
        public SettingDisplayNameAttribute(string name) => Name = name;
    }
}
