#define SHADOWOODTRI
// Copyright (c) WarpFrog. All rights reserved.

using System;

namespace TriInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class InlineButtonAttribute : Attribute
    {
        public string Method { get; set; }
        public string Text { get; set; }
        public float ButtonWidth { get; set; }
        public float ButtonHeight { get; set; }
        public bool LeftSide { get; set; } = false;
        public bool DropdownButton { get; set; } = false;
#if SHADOWOODTRI
        public bool NewLine { get; set; } = false;
#endif
        public GUIEnableState GUIEnabled { get; set; } = GUIEnableState.Always;
        
        public enum GUIEnableState
        {
            Always,
            WithProperty,
            OppositeProperty,
        }

        public InlineButtonAttribute(string method, string text = null)
        {
            Method = method;
            Text = text;
        }
    }
}
