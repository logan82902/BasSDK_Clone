using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Condition
{
    [DeclareBoxGroup("Inputs", Title = "Inputs")]
    [DeclareHorizontalGroup("Inputs/Horiz")]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class TargetHasMoved : ConditionNode
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
        [Group("Inputs")]
        public float distance = 3;

    }
}
