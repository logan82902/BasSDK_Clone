using System;
using System.Diagnostics;

namespace TriInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class AutoLabelWidthAttribute : ApplyToChildsAttribute
    {
        public float AddedWidth { get; set; } = 0f;
        public float MinWidth { get; set; } = 0.01f;

        public AutoLabelWidthAttribute() { }
    }
}
