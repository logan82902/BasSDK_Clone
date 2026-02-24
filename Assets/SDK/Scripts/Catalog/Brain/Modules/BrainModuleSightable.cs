using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.AI;


using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class ShootPosition
    {
        public Vector3 position;
        public bool valid;
        public Creature creature;
    }

    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    [DeclareHorizontalGroup("Split")]
    [DeclareBoxGroup("Split/Sight", Title = "Sight")]
    [DeclareBoxGroup("Split/Shoot Position", Title = "Shoot Position")]
    [DeclareHorizontalGroup("Split2")]
    [DeclareBoxGroup("Split2/Instance", Title = "Instance")]
    public class BrainModuleSightable : BrainData.Module
    {
        [Group("Split/Sight")]
        [LabelWidth(200)]
        public float sightThickness = 0.1f;
        [Group("Split/Sight")]
        [LabelWidth(200)]
        public float sightDetectionMaxDistance = 20;
        [Group("Split/Sight")]
        [LabelWidth(200)]
        public float sightMaxDistance = 30;
        // Defines this creature's ability to see in the dark. Objects with a brightness below the minimum brightness aren't visible to the creature, objects above are visible.
        // Max brightness is used for determining how alert a creature may be to the object. Objects above max brightness contribute fully to creature alertedness increases
        [Group("Split/Sight")]
        [LabelWidth(200)]
        [MinMaxSlider(0f, 1f, true)]
        public Vector2 minMaxBrightness = new(0f, 0.25f);
        [Group("Split/Sight")]
        [LabelWidth(200)]
        public float shootPointSightThickness = 0;
        [Group("Split/Sight")]
        [LabelWidth(200)]
        public List<SightableRagdollPart> sightableRagdollParts;

        public enum SightableRagdollPart
        {
            TargetPart,
            HeadPart,
            RootPart,
            HandRightPart,
            HandLeftPart,
            FootRightPart,
            FootLeftPart
        }

        [Group("Split/Shoot Position")]
        [LabelWidth(200)]
        public int shootPositionCircleSteps = 15;
        [Group("Split/Shoot Position")]
        [LabelWidth(200)]
        public int shootPositionDepthSteps = 4;
        [Group("Split/Shoot Position")]
        [LabelWidth(200)]
        public int shootPositionMinRadius = 8;
        [Group("Split/Shoot Position")]
        [LabelWidth(200)]
        public float navSamplePositionMaxDistance = 50;
        [Group("Split/Shoot Position")]
        [LabelWidth(200)]
        public float lastShootUpdateDelay = 1;
        [Group("Split/Shoot Position")]
        [LabelWidth(200)]
        public float lastShootUpdatePositionMaxDistance = 0.5f;
        [Group("Split/Shoot Position")]
        [LabelWidth(200)]
        public bool debug;

        [Group("Split2/Instance")]
        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public ShootPoint[] shootpoints = new ShootPoint[50];
        [Group("Split2/Instance")]
        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public int shootpointCount;
        protected float lastUpdateShootPointsTime;
        protected Vector3 lastUpdateShootPointsLocation;

        [Group("Split2/Instance")]
        [ShowInInspector]
        [ReadOnly]
        [TableList()]
        [NonSerialized]
        public ShootPosition[] shootPositions;
        protected float lastUpdateShootPositionsTime;
        protected Vector3 lastUpdateShootPositionsLocation;

    }
}