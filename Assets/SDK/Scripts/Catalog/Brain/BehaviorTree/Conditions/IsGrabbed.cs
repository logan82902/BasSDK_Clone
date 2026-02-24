using System;
using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad.AI.Condition
{
    [DeclareBoxGroup("Options", Title = "Options")]
    [DeclareHorizontalGroup("Options/Horiz")]
    [DeclareHorizontalGroup("Options/Horiz2")]
    [DeclareHorizontalGroup("Options/Horiz3")]
    [DeclareHorizontalGroup("Options/Horiz4")]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class IsGrabbed : ConditionNode
    {
        [Flags]
        public enum HandleOptions
        {
            SpecificHandle = 0,
            CarryHandle = 1,
            MuffleHandle = 2,
            ChokeHandle = 4
        }

        public enum FromDirection
        {
            Either,
            FrontOnly,
            BackOnly
        }

        [Group("Options/Horiz")]
        [LabelWidth(200)]
        public bool anyHandle = true;
        [Group("Options/Horiz2")]
        [LabelWidth(200)]
        [DisableIf("anyHandle", true)]
        public HandleOptions handleSpecifiers = HandleOptions.SpecificHandle;
        [Group("Options/Horiz3")]
        [LabelWidth(200)]
        [DisableIf("anyHandle", true)]
        [EnableIf(nameof(handleSpecifiers), HandleOptions.SpecificHandle)]
        public string handleName = "None";
        [Group("Options/Horiz4")]
        [LabelWidth(200)]
        public FromDirection fromDirection = FromDirection.Either;

    }
}