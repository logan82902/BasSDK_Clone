using System;
using UnityEngine;

using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [Serializable]
    [DeclareBoxGroup("Merge", Title = "Merge")]
    [DeclareBoxGroup("Charge", Title = "Charge")]
    [DeclareBoxGroup("Hand force", Title = "Hand force")]
    [DeclareBoxGroup("Throw", Title = "Throw")]
    [DeclareBoxGroup("Hand pose", Title = "Hand pose")]
    public class SpellMergeData : SpellData
    {
        [Group("Merge")]
        [Dropdown(nameof(GetAllSpellID))]
        [JsonMergeKey]
        public string leftSpellId;

        [Group("Merge")]
        [Dropdown(nameof(GetAllSpellID))]
        [JsonMergeKey]
        public string rightSpellId;

        [Group("Charge")]
        public float chargeSpeed = 0.2f;
        [Group("Charge")]
        [LabelText("$" + nameof(ChargeSpeedPerSkillLabel))]
        public float chargeSpeedPerSkill = 0.05f;
        [JsonIgnore]
        public string ChargeSpeedPerSkillLabel => SkillPassiveLabel("Charge Speed", chargeSpeedPerSkill);
        [Group("Charge")]
        public float chargeStartHandsRatio = 0.5f;
        [Group("Charge")]
        public float stopSpeed = 0.6f;
        [Group("Charge")]
        public bool stopIfManaDepleted = true;
        [NonSerialized]
        public float currentCharge;

        [Group("Merge")]
        public float handEnterAngle = 30;
        [Group("Merge")]
        public float handEnterDistance = 0.2f;
        [Group("Merge")]
        public float handExitAngle = 90;
        [Group("Merge")]
        public float handExitDistance = 0.6f;
        [Group("Merge")]
        public float handCompletedDistance = 0.001f;
        [Group("Merge")]
        public float minCharge = 0.9f;

        [Group("Merge")]
        public AnimationCurve hapticIntensityCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            // new Utils.CurveBuilder()
            // .WithKey(0, 0)
            // .WithKey(0.83f, 0.4f)
            // .WithKey(0.86f, 0)
            // .WithKey(0.89f, 0)
            // .WithKey(1, 1)
            // .Build();

        [Group("Merge")]
        public float hapticCurveModifier = 0.5f;

        [Group("Hand force")]
        public AnimationCurve handForceCurve = new(new Keyframe[3] { new(0f, 120f), new(0.05f, 180f), new(0.1f, 120f) });
        [Group("Hand force")]
        public AnimationCurve handForceMultiplierDistanceCurve = new(new Keyframe[2] { new(0f, 1f), new(0.2f, 0f) });
        [Group("Hand force")]
        public AnimationCurve handForceChargeCurve = new(new Keyframe[2] { new(0f, 0f), new(1f, 1f) });
        [Group("Hand force")]
        public float handPositionSpringMultiplier = 0.4f;
        [Group("Hand force")]
        public float handPositionDamperMultiplier = 1f;
        [Group("Hand force")]
        public float handLocomotionVelocityCorrectionMultiplier = 0.3f;

        private bool reachedMinCharge = false;

        [Group("Merge")]
        [Dropdown(nameof(GetAllEffectID))]
        public string chargeEffectId;
        protected EffectData chargeEffectData;

        [Group("Merge")]
        [Tooltip("Set to zero for no lerping")]
        public float effectLerpFactor = 0;
        [Group("Merge")]
        [Dropdown(nameof(GetAllEffectID))]
        public string fingerEffectId;
        protected EffectData fingerEffectData;
        
        
        [NonSerialized]
        public EffectData overrideFingerEffect;
        
        [NonSerialized]
        public EffectData overrideChargeEffect;

        [Group("Throw")]
        public bool allowThrow = false;
        [Group("Throw")]
        [Dropdown(nameof(GetAllEffectID))]
        [ShowIf("allowThrow")]
        public string throwEffectId;
        protected EffectData throwEffectData;
        [Group("Throw")]
        public float throwMinHandVelocity = 2f;

        [Group("Hand pose")]
        [Dropdown(nameof(GetAllHandPoseID))]
        public string closeHandPoseId;
        [NonSerialized]
        public HandPoseData closeHandPoseData;

        [Group("Hand pose")]
        [Dropdown(nameof(GetAllHandPoseID))]
        public string openHandPoseId;
        [NonSerialized]
        public HandPoseData openHandPoseData;
        


        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            if (chargeEffectId != null && chargeEffectId != "") chargeEffectData = Catalog.GetData<EffectData>(chargeEffectId);
            if (closeHandPoseId != null && closeHandPoseId != "") closeHandPoseData = Catalog.GetData<HandPoseData>(closeHandPoseId);
            if (openHandPoseId != null && openHandPoseId != "") openHandPoseData = Catalog.GetData<HandPoseData>(openHandPoseId);
            if (fingerEffectId != null && fingerEffectId != "") fingerEffectData = Catalog.GetData<EffectData>(fingerEffectId);
            handForceCurve.postWrapMode = WrapMode.Loop;
        }

        public override int GetCurrentVersion()
        {
            return 0;
        }
    }
}
