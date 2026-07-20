using System;
using UnityEngine;
using UnityEngine.Events;

namespace ThunderRoad
{
    [RequireComponent(typeof(Rigidbody))]
    public class StickyObject : ThunderBehaviour
    {
        public bool stickyActive = true;
        public bool allowStickWhileHeld = false;
        public ObjectClassification stickTargets = ~ObjectClassification.None;
        public UnityEvent onStick;
        public UnityEvent onUnstick;

        public void SetActive(bool active) => stickyActive = active;

        public void Unstick()
        {
        }

    }
}
