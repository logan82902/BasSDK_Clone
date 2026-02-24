using System.Collections;
using System.Collections.Generic;
using TriInspector;
using ThunderRoad.Skill.Spell;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace ThunderRoad.Skill.SpellMerge
{
    [DeclareBoxGroup("Tornado", Title = "Tornado")]
    [DeclareBoxGroup("Preview", Title = "Preview")]
    [DeclareBoxGroup("Hand Gesture", Title = "Hand Gesture")]
    [DeclareBoxGroup("Force", Title = "Force")]
    [DeclareBoxGroup("Bolts", Title = "Bolts")]
    public class SpellMergeGravnado : SpellMergeData
    {

        [Group("Tornado")]
        [Dropdown(nameof(GetAllEffectID))]
        public string tornadoId;

        [Group("Tornado")]
        public float tornadoDuration = 10f;

        [Group("Tornado")]
        public float aimDistance = 3.5f;

        [Group("Preview")]
        [Dropdown(nameof(GetAllEffectID))]
        public string previewEffectId = "SpellLightningBoltZapSingle";

        protected EffectData previewEffectData;

        [Group("Preview")]
        [Dropdown(nameof(GetAllEffectID))]
        public string previewLoopEffectId;
        protected EffectData previewLoopEffectData;
        [Group("Preview")]
        public float minChargeForPreview = 0.3f;
        [Group("Preview")]
        public float previewBoltDelay = 1f;

        [Group("Preview")]
        public float previewZapBoltDelay = 2f;

        [Group("Preview")]
        public float previewZapForce = 10f;

        [Group("Preview")]
        public Gradient previewBoltGradient;

        [Group("Preview")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string electrocuteStatusId = "Electrocute";

        protected StatusData electrocuteStatusData;

        [Group("Preview")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string floatingStatusId = "Floating";
        protected StatusData floatingStatusData;

        [Group("Hand Gesture")]
        public float handSeparationMaxAngle = 45f;

        [Group("Force")]
        public float radius = 5f;

        [Group("Force")]
        public AnimationCurve radialForce = Utils.Curve(0, 1, 1, 1).Multiply(-50);
        [Group("Force")]
        public AnimationCurve swirlForce = Utils.Curve(0, 1, 1, 1).Multiply(50);

        [Group("Force")]
        public AnimationCurve linearForce = Utils.Curve(0, 1, 1, 1).Multiply(50);

        [Group("Bolts")]
        [MinMaxSlider(0, 5, true)]
        public Vector2 boltDelay = new(1, 3);

        [Group("Bolts")]
        public string thunderboltId = "Thunderbolt";

        protected SkillThunderbolt thunderboltData;

        [Group("Bolts")]
        public float boltRadius = 10f;
    
        protected EffectData tornadoData;
        protected float lastPreviewBolt;
        protected float lastPreviewZapBolt;
        protected Coroutine routine;
        public float yeetRadius = 1f;
        public float yeetHeight = 10f;
        public static bool tornadoActive;

    }
}
