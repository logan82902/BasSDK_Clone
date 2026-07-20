using System;
using System.Collections.Generic;
using ThunderRoad.Skill.Spell;
using TriInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ThunderRoad
{
    public class SpellModuleMusketFire : SpellModuleMusket<SpellCastProjectile>
    {
        public float musketDamage = 30f;
        public float musketRadius = 2f;
        public LayerMask musketDecapitateLayerMask;
        public int fireSparkCount = 6;
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string musketStatusId;
        protected StatusData musketStatusData;
        [Dropdown(nameof(GetAllEffectID))]
        public string musketSparkEffectId;
        protected EffectData musketSparkEffectData;
        public float musketForce = 20f;
        public float musketStatusDuration = Mathf.Infinity;
        public float musketStatusParameterCone = 100;
        public float musketStatusParameterParticle = 100;
        public Vector2 musketSparkVelocity = new Vector2(15f, 25f);
        [NonSerialized]
        public SkillTwinFlame twinFlameSkill;

    }
}