using UnityEngine;
using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Levels/Rope.html")]
    [DeclareBoxGroup("Rope")]
    [DeclareHorizontalGroup("Rope/Ends")]
    [DeclareHorizontalGroup("Rope/Mesh")]
    [DeclareHorizontalGroup("Rope/Physics")]
    [DeclareHorizontalGroup("Rope/Height")]
    [DeclareHorizontalGroup("Rope/Segments")]
    public class Rope : Handle
    {
        [Group("Rope/Ends")]
        [AutoLabelWidth(AddedWidth = 7f)]
        [Tooltip("Position of where the rope starts.")]
        public Transform ropeStart;
        [Group("Rope/Ends")]
        [AutoLabelWidth(AddedWidth = 7f)]
        [Tooltip("Position of where the rope ends.")]
        public Transform ropeTarget;
        [Group("Rope/Mesh")]
        [AutoLabelWidth(AddedWidth = 7f)]
        [Tooltip("Depicts the radius of the rope mesh (Default : 0.03")]
        public float ropeRadius = 0.03f;
        [Group("Rope/Mesh")]
        [AutoLabelWidth(AddedWidth = 7f)]
        [Tooltip("What material the rope uses when generated.")]
        public Material ropeMaterial;
        [Group("Rope/Physics")]
        [AutoLabelWidth(AddedWidth = 7f)]
        [Tooltip("Depicts if the rope has a collider")]
        public bool ropeUseCollider;
        [Group("Rope/Physics")]
        [AutoLabelWidth(AddedWidth = 7f)]
        [Tooltip("Depicts what layer the rope is on (default: 17)")]
        public int ropeLayer;
        [Group("Rope/Physics")]
        [AutoLabelWidth(AddedWidth = 7f)]
        [Tooltip("Depicts what physics material the rope uses.")]
        public PhysicMaterial ropePhysicMaterial;
        [Tooltip("(Obsolete) Depicts what is stated on the handle when the hand is in the handle radius.")]
        public string ropeTag = "Rope";

        [Group("Rope/Height")]
        [Title("Dynamic height")]
        [AutoLabelWidth(AddedWidth = 7f)]
        [Tooltip("When enabled, the Rope Target is ignored, and instead the rope will use a raycast to depict the length of the rope.")]
        public bool dynamicHeight = false;
        [Group("Rope/Height")]
        [Title(" ")]
        [AutoLabelWidth(AddedWidth = 7f)]
        [Tooltip("The maximum range of the raycast used to determine the rope length")]
        public float raycastRange = 50.0f;
        [Group("Rope/Height")]
        [Title(" ")]
        [AutoLabelWidth(AddedWidth = 7f)]
        [Tooltip("Once the Dynamic height raycast is complete, how far from the ground do you want the rope to cut back by.")]
        public float heightFromGround = 2.0f;
        [Group("Rope/Segments")]
        [AutoLabelWidth(AddedWidth = 7f)]
        [Tooltip("If enabled, after finding the dynamic height of the rope, determines whether or not it gets split into segments.")]
        public bool splitIntoSegments = false;
        [Group("Rope/Segments")]
        [AutoLabelWidth(AddedWidth = 7f)]
        [Tooltip("When splitting the rope into segments, use this length to determine where segments start/stop.")]
        public float segmentLength = 1f;
        [Group("Rope/Segments")]
        [AutoLabelWidth(AddedWidth = 7f)]
        [Tooltip("If enabled, after calculating how many segments the rope needs, even out the lengths so they're all the same.")]
        public bool evenSegments = true;

        private bool isGenerated = false;
        private LightProbeVolume _probeVolume;
        private Area area;

        protected virtual void OnDrawGizmos()
        {
            if (ropeTarget)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(ropeStart.position, ropeTarget.position);
            }
        }
    }
}
