using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ThunderRoad
{
    public class PlayerDetector : ThunderBehaviour
    {
        public UnityEvent onPlayerEnter;
        public UnityEvent onPlayerExit;
        public LayerMask playerMask;

        private List<Collider> colliders;
        private bool playerInside = false;
        private bool spectatorInside = false;

        private void OnTriggerEnter(Collider other) => HandleTriggerChange(other, true);

        private void OnTriggerExit(Collider other) => HandleTriggerChange(other, false);

        protected override void ManagedOnEnable()
        {
            base.ManagedOnEnable();
            colliders = new List<Collider>(GetComponentsInChildren<Collider>());
        }

        protected bool IsInLayerMask(int layerMask, int layer) => layerMask == (layerMask | (1 << layer));

        protected virtual void HandleTriggerChange(Collider collider, bool enter)
        {
            PhysicBody pb = collider.GetPhysicBody();
            // We generally want the layer of the rigidbody attached to the collider, but if there is no attached rigidbody (which should be *exceptionally* rare), go with the collider layer
            int objectLayer = pb?.gameObject.layer ?? collider.gameObject.layer;
            if (!IsInLayerMask(playerMask, objectLayer)) return;
            playerInside = enter;
            TryInvokeEvent(enter);
        }

        private void TryInvokeEvent(bool enter)
        {
            if (enter)
            {
                // if enter is true, playerInside or spectatorInside should be true. if the other is equal, it's already inside, so don't invoke the event
                if (playerInside == spectatorInside) return;
                onPlayerEnter?.Invoke();
            }
            else
            {
                // if we're trying to invoke the exit event, but either is still inside, return early to not invoke the event
                if (playerInside || spectatorInside) return;
                onPlayerExit?.Invoke();
            }
        }


        protected enum Direction
        {
            Exit,
            Enter
        }
    }
}