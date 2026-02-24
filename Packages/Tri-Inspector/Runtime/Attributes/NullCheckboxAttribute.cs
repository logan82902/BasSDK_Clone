// Copyright (c) WarpFrog. All rights reserved.

using System;

namespace TriInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class NullCheckboxAttribute : Attribute
    {
    }
}
