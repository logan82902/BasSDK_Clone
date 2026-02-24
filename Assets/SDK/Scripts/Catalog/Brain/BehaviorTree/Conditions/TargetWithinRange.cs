using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Condition
{
    [DeclareBoxGroup("Inputs", Title = "Inputs")]
    [DeclareHorizontalGroup("Inputs/Horiz")]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class TargetWithinRange : ConditionNode
    {
        [Group("Inputs")]
        public bool flatDistance = false;
        [Group("Inputs/Horiz")]
        [LabelWidth(100)]
        public Target target = Target.CurrentTarget;
        [Group("Inputs/Horiz")]
        [ShowIf("target", Target.InputTransform)]
        [LabelWidth(200)]
        public string inputTransformVariableName = "";

        public enum Target
        {
            CurrentTarget,
            InputTransform,
            None,
        }

        public enum CompareType
        {
            LessThanOrEqual,
            LessThan,
            CloseTo,
            GreaterThan,
            GreaterOrEqual
        }

        public enum Range
        {
            Fixed,
            MinMeleeRange,
            MaxMeleeRange,
            Blackboard,
        }

        [Group("Inputs")]
        public CompareType comparison;
        [Group("Inputs")]
        public Range range = Range.Fixed;
        [Group("Inputs")]
        [ShowIf("range", Range.Fixed)]
        public float distance;
        [Group("Inputs")]
        [ShowIf("range", Range.Blackboard)]
        public string valueName = "TargetWithinRange";
        [Group("Inputs")]
        [HideIf("range", Range.Fixed)]
        public float addedDistance;
        [Group("Inputs")]
        [EnableIf(nameof(comparison), CompareType.CloseTo)]
        public float maxDiff;

    }
}
