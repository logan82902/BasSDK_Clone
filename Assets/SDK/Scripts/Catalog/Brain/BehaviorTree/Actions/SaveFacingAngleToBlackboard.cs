using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class SaveFacingAngleToBlackboard : ActionNode
    {
        public bool flatAngle = true;
        public string inputTransformVariableName = "InputTransform";
        public string outputAngleVariableName = "FacingAngle";

        public override State Evaluate()
        {
            
            return State.SUCCESS;
            
        }
    }
}
