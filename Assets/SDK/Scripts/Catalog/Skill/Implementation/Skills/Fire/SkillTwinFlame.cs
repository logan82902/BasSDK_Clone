using System;
using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Skills", Title = "Skills")]
    [DeclareBoxGroup("Conditions", Title = "Conditions")]
    [DeclareBoxGroup("Link", Title = "Link")]
    [DeclareBoxGroup("Wall", Title = "Wall")]
    public class SkillTwinFlame : SpellSkillData
    {
        [Group("Skills")]
        [Dropdown(nameof(GetAllSkillID))]
        public string remoteDetonationId;

        [Group("Conditions")]
        public float detonateWindow = 0.1f;

        [Group("Conditions")]
        public float maxRange = 15f;

        [Group("Link")]
        [Dropdown(nameof(GetAllEffectID))]
        public string linkEffectId;

        [NonSerialized]
        public EffectData linkEffectData;

        [Group("Wall")]
        [Dropdown(nameof(GetAllEffectID))]
        public string dropEffectId;

        [NonSerialized]
        public EffectData dropEffectData;

        [Group("Wall")]
        [Dropdown(nameof(GetAllEffectID))]
        public string wallEffectId;

        [NonSerialized]
        public EffectData wallEffectData;
        [Group("Wall")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string statusId;

        [NonSerialized]
        public StatusData status;

        [Group("Wall")]
        public float thickness = 0.5f;

        [Group("Wall")]
        public float heatRadius = 1f;

        [Group("Wall")]
        public float height = 1f;

        [Group("Wall")]
        public float heatPerSecond;

        [Group("Wall")]
        public float duration = 10;

        [Group("Wall")]
        public bool allowXZMotion = false;
    }

    public class FlameWall : ThunderBehaviour
    {
        public override ManagedLoops EnabledManagedLoops => ManagedLoops.Update;
        public StatusData statusData;
        public float statusDuration;
        public float heatPerSecond;
        public bool ignorePlayer = true;
        
    }
}
