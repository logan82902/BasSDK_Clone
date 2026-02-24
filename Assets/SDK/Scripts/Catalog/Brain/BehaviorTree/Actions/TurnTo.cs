using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class TurnTo : ActionNode
    {
        public Target target = Target.CurrentTarget;
        [HideIf("target", Target.CurrentTarget)]
        public string inputVariableName = "TurnTarget";

        public bool successIfAngleReached;
        public float successAngle = 0;

        public bool becomeExclusiveHandler = false;

        public enum Target
        {
            CurrentTarget,
            InputTransform,
            InputTransformForward,
            InputPosition,
        }

    }
}
