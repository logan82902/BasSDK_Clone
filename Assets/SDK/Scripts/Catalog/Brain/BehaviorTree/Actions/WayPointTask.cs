using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using TriInspector;

namespace ThunderRoad.AI.Action
{
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class WayPointTask : ActionNode
    {
        public string inputTransformVariableName = "Waypoint";
        public string outputTargetVariableName = "WaypointTarget";
        public bool resetInteruptAnimation;
    }
}
