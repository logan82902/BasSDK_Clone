using System;
using ThunderRoad.Skill.Spell;
using UnityEngine;

namespace ThunderRoad
{
    public class SpellModuleBombFire : SpellModuleBomb<SpellCastProjectile>
    {
        [NonSerialized]
        public SkillRemoteDetonation bombFireBurstSkill;
        [NonSerialized]
        public SkillTwinFlame bombFlameWallSkill;

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            bombFireBurstSkill = Catalog.GetData<SkillRemoteDetonation>("BombFireBurst");
            bombFlameWallSkill = Catalog.GetData<SkillTwinFlame>("BombFlameWall");
        }

    }
}