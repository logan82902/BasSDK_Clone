using UnityEngine;
using System;
using System.Collections.Generic;


using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/CollisionHandler")]
    [AddComponentMenu("ThunderRoad/Collision handler")]
    public class CollisionHandler : ThunderBehaviour
    {
        public bool active = true;
        public bool checkMinVelocity = true;
        public bool forceFullIntensity = false;
        public bool enterOnly = false;
        public int forceAllowHitLocomotionLayer = -1;

        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public List<CollisionHandler> penetratedObjects = new();

        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public List<Holder> holders = new();

    }
}
