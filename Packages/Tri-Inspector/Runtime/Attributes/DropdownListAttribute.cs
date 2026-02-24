using System;
using System.Diagnostics;

namespace TriInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [Conditional("UNITY_EDITOR")]
    public class DropdownListAttribute : ListDrawerSettingsAttribute
    {
        public string Source { get; set; }
        public bool Searchable { get; set; } = true;

        public DropdownListAttribute(string source)
        {
            Source = source;
        }
    }
}