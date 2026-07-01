using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TriInspector;

namespace ThunderRoad
{
    [DeclareHorizontalGroup("Horiz")]
    public class HandTouchDetector : ThunderBehaviour
    {
        [Group("Horiz")]
        [AutoLabelWidth(AddedWidth = 7f)]
        public float minRequiredVelocity = 1f;
        [Group("Horiz")]
        [AutoLabelWidth(AddedWidth = 7f)]
        public float maxRegisteredVelocity = 2f;
        [AutoLabelWidth(AddedWidth = 7f)]
        public TouchMotionDir requiredMotionDir = TouchMotionDir.SurfaceNormal;
        [ListDrawerSettings(IgnoreButtonSpacing = true)]
        public List<VelocityReceivedThreshold> velocityReceptionThresholds = new List<VelocityReceivedThreshold>();
        [AutoLabelWidth(AddedWidth = 7f)]
        public bool debugLogging = false;

        public enum TouchMotionDir
        {
            Any,
            SurfaceNormal,
            SurfaceParallel
        }

        [System.Serializable]
        [DeclareBoxGroup("Box", HideTitle = true)]
        [DeclareHorizontalGroup("Box/Horiz", Sizes = new float[] { 0f, 120f })]
        public class VelocityReceivedThreshold
        {
            [Group("Box/Horiz")]
            [AutoLabelWidth(AddedWidth = 7f)]
            public float velocity;
            [Group("Box/Horiz")]
            [AutoLabelWidth(AddedWidth = 7f)]
            public bool totalLifetime = false;
            [Group("Box")]
            public UnityEvent onReached = new UnityEvent();
            [Group("Box")]
            public int maxInvokes = 9999;

            [Group("Box")]
            public int invokes = 0;

            public void Invoke()
            {
                invokes++;
                onReached?.Invoke();
            }
        }

        [Button]
        public void ResetLifetimeVelocityReceived()
        {
        }

        [Button]
        public void RegisterVelocity(float velocity)
        {
        }

    }
}
