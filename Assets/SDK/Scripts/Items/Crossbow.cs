using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace ThunderRoad
{
    [DeclareBoxGroup("Bow")]
    [DeclareHorizontalGroup("Horiz1")]
    [DeclareBoxGroup("Horiz1/Trigger", Title = "Trigger")]
    [DeclareBoxGroup("Horiz1/Spanning", Title = "Spanning")]
    [DeclareBoxGroup("LockLoad", Title = "Span Lock and Loading")]
    [DeclareHorizontalGroup("LockLoad/Horiz", Sizes = new float[] { 230f, 180f, 0f})]
    [DeclareBoxGroup("AutoSpanOptions", Title = "Auto Span Options")]
    [DeclareHorizontalGroup("AutoSpanOptions/Horiz1")]
    [DeclareBoxGroup("ButtonSpanOptions", Title = "Button Span Options")]
    [DeclareHorizontalGroup("ButtonSpanOptions/Horiz1")]
    [DeclareFoldoutGroup("Events")]
    public class Crossbow : ThunderBehaviour
    {
        [Group("Bow")]
        [AutoLabelWidth(MinWidth = 150f)]
        public Item item;
        [Group("Bow")]
        [AutoLabelWidth(MinWidth = 150f)]
        public BowString bowString;
        [Group("Bow")]
        [AutoLabelWidth(MinWidth = 150f)]
        public StringGrab stringGrabAllowance = StringGrab.Unloaded;
        [Group("Horiz1/Trigger")]
        [AutoLabelWidth(AddedWidth = 15f)]
        public ImbueUseButton triggerButton = ImbueUseButton.Use;
        [Group("Horiz1/Trigger")]
        [ListDrawerSettings(IgnoreButtonSpacing = true)]
        public List<Handle> triggerHandles;
        [Group("Horiz1/Trigger")]
        [AutoLabelWidth(AddedWidth = 30f)]
        public bool allowDryFire = true;
        [Group("Horiz1/Spanning")]
        [AutoLabelWidth(AddedWidth = 15f)]
        public SpanType spanType = SpanType.Manual;
        [Group("Horiz1/Spanning")]
        [ListDrawerSettings(IgnoreButtonSpacing = true)]
        public List<Handle> spanHandles;
        [Group("Horiz1/Spanning")]
        [AutoLabelWidth(AddedWidth = 30f)]
        public bool tkCountsAsHeld = false;
        [Group("Horiz1/Spanning")]
        public AudioContainerPlayer autoSpanLoopAudio;
        [NonSerialized]
        public bool autoSpanLoopAudioAddressed = false;
        [Group("LockLoad")]
        [AutoLabelWidth(MinWidth = 150f)]
        [InlineButton(nameof(LockString))]
        public float pullRatioToLock = 0.95f;
        [Group("LockLoad/Horiz")]
        [AutoLabelWidth(MinWidth = 150f)]
        public bool autoLockIfHeld = false;
        [Group("LockLoad/Horiz")]
        [AutoLabelWidth(AddedWidth = 7f)]
        public bool createRigidJointOnLock = true;
        [NonSerialized]
        [ShowInInspector]
        [Group("LockLoad/Horiz")]
        [ReadOnly]
        [AutoLabelWidth(AddedWidth = 7f)]
        public FixedJoint lockJoint;
        [Group("LockLoad")]
        [AutoLabelWidth(MinWidth = 150f)]
        public AudioContainer audioContainerLock;
        [NonSerialized]
        public bool lockAudioAddressed = false;
        [Group("LockLoad")]
        [AutoLabelWidth(MinWidth = 150f)]
        public CrossbowLoader loader;
        [Group("AutoSpanOptions/Horiz1")]
        [AutoLabelWidth(AddedWidth = 15f)]
        public bool requireSpanHandleHeld = true;
        [Group("AutoSpanOptions/Horiz1")]
        [AutoLabelWidth(AddedWidth = 15f)]
        public bool resetUnlessLocked = true;
        [Group("AutoSpanOptions")]
        [AutoLabelWidth(AddedWidth = 15f)]
        [Tooltip("Measured in percent (from 0.0 being 0% and 1.0 being 100%) per second. Span speed of 0.5 spans in two seconds, speed of 1 spans in one second, span speed of 2 spans in two seconds, etc.")]
        public float spanSpeed = 0f;
        [Group("ButtonSpanOptions")]
        [AutoLabelWidth(AddedWidth = 15f)]
        public ImbueUseButton spanButton = ImbueUseButton.None;
        [Group("ButtonSpanOptions/Horiz1")]
        [AutoLabelWidth]
        [InlineButton(nameof(IncreasePull))]
        public float spanPerPress = 0.1f;
        [Group("ButtonSpanOptions/Horiz1")]
        [AutoLabelWidth]
        public float spanPressCooldown = 0.1f;
        [Group("Events")]
        public UnityEvent onSpanButtonPress;
        [Group("Events")]
        public UnityEvent onSpanComplete;
        [Group("Events")]
        public UnityEvent onLoaderChange;
        [Group("Events")]
        public UnityEvent onLoadBolt;
        [Group("Events")]
        public UnityEvent onLoaderEmpty;
        [Group("Events")]
        public UnityEvent onShoot;
        [Group("Events")]
        public UnityEvent onShootLastShot;

        private bool usesSpanSpeed => spanType != SpanType.Manual && spanType != SpanType.Button;
        private bool requiresButton => spanType == SpanType.Button || spanType == SpanType.HeldButton;
        private BoolHandler spanHandleHeld = new BoolHandler(false);
        private BoolHandler spanButtonHeld = new BoolHandler(false);
        private bool locked;

        public enum StringGrab
        {
            Always,
            Unloaded,
            Never,
        }

        public enum SpanType
        {
            Manual = 0, // requires the player to grab the string or otherwise pull it back to span the crossbow
            Button = 1, // spans by a set amount per button press, with configurable cooldown
            HeldButton = 2, // automatically spans the crossbow while the button is held, using the span speed
            AutoLoad = 3, // automatically spans the crossbow if a bolt is loaded or if it has a loader with a bolt to load
            Automatic = 4, // automatically spans the crossbow back to 100%, using the span speed, no matter what
        }

        public void SetLoader(CrossbowLoader loader)
        {
        }

        public void SetSpanType(int spanType)
        {
            this.spanType = (SpanType)spanType;
        }

        private void OnValidate()
        {
            TryGetComponents();
        }

        private void TryGetComponents()
        {
            if (item == null) item = GetComponent<Item>();
            if (bowString == null) bowString = GetComponentInChildren<BowString>();
            // crossbow should always control the string grabbing state
            if (bowString) bowString.stringAlwaysGrabbable = true;
            if (autoSpanLoopAudio != null) autoSpanLoopAudio.GetComponent<AudioSource>().loop = true;
        }

        public void IncreasePull() => IncreasePull(spanPerPress);

        [Button]
        [Group("LockLoad")]
        public void IncreasePull(float amount)
        {
        }

        public void LockString()
        {
        }

        [Button]
        [Group("Horiz1/Trigger")]
        public void TryShoot()
        {
        }

    }
}
