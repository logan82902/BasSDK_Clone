using UnityEngine;
using TriInspector;

namespace ThunderRoad.Skill.Spell
{
    [DeclareBoxGroup("Boost", Title = "Boost")]
    public class BoostSkillData : SkillData
    {
        [Group("Boost")]
        [Dropdown(nameof(GetAllSkillID))]
        public string boostJumpSkillId = "BoostJump";


        public virtual void OnJump(
            SkillBoostJump skill,
            SpellCastGravity gravity,
            Vector3 pushForce,
            Vector3 jumpForce,
            bool onGround)
        {
        }
    }
}
