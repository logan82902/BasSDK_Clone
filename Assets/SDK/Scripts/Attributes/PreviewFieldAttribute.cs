using System;
using System.Diagnostics;

namespace ThunderRoad
{
    [AttributeUsage((AttributeTargets.Field | AttributeTargets.Property))]
    [Conditional("UNITY_EDITOR")]
    public class PreviewFieldAttribute : Attribute
    {
        public int LinesHeight { get; set; } = 2;
        public float Height { get; set; }
        public bool SceneObjects { get; set; }
        public bool LabelTop { get; set; }

        public PreviewFieldAttribute() { }
    }
}
