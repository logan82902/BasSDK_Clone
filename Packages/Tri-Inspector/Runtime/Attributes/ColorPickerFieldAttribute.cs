using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace TriInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    public class ColorPickerFieldAttribute : Attribute
    {
        public bool EyeDropper { get; set; } = true;
        public bool Alpha { get; set; } = false;
        public bool HDR { get; set; } = false;
    }
}
