using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ThunderRoad
{
    public class JointedWeaponHandler : ThunderBehaviour
    {

        public static List<JointedWeaponHandler> allActive = new List<JointedWeaponHandler>();

        public List<Item> jointedItems = new List<Item>();
        public bool manuallyCalculateVelocities = true;
        public float physicToggleRadius;
        public bool physicToggleHandlingOnly;
        public float itemHitCreatureCooldown = 0.5f;
        public bool disableItemCreatureCollision = true;

        [NonSerialized]
        public bool handling;

        public void StartHandling()
        {
        }

        public void StopHandling()
        {
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = !physicToggleHandlingOnly || handling ? Color.yellow : Color.white;
            Gizmos.DrawWireSphere(transform.position, physicToggleRadius);
        }
    }
}
