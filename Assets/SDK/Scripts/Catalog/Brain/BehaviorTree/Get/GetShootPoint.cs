using UnityEngine;
using UnityEngine.AI;
using TriInspector;

namespace ThunderRoad.AI.Get
{
    [DeclareBoxGroup("Inputs", Title = "Inputs")]
    [DeclareHorizontalGroup("Inputs/Horiz")]
    [DeclareBoxGroup("Selection", Title = "Selection")]
    [DeclareBoxGroup("Output", Title = "Output")]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class GetShootPoint : ActionNode
    {
        [Group("Inputs/Horiz")]
        [LabelWidth(100)]
        public Target target = Target.CurrentTarget;
        [DisableIf("target", Target.CurrentTarget)]
        [Group("Inputs/Horiz")]
        [LabelWidth(200)]
        public string inputVariableName = "";

        public enum Target
        {
            CurrentTarget,
            InputCreature,
        }

        [Group("Selection")]
        [LabelWidth(200)]
        public bool checkShortestPath = false;

        [Group("Output")]
        public string outputPositionVariableName = "ShootPosition";

        public enum Sight
        {
            Nothing,
            Creature,
            Target,
        }

        public GetShootPoint() { }

    }
}
