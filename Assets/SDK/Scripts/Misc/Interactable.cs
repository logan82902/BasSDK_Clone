using UnityEngine;
using System.Collections.Generic;
using System;

using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Misc/Interactable.html")]
    [DeclareBoxGroup("Data", Title = "Interactable")]
    [DeclareBoxGroup("Touch")]
    public class Interactable : ThunderBehaviour
    {

        [Group("Data")]
        [Dropdown(nameof(GetAllInteractableID))]
        [Tooltip("(Only needed for non-json handles)\nInsert Interactable ID here.")]
        public string interactableId;

        [Group("Touch")]
        [Tooltip("What hand is allowed to grab the handle.")]
        public HandSide allowedHandSide = HandSide.Both;

        [Group("Data")]
        public Transform highlighterTransform;

        public enum HandSide
        {
            Both,
            Right,
            Left,
        }

        [Group("Touch")]
        [Tooltip("The length of which the player can grab along.\nIf >0, a button will appear and allow you to adjust the length along its points")]
        [Min(0f)]
        public float axisLength = 0;
        [Group("Touch")]
        [Tooltip("The radius of which the player can grab the handle")]
        [Min(0f)]
        public float touchRadius = 0.1f;
        [Group("Touch")]
        [Tooltip( "When the player's hand is within the range of multiple interactables, the closest one is prioritized. \n \nArtifical distance is a fake distance added to the player's hand while checking which interactable is the nearest.\nSetting this to a high value gives it a low priority when working out which interactable to use, while a low (or negative) value will give it a high priority compared to other interactables. \n \n Generally, you can leave this value at 0.")]
        public float artificialDistance = 0f;
        [Group("Touch")]
        [Tooltip("Determines the center of the touchRadius")]
        public Vector3 touchCenter;

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.matrix = this.transform.localToWorldMatrix;
            if (axisLength > 0)
            {
                Gizmos.color = Color.white;
                Common.DrawGizmoArrow(Vector3.zero, Vector3.up * (axisLength / 2), Common.HueColourValue(HueColorName.White), 0.05f);
                Common.DrawGizmoArrow(Vector3.zero, -Vector3.up * (axisLength / 2), Common.HueColourValue(HueColorName.White), 0.05f);
                Common.DrawGizmoCapsule(transform.position, transform.up, axisLength + touchRadius, touchRadius, Common.HueColourValue(HueColorName.White));
                Common.DrawGizmoCapsule(transform.position, transform.up, axisLength + touchRadius, touchRadius, Common.HueColourValue(HueColorName.White));
            }
            else
            {
                Gizmos.matrix = this.transform.localToWorldMatrix;
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(touchCenter, touchRadius);
            }
        }

        protected override void ManagedOnDisable()
        {
            base.ManagedOnDisable();
            TouchEndPlayerHands(false);
        }

        public virtual void SetTouch(bool active)
        {
        }

        protected void TouchEndPlayerHands(bool active)
        {
            
        }

        public virtual void SetTouchPersistent(bool active)
        {
        }

        public static bool showTouchHighlighter = true;

        [Group("Data")]
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public InteractableData data;

        [NonSerialized]
        public Collider touchCollider;
        [NonSerialized]
        public bool touchActive = true;

        private float previousCenterDistance = Mathf.Infinity;

        protected bool initialized;

        public delegate void ActionDelegate(RagdollHand ragdollHand, Action action);
        public event ActionDelegate OnTouchActionEvent;

        public enum Action
        {
            UseStart,
            UseStop,
            AlternateUseStart,
            AlternateUseStop,
            Grab,
            Ungrab,
        }

        public class InteractionResult
        {
            public InteractionResult(RagdollHand ragdollHand, bool isInteractable, bool showHint = false, string hintTitle = null, string hintDesignation = null, Color? hintColor = null, AudioClip audioClip = null, string altHintDesignation = null)
            {
                this.ragdollHand = ragdollHand;
                this.isInteractable = isInteractable;
                this.showHint = showHint;
                this.hintTitle = hintTitle;
                this.hintDesignation = hintDesignation;
                this.altHintDesignation = altHintDesignation;
                this.hintColor = hintColor ?? Color.white;
                this.audioClip = audioClip;
            }
            public RagdollHand ragdollHand;
            public bool isInteractable;
            public bool showHint;
            public string hintTitle;
            public string hintDesignation;
            public string altHintDesignation;
            public Color hintColor;
            public AudioClip audioClip;
        }

        [Serializable]
        public class HighlightParams
        {
            public HighlightParams Copy()
            {
                return (HighlightParams)this.MemberwiseClone();
            }
            public Color touchColor = Color.yellow;
            public Color proximityColor = Color.white;
            public float proximityDistance = 2;
        }

        public TriDropdownList<string> GetAllInteractableID() => Catalog.GetDropdownAllID(Category.Interactable);

    }
}
