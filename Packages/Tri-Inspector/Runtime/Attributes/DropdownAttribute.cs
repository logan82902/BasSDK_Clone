using System;
using System.Diagnostics;

namespace TriInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    public class DropdownAttribute : Attribute
    {
        public string Values { get; }
        public bool Searchable { get; set; } = true;
        public bool AppendToField { get; set; } = false;

        public TriMessageType ValidationMessageType { get; set; } = TriMessageType.Error;

        public DropdownAttribute(string values)
        {
            Values = values;
        }
    }
}