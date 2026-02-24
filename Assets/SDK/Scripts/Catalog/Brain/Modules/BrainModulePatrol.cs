using UnityEngine;
using System;


using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class BrainModulePatrol : BrainData.Module
    {
        public float distanceChangePoint = 1;
        public float viewRadius;
        public float viewAngle;
        public LayerMask targetMask;
        public LayerMask obstacleMask;
        public float runSpeedRatio;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public int targetWaypointIndex;
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public WayPoint[] waypoints;
    }
}