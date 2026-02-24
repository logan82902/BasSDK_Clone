using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class SetLookAt : ActionNode
    {
        public bool enabled = true;
        [ShowIf("enabled")]
        public BrainModuleLookAt.BodyBehaviour bodyBehaviour = BrainModuleLookAt.BodyBehaviour.BodyUpright;
        [ShowIf("enabled")]
        public Target target = Target.CurrentTarget;
        [ShowIf("enabled")]
        [HideIf("target", Target.CurrentTarget)]
        public string inputVariableName = "LookTarget";
        [ShowIf("enabled")]
        public Vector3 localOffset = new();

        public enum Target
        {
            CurrentTarget,
            InputTransform,
            InputPosition,
        }

    }
}
