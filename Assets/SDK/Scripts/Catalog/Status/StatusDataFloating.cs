
using System;
using System.Collections.Generic;
using ThunderRoad.Skill.Spell;
using UnityEngine;
using TriInspector;

namespace ThunderRoad
{
    [DeclareBoxGroup("Physic Modifier", Title = "Physic Modifier")]
    [DeclareBoxGroup("Force", Title = "Force")]
    [DeclareBoxGroup("Torque", Title = "Torque")]
    [DeclareBoxGroup("Options", Title = "Options")]
    [DeclareBoxGroup("Slam", Title = "Slam")]
    public class StatusDataFloating : StatusData
    {
        [Group("Physic Modifier")]
        public float gravityMult = 0;
        [Group("Physic Modifier")]
        public float dragRatio = 2f;
        [Group("Physic Modifier")]
        public float gravityMultPlayer = 0.3f;
        [Group("Physic Modifier")]
        public float dragRatioPlayer = 3f;
        [Group("Physic Modifier")]
        public float massRatio = -1f;
        [Group("Physic Modifier")]
        public float angularDragRatio = 2f;
        [Group("Force")]
        public float upwardsForceCreature = 30f;
        [Group("Force")]
        public float upwardsForceItem = 3f;

        [Group("Force")]
        public float upwardsForcePlayer = 3f;
        [Group("Force")]
        public ForceMode upwardsForceMode = ForceMode.VelocityChange;

        [Group("Torque")]
        public float creatureTorqueMult = 60;
        [Group("Torque")]
        public float itemTorqueMult = 20;
        [Group("Torque")]
        public ForceMode torqueForceMode = ForceMode.VelocityChange;
        [Group("Torque")]
        public bool constantTorque = false;
        [Group("Options")]
        public bool destabilizeCreatures = false;
        [Group("Options")]
        public float environmentDamageMult = 0.1f;
        [Group("Slam")]
        public bool allowSlamOnEnd = false;
        [Group("Slam")]
        public float nonRootSlamMultiplier = 0.2f;
        [Group("Slam")]
        [Dropdown(nameof(GetAllEffectID))]
        public string slamEffectId;

        [Group("Slam")]
        [Dropdown(nameof(GetAllSkillID))]
        public string lithobrakeId = "Lithobrake";
        [NonSerialized]
        public SkillHoverSlam lithobrakeData;

        public TriDropdownList<string> GetAllSkillID() => Catalog.GetDropdownAllID(Category.Skill);


        [NonSerialized]
        public EffectData slamEffectData;

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            slamEffectData = Catalog.GetData<EffectData>(slamEffectId);
            lithobrakeData = Catalog.GetData<SkillHoverSlam>(lithobrakeId);
        }
    }

}
