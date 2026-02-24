using System;
using TriInspector;
using UnityEngine;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Dependencies", Title = "Dependencies")]
    [DeclareBoxGroup("Explosion Force", Title = "Explosion Force")]
    public class SkillRepellingBurst : SkillData
    {
        [Group("Dependencies")]
        [Dropdown(nameof(GetAllSkillID))]
        public string skillRemoteDetonationId = "RemoteDetonation";
        [NonSerialized]
        public SkillRemoteDetonation skillRemoteDetonation;

        [Group("Explosion Force")]
        public float force = 1;
        [Group("Explosion Force")]
        public float forcePlayer = 1;
        [Group("Explosion Force")]
        public float upwardsModifier = 1;

        [Group("Explosion Force")]
        public ForceMode forceMode = ForceMode.VelocityChange;

        [Group("Explosion Force")]
        public int pushLevel = 2;

    }
}
