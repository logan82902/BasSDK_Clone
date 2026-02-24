using System;

namespace TriInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ReadOnlyReferencePickerAttribute : ReferencePickerStyleAttribute
    {
    }
}
