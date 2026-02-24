using System;
using System.Collections.Generic;
using ThunderRoad.Skill.SpellPower;
using TriInspector;

namespace ThunderRoad.AI.Condition
{
    [DeclareBoxGroup("Options", Title = "Options")]
    [DeclareHorizontalGroup("Options/Horiz")]
    [DeclareHorizontalGroup("Options/Horiz2")]
    [DeclareHorizontalGroup("Options/Horiz3")]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class IsTKGrabbed : ConditionNode
    {
        [Group("Options/Horiz")]
        [LabelWidth(200)]
        public bool anyHandle = true;
        [Group("Options/Horiz2")]
        [LabelWidth(200)]
        [DisableIf("anyHandle", true)]
        public TKOptions handleSpecifiers = TKOptions.SpecificHandle;
        [Group("Options/Horiz3")]
        [LabelWidth(200)]
        [DisableIf("anyHandle", true)]
        [EnableIf(nameof(handleSpecifiers), TKOptions.SpecificHandle)]
        public string handleName = "None";

        [Flags]
        public enum TKOptions
        {
            SpecificHandle = 0,
            CarryHandle = 1,
            ChokeHandle = 2
        }

    }
}