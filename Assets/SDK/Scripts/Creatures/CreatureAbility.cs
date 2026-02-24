using UnityEngine;

using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    //[CreateAssetMenu(menuName = "ThunderRoad/Creatures/Golem ability/Generic")]
    [DeclareBoxGroup("Damage", Title = "Damage")]
    [DeclareHorizontalGroup("Damage/Horiz1")]
    [DeclareHorizontalGroup("Damage/Horiz2")]
    public class CreatureAbility : ScriptableObject
    {
        [Group("Damage")]
        public bool multiHit = true;
        [Group("Damage/Horiz1")]
        public float contactDamage;
        [Group("Damage/Horiz1")]
        public bool breakBreakables;
        [Group("Damage/Horiz2")]
        public float contactForce;
        [Group("Damage/Horiz2")]
        public bool forceUngrip;

        public Creature creature { get; protected set; }

        public virtual void Setup(Creature creature)
        {
            this.creature = creature;
        }
    }
}
