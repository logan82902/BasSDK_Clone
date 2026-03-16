
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Flamethrower", Title = "Flamethrower")]
    public class SkillInfernoStaff : SpellSkillData
    {
        [Group("Flamethrower")]
        [Dropdown(nameof(GetAllEffectID))]
        public string flamethrowerEffectId = "Flamethrower";

        [JsonIgnore]
        public EffectData flamethrowerEffectData;

        [Group("Flamethrower")]
        public float sphereRadius = 3f;
        [Group("Flamethrower")]
        public float tickDelay = 0.3f;
        [Group("Flamethrower")]
        public float damagePerTick = 1;
        [Group("Flamethrower")]
        public LayerMask layerMask;
        [Group("Flamethrower")]
        public float forceMult = 2;
        [Group("Flamethrower")]
        public float imbueTransferPerSecond = 10000;
        [Group("Flamethrower")]
        public float heatTransferPerSecond = 70;
        [Group("Flamethrower")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string burningId = "Burning";
        [Group("Flamethrower")]
        public AnimationCurve pushbackCurve = AnimationCurve.EaseInOut(0, 2, 0.5f, 1);
        [Group("Flamethrower")]
        public float pushbackForce = 2;
        [Group("Flamethrower")]
        public ForceMode pushbackForceMode = ForceMode.Acceleration;

        [NonSerialized]
        public StatusData burning;
    }
}