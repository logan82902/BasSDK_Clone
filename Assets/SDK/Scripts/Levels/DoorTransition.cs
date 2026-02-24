using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using TriInspector;

namespace ThunderRoad
{
    public class DoorTransition : MonoBehaviour
    {
        public HingeJoint doorJoint;
        public float jointSensitivity = 0.9f;
        public float fadeoutTimer = 0.5f;
        public string mapDestination;
        public Transform teleportDestination;
        public List<Collider> ignoredColliders;
    }
}