using TriInspector;
using System;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Ignition", Title = "Ignition")]
    [DeclareBoxGroup("Dependencies", Title = "Dependencies")]
    public class SkillIgnitionBurst : SkillData
    {
        [Group("Ignition")]
        public float igniteMinRadius;

        [Group("Ignition")]
        public float ignitionHeat;

        [Group("Ignition")]
        public AnimationCurve heatFalloffMult = AnimationCurve.Linear(0, 0, 1, 1);

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
        
    }
}
