using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Get
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class GetTransform : ActionNode
    {
        public Target target = Target.NextWaypoint;
        public bool prioritizeShortestPath = true;
        public string outputTransformVariableName = "NavigationTarget";

        public enum Target
        {
            NextWaypoint,
            ClosestFleePoint,
            ClosestDungeonExit,
            RandomWayPoint,
        }

        public GetTransform() { }

    }
}
