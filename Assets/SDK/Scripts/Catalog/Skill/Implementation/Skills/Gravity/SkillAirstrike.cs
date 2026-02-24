using TriInspector;
using System.Collections.Generic;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Fall Damage", Title = "Fall Damage")]
    [DeclareBoxGroup("Dive", Title = "Dive")]
    [DeclareBoxGroup("Shockwave", Title = "Shockwave")]
    [DeclareBoxGroup("Shake", Title = "Shake")]
    public class SkillAirstrike : SkillSpellPunch
    {
        [Group("Fall Damage")]
        public float fallDamageScale = 0f;
        [Group("Dive")]
        public float maxAngle = 70f;
        [Group("Dive")]
        public float minDownwardVelocity = 5f;
        [Group("Dive")]
        public float minHeight = 4f;
        [Group("Dive")]
        public float maxHeightDistance = 3f;
        [Group("Dive")]
        [Dropdown(nameof(GetAllEffectID))]
        public string whooshEffectId;

        protected EffectData whooshEffectData;
        [Group("Shockwave")]
        public float force = 4f;
        [Group("Shockwave")]
        public float upwardsModifier = 0f;
        [Group("Shockwave")]
        public int shockwavePushLevel = 2;
        [Group("Shockwave")]
        public float distanceMult = 1f;
        [Group("Shockwave")]
        public float shockwaveDamage = 30f;
        [Group("Shockwave")]
        [Dropdown(nameof(GetAllEffectID))]
        public string shockwaveEffectId;

        protected EffectData shockwaveEffectData;

        [Group("Shockwave")]
        [Dropdown(nameof(GetAllEffectID))]
        public string playerEffectId;

        protected EffectData playerEffectData;
        [Group("Shockwave")]
        public float radius = 4f;
        [Group("Shake")]
        public bool shake = true;
        [Group("Shake")]
        [ShowIf(nameof(shake))]
        public float shakeDuration = 0.5f;
        [Group("Shake")]
        [ShowIf(nameof(shake))]
        public float shakeIntensity = 1;
        [Group("Shake")]
        [ShowIf(nameof(shake))]
        [MinMaxSlider(0, 0.1f)]
        public Vector2 minMaxShakeIntensity = new(0.005f, 0.01f);
        [Group("Shake")]
        [ShowIf(nameof(shake))]
        public AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    }
}