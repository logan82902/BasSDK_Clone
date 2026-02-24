#define SHADOWOODTRI
using System;
using System.Diagnostics;

namespace TriInspector
{
    [AttributeUsage((AttributeTargets.Field | AttributeTargets.Property))]
    [Conditional("UNITY_EDITOR")]
    public class EnumToggleButtonsAttribute : Attribute
    {
#if SHADOWOODTRI
        public int Columns { get; set; }
#endif
    }
}