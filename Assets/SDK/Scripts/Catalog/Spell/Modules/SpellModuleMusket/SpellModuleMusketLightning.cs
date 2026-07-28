using System;
using System.Collections.Generic;
using ThunderRoad.Skill.Spell;
using TriInspector;
using UnityEngine;
using Newtonsoft.Json;

namespace ThunderRoad
{
    public class SpellModuleMusketLightning : SpellModuleMusket<SpellCastLightning>
    {
        public static readonly Type[] MUSKET_IGNORED_EFFECT_MODULES = { typeof(EffectModuleAudio) };
        
        [Dropdown(nameof(GetAllSkillID))]
        public string musketThunderboltId;
        [JsonIgnore]
        public SkillThunderbolt musketThunderboltData;

        [Dropdown(nameof(GetAllEffectID))]
        public string musketProjectileEffectId;
        [JsonIgnore]
        public EffectData musketProjectileEffectData;

        [Dropdown(nameof(GetAllItemID))]
        public string musketProjectileId;
        [JsonIgnore]
        public ItemData musketProjectileData;

        [Dropdown(nameof(GetAllDamagerID))]
        public string musketProjectileDamagerId;
        [JsonIgnore]
        protected DamagerData musketProjectileDamagerData;

        public int musketProjectileCount = 6;
        public float musketProjectileSpeed = 30;
        public float musketTargetRadius = 3;
        public float musketProjectileAngle = 35;
    }
}