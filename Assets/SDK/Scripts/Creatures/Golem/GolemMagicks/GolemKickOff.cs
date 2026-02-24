using UnityEngine;

using TriInspector;

namespace ThunderRoad
{
    [CreateAssetMenu(menuName = "ThunderRoad/Creatures/Golem/Kick off config")]
    [DeclareBoxGroup("General", Title = "General")]
    public class GolemKickOff : GolemAbility
    {
#if UNITY_EDITOR
        [Group("General")]
#endif
        public GolemController.AttackMotion motion;
#if UNITY_EDITOR
        [Group("General")]
#endif
        public bool launchClimbers = true;
#if UNITY_EDITOR
        [Group("General")]
#endif
        public float launchSpeed = 2f;
#if UNITY_EDITOR
        [Group("General")]
#endif
        public bool launchVertical = false;

    }
}
