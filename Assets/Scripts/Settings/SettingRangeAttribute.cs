using System;

namespace Assets.Helpers
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class SettingRangeAttribute : Attribute
    {
        public float Min { get; }
        public float Max { get; }
        public float Increment { get; }
        public SettingRangeAttribute(float min, float max, float increment = 0f)
        {
            Min = min;
            Max = max;
            Increment = increment;
        }
    }
}
