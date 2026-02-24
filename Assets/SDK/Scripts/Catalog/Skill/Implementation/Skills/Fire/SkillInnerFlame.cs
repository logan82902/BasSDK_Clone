
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Explosion", Title = "Explosion")]
    public class SkillInnerFlame : SpellSkillData
    {
        [Group("Explosion")]
        public float explosionRadius = 3;
        [Group("Explosion")]
        public LayerMask explosionLayerMask;
        [Group("Explosion")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string explosionStatusId;

        [NonSerialized]
        public StatusData explosionStatus;
        [Group("Explosion")]
        public float explosionStatusDuration = 5;
        [Group("Explosion")]
        [Dropdown(nameof(GetAllEffectID))]
        public string explosionEffectId;
        [NonSerialized]
        public EffectData explosionEffectData;
        [Group("Explosion")]
        public float explosionForce = 10f;
        [Group("Explosion")]
        public float explosionPlayerForce = 5f;
    }
}
