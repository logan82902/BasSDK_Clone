// Copyright (c) WarpFrog. All rights reserved.

using System;

namespace ThunderRoad
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EditorLoadPriorityAttribute : Attribute
    {
        public int Priority { get; set; }

        public EditorLoadPriorityAttribute(int priority) => Priority = priority;
    }
}
