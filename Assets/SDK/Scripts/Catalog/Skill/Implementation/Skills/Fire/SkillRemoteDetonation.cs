using System;
using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Detonation", Title = "Detonation")]
    public class SkillRemoteDetonation : SpellSkillData
    {
        [Group("Detonation")]
        [Dropdown(nameof(GetAllEffectID))]
        public string explosionEffectId = "RemoteDetonation";
        [NonSerialized]
        public EffectData explosionEffectData;

        [Group("Detonation")]
        public float playerDamage = 10f;
        [Group("Detonation")]
        public float enemyDamage = 50f;
        [Group("Detonation")]
        public float radius = 2.5f;
        [Group("Detonation")]
        public float pushMinRadius = 1.5f;
        [Group("Detonation")]
        public float force = 10f;
        [Group("Detonation")]
        public float breakForce = 50f;

        [Group("Detonation")]
        public ForceMode forceMode = ForceMode.Impulse;

        [Group("Detonation")]
        public AnimationCurve hapticAnimationCurve;
        
        protected SkillBurning ignitionData;
        protected GameData.HapticClip hapticClip;
    }
}
