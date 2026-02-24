using UnityEngine;
using UnityEngine.Profiling;
using TriInspector;


namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Detection", Title = "Detection")]
    public class SkillFocusedReflexes : SkillData
    {
        [Group("Detection")]
        public float radius = 3f;
        [Group("Detection")]
        public float minimumVelocity = 3f;
        [Group("Detection")]
        public float velocityThreatAngle = 40;
        [Group("Detection")]
        public bool noOwnerIsThreat = false;

    }

    public class ProjectileDetector : ThunderBehaviour
    {
        public delegate void ProjectileCountEvent(ProjectileDetector detector);
        public event ProjectileCountEvent OnAnyProjectiles;
        public event ProjectileCountEvent OnNoProjectiles;

        public Creature creature;
        public SkillFocusedReflexes skill;

        private int lastCount;

    }
}
