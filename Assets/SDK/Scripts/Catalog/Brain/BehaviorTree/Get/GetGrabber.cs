using UnityEngine;
using System;
using ThunderRoad.Skill.SpellPower;
using TriInspector;

namespace ThunderRoad.AI.Get
{
    [DeclareBoxGroup("Options", Title = "Options")]
    [DeclareHorizontalGroup("Options/Horiz")]
    [DeclareHorizontalGroup("Options/Horiz2")]
    [DeclareHorizontalGroup("Options/Horiz3")]
    [DeclareHorizontalGroup("Options/Horiz4")]
    [DeclareBoxGroup("Outputs", Title = "Outputs")]
    [DeclareHorizontalGroup("Outputs/Horiz", Sizes = new float[] { 215f, 0f })]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class GetGrabber : ActionNode
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

        public enum Faction
        {
            OnlyEnemy,
            Either,
            OnlyFriend,
            OnlyPlayer,
        }

        public Faction factionCondition = Faction.OnlyEnemy;
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
        public float rememberDuration = 20f;
        [Group("Outputs/Horiz")]
        [LabelWidth(200)]
        public bool outputCreatureAsCurrentTarget = true;
        [Group("Outputs/Horiz")]
        [LabelWidth(200)]
        [DisableIf(nameof(outputCreatureAsCurrentTarget))]
        public string outputCreatureVariableName = "";

        public GetGrabber() { }

    }
}