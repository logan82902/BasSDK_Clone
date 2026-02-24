using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [DeclareHorizontalGroup("Horiz", Sizes = new float[] { 80f, 0f, 80f, 0f})]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class BlackboardFloatAdd : ActionNode
    {
        public struct Additive
        {
            public bool useOtherBlackboardValue;
            [HideIf("useOtherBlackboardValue")]
            public float value;
            [ShowIf("useOtherBlackboardValue")]
            public string otherValueName;
        }

        public string blackboardValue = "";
        public bool scaleWithCycleSpeed = true;
        public List<Additive> toAdd = new List<Additive>();

        [LabelText("Min Value")]
        [LabelWidth(65)]
        [Group("Horiz")]
        public bool clampLower = false;
        [EnableIf(nameof(clampLower))]
        [Group("Horiz")]
        [HideLabel]
        public float minValue = 0f;
        [LabelText("Max Value")]
        [LabelWidth(65)]
        [Group("Horiz")]
        public bool clampUpper = false;
        [EnableIf(nameof(clampUpper))]
        [Group("Horiz")]
        [HideLabel]
        public float maxValue = 10f;

        [ShowInInspector]
        [ReadOnly]
        [LabelText("Current Value")]
        [LabelWidth(80)]
        protected float value = 0f;

    }
}
