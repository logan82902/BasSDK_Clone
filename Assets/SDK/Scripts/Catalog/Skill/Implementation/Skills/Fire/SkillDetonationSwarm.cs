using TriInspector;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Sparks", Title = "Sparks")]
    [DeclareBoxGroup("Dependencies", Title = "Dependencies")]
    public class SkillDetonationSwarm : SkillData
    {
        [Group("Sparks")]
        public float damageMultiplier = 0.4f;
        [Group("Sparks")]
        public float fireVelocity = 3f;
        [Group("Sparks")]
        public int baseSparkCount = 3;
        [Group("Sparks")]
        public int sparkCountBody = 1;
        [Group("Sparks")]
        public float homingDelay = 0.5f;

        [Group("Sparks")]
        public bool requireBurningStatus = true;

        [Group("Dependencies")]
        [Dropdown(nameof(GetAllSkillID))]
        public string remoteDetonationSkillId = "RemoteDetonation";

        [NonSerialized]
        public SkillRemoteDetonation remoteDetonationSkill;
        [Group("Dependencies")]
        [Dropdown(nameof(GetAllStatusEffectID))]
        public string burningStatusId = "Burning";
        
        [NonSerialized]
        public StatusDataBurning burning;

        protected SkillRemoteDetonation skillRemoteDetonate;

    }
}
