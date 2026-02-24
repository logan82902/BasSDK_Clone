using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriInspector
{
    [AttributeUsage((AttributeTargets.Class | AttributeTargets.Struct))]
    [Conditional("UNITY_EDITOR")]
    public class ListElementLabelAttribute : Attribute
    {
        public string Text { get; set; }

        public ListElementLabelAttribute(string text)
        {
            Text = text;
        }
    }
}
