using UnityEngine;
using System.Collections.Generic;
using System;
using ThunderRoad.Skill.SpellPower;
using UnityEngine.Events;

using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Items/Handle.html")]
    [AddComponentMenu("ThunderRoad/Handle")]
    [DeclareBoxGroup("Grabbing")]
    [DeclareVerticalGroup("Grabbing/Orientations")]
    [DeclareVerticalGroup("Grabbing/Orientations/Default")]
    [DeclareHorizontalGroup("Grabbing/Orientations/Default/Horiz")]
    [DeclareHorizontalGroup("Grabbing/OtherHandles")]
    [DeclareBoxGroup("Axis Events")]
    public class Handle : Interactable
    {
        [Range(-1, 1)]
        [Group("Grabbing")]
        [Tooltip("Define where the handle is automatically grabbed along the axis length")]
        public float defaultGrabAxisRatio;
        [Group("Grabbing")]
        public Vector3 ikAnchorOffset;

        [NonSerialized]
        public List<HandlePose> orientations = new();
        [Group("Grabbing/Orientations/Default/Horiz")]
        [Tooltip("The default handpose to be grabbed (Left Hand)")]
        public HandlePose orientationDefaultLeft;
        [Group("Grabbing/Orientations/Default/Horiz")]
        [Tooltip("The default handpose to be grabbed (Right Hand)")]
        public HandlePose orientationDefaultRight;
        [Group("Grabbing/Orientations")]
        [LabelText("!! OBSOLETE - HOVER FOR TOOLTIP !! Allowed Orientations")]
        [Tooltip("Legacy method for setting hand poses. This is still functional but is unnecessary to fill.\n\nYou can still use this list to set hand poses: once you're finished, use the \"Try Make Default Hand Poses\" button above to generate proper hand pose objects.")]
        public List<Orientation> allowedOrientations = new List<Orientation>();
        [Group("Grabbing/OtherHandles")]
        [Tooltip("When linked handle is grabbed, ungrip this handle.")]
        public Handle releaseHandle;
        [Group("Grabbing/OtherHandles")]
        [Tooltip("Handle will only activate when the linked handle is grabbed.")]
        public Handle activateHandle;
        [Group("Grabbing/OtherHandles")]
        [Tooltip("NPCs will try to grab this handle if their brain logic tells them to.")]
        public Handle AIGrabHandle;

        [Group("Grabbing")]
        [Tooltip("When ticked, no sound will play when the handle is grabbed")]
        public bool silentGrab = false;
        [Group("Grabbing")]
        [Tooltip("When ticked, the player will ungrab the handle when grounded (touching the ground)")]
        public bool forceAutoDropWhenGrounded;
        [Group("Grabbing")]
        [Tooltip("The default handpose to be grabbed (Right Hand)")]
        public bool ignoreClimbingForceOverride;

        [Group("Grabbing")]
        [Tooltip("If the player can cast spells while holding this handle, this transform defines where the orb will appear.")]
        public Transform spellOrbTarget;

        [Group("Grabbing")]
        [Tooltip("Lets AI know how far the item is away from the player. You can use the button to calculate this automatically, so long as a ColliderGroup is set up with sufficient colliders.")]
        [Min(0f)]
        [InlineButton(nameof(CalculateReach), "Calculate Reach")]
        public float reach = 0.5f;
        [Group("Grabbing")]
        [Tooltip("(Optional)Disables listed colliders once the handle is grabbed.")]
        public List<Collider> handOverlapColliders;
        [Group("Grabbing")]
        [Tooltip("(Optional) Allows you to add a custom rigidbody to the handle. (Do not reference item!)")]
        [Obsolete("Handle's customRigidBody should not be referenced directly! Instead, use customPhysicBody and associated PhysicBody methods.")]
        public Rigidbody customRigidBody;
        [Group("Grabbing")]
        [Tooltip("If set to true, requires that both handlers be grabbing onto the same physicbody to count as two handed grip")]
        public bool twoHandedRequireSamePhysicbody = false;
        [Group("Grabbing")]
        [Tooltip("(Optional) When player hand reaches the top of the handle via slide, it will switch to listed handle once the top is reached.")]
        public Handle slideToUpHandle;
        [Group("Grabbing")]
        [Tooltip("(Optional) When player hand reaches the bottom of the handle via slide, it will switch to listed handle once the bottom is reached.")]
        public Handle slideToBottomHandle;
        [Group("Grabbing")]
        [Tooltip("(Optional) Offset of the bottom and top slide up handles. Can switch handle when reaching 0.2 meters away from the top/bottom, for example.")]
        public float slideToHandleOffset = 0.01f;
        [Group("Grabbing")]
        [Tooltip("Allows you to enable/disable handle sliding (Only works on handles with a length greater than 0!)")]
        public SlideBehavior slideBehavior = SlideBehavior.CanSlide;

        [Group("Grabbing")]
        [Tooltip("(Optional) When you slide up the handle, and the axis length is 0, sliding will instead snap to the referenced handle.")]
        public Handle moveToHandle;
        [Group("Grabbing")]
        [Tooltip("Axis Position for the \"Move To Handle\" handle")]
        public float moveToHandleAxisPos = 0;
        [Group("Grabbing")]
        [Tooltip("When this box is checked, hand poses will update whenever the target weight changes or whenever the pose data changes.")]
        public bool updatePosesAutomatically = false;


        [Group("Axis Events")]
        public bool redirectControllerAxis;
        [Group("Axis Events")]
        public UnityEvent<float> controllerAxisOutputX = new();
        [Group("Axis Events")]
        public UnityEvent<float> controllerAxisOutputY = new();

        public new enum HandSide
        {
            None,
            Right,
            Left,
            Both,
        }

        public enum SlideBehavior
        {
            CanSlide,
            KeepSlide,
            DisallowSlide,
        }

        [Serializable]
        public class Orientation
        {
            public Vector3 rotation;
            public Vector3 positionOffset;
            public HandSide allowedHand = HandSide.Both;
            public HandSide isDefault = HandSide.None;
            public Orientation(Vector3 position, Vector3 rotation, HandSide allowedHand, HandSide isDefault)
            {
                this.rotation = rotation;
                this.positionOffset = position;
                this.allowedHand = allowedHand;
                this.isDefault = isDefault;
            }
        }
        public enum TwoHandedMode
        {
            Position,
            Dominant,
            AutoFront,
            AutoRear,
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
#if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer)
                return;

            if (!this.InPrefabScene())
                return;
#endif      
            if (!gameObject.activeInHierarchy)
                return;
            CheckHandOverlapColliders();
        }

        private void CheckHandOverlapColliders()
        {
            for (int i = handOverlapColliders.Count - 1; i >= 0; i--)
            {
                if (handOverlapColliders[i] == null)
                {
                    Debug.LogWarning($"Handle '{transform.GetPathFrom(transform.root)}' has a missing reference in its hand overlap colliders list! The missing reference is being removed.");
                    handOverlapColliders.RemoveAt(i);
                }
            }
        }


        [Group("Grabbing/Orientations/Default")]
        [Button]
        public virtual void TryMakeDefaultHandPoses()
        {
            orientations = new List<HandlePose>(this.GetComponentsInChildren<HandlePose>());
            if (orientations.Count == 0)
            {
                if (allowedOrientations.Count > 0)
                {
                    foreach (Orientation allowedOrientation in allowedOrientations)
                    {
                        if (allowedOrientation.allowedHand == HandSide.Both || allowedOrientation.allowedHand == HandSide.Right)
                        {
                            HandlePose handleOrientation = AddOrientation(Side.Right, allowedOrientation.positionOffset, Quaternion.Euler(allowedOrientation.rotation));
                            if (orientationDefaultRight == null && (allowedOrientation.isDefault == HandSide.Both || allowedOrientation.isDefault == HandSide.Right))
                            {
                                orientationDefaultRight = handleOrientation;
                            }
                        }
                        if (allowedOrientation.allowedHand == HandSide.Both || allowedOrientation.allowedHand == HandSide.Left)
                        {
                            HandlePose handleOrientation = AddOrientation(Side.Left, allowedOrientation.positionOffset, Quaternion.Euler(allowedOrientation.rotation));
                            if (orientationDefaultLeft == null && (allowedOrientation.isDefault == HandSide.Both || allowedOrientation.isDefault == HandSide.Left))
                            {
                                orientationDefaultLeft = handleOrientation;
                            }
                        }
                    }
                }
                else
                {
                    orientationDefaultRight = AddOrientation(Side.Right, Vector3.zero, Quaternion.identity);
                    orientationDefaultLeft = AddOrientation(Side.Left, Vector3.zero, Quaternion.identity);
                }
            }
        }

        public virtual HandlePose AddOrientation(Side side, Vector3 position, Quaternion rotation)
        {
            GameObject orient = new GameObject("Orient");
            orient.transform.SetParent(this.transform);
            orient.transform.localPosition = position;
            orient.transform.localRotation = rotation;
            orient.transform.localScale = Vector3.one;
            HandlePose handleOrientation = orient.AddComponent<HandlePose>();
            handleOrientation.side = side;
#if UNITY_EDITOR
            handleOrientation.UpdateName();
#endif
            orientations.Add(handleOrientation);
            return handleOrientation;
        }

        public virtual float GetDefaultAxisLocalPosition()
        {
            if (axisLength == 0) return 0;
            else return defaultGrabAxisRatio * (axisLength / 2);
        }

        public virtual Vector3 GetDefaultAxisPosition(Side side)
        {
            return this.transform.TransformPoint(0, GetDefaultAxisLocalPosition(), 0);
        }

        public virtual HandlePose GetDefaultOrientation(Side side)
        {
            if (side == Side.Right && orientationDefaultRight)
            {
                return orientationDefaultRight;
            }
            if (side == Side.Left && orientationDefaultLeft)
            {
                return orientationDefaultLeft;
            }
            Debug.LogError("No default orientation found! Please check the prefab " + this.transform.parent.name + "/" + this.name);
            return null;
        }

        public virtual HandlePose GetNearestOrientation(Transform grip, Side side)
        {
            float higherDot = -Mathf.Infinity;
            HandlePose orientationResult = null;
            foreach (HandlePose orientation in orientations)
            {
                if (orientation.side == side)
                {
                    float dot = Vector3.Dot(grip.forward, orientation.transform.rotation * Vector3.forward) + Vector3.Dot(grip.up, orientation.transform.rotation * Vector3.up);
                    if (dot > higherDot)
                    {
                        higherDot = dot;
                        orientationResult = orientation;
                    }
                }
            }
            return orientationResult;
        }

        public virtual bool IsAllowed(Side side)
        {
            foreach (HandlePose orientation in orientations)
            {
                if (side == orientation.side)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void CalculateReach()
        {
            float farthestDamagerDist = 0;
            foreach (ColliderGroup colliderGroup in this.GetComponentInParent<Item>().GetComponentsInChildren<ColliderGroup>())
            {
                foreach (Collider collider in colliderGroup.GetComponentsInChildren<Collider>())
                {
                    Vector3 farthestPoint = collider.ClosestPointOnBounds(this.transform.position + (this.transform.up.normalized * 10));
                    float dist = this.transform.InverseTransformPoint(farthestPoint).y;
                    if (dist > farthestDamagerDist)
                    {
                        farthestDamagerDist = dist;
                    }
                }
            }
            reach = farthestDamagerDist - GetDefaultAxisLocalPosition();
        }

        public virtual void SetUpdatePoses(bool active)
        {
            updatePosesAutomatically = active;
        }

        // These methods intentionally exposed for public SDK use
        public virtual void Release()
        {
        }

        protected virtual void ForcePlayerGrab()
        {
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(new Vector3(0, GetDefaultAxisLocalPosition(), 0), reach);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(new Vector3(0, GetDefaultAxisLocalPosition(), 0), 0.03f);
        }
    }
}
