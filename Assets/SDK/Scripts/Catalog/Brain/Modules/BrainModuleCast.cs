using UnityEngine;
using ThunderRoad.AI;
using ThunderRoad.AI.Action;
using System.Collections;


using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    [DeclareBoxGroup("Probability", Title = "Probability")]
    public class BrainModuleCast : BrainData.Module
    {
        [MinMaxSlider(0, 60, true)]
        public Vector2 castMinMaxDelay = new(8, 16);

        public float aimArmLengthMultiplier = 0.99f;

        public float minMana = 5;
        public float maxAimAngle = 50;
        public float chargeDurationMultiplier = 1f;
        public bool usePositionIK = true;
        public bool useRotationIK = false;
        public AnimationCurve IKAdjustCurve;
        public float IKMaxWeight = 0.7f;
        public float throwEarlyRelease = 0.1f;
        public Vector2 spreadCone = new Vector2(0.6f, 0.6f);
        public float maxFireAngle = 60;

        [Group("Probability")]
        public Vector2 shortRangeMinMaxDist = new(0, 5);
        [Group("Probability")]
        public Vector2 midRangeMinMaxDist = new(5, 8);
        [Group("Probability")]
        public Vector2 longRangeMinMaxDist = new(8, 20);

    }
}