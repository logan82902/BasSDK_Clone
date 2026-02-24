using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class PickSpell : ActionNode
    {
        public Side side = Side.Left;
        public SelectionMode selectionMode = SelectionMode.ByDistance;
        [ShowIf("selectionMode", SelectionMode.ByID)]
        public string spellID = "";
        [ShowIf("selectionMode", SelectionMode.ByDistance)]
        public Target target = Target.CurrentTarget;
        [ShowIf("selectionMode", SelectionMode.ByDistance)]
        [DisableIf("target", Target.CurrentTarget)]
        public string inputTransformVariableName = "";

        public enum SelectionMode
        {
            ByDistance,
            ByID,
            Random,
        }

        public enum Target
        {
            CurrentTarget,
            InputTransform,
        }

    }
}
