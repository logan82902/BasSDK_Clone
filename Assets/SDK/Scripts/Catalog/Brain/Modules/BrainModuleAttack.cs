using UnityEngine;
using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public abstract class BrainModuleAttack : BrainData.Module
    {
        public Vector2 minMaxTimeBetweenAttack = new Vector2(4, 8);
        public float minAttackBehindChance = 0f;
        [MinMaxSlider(0f, 180f, true)]
        public Vector2 attackBehindAngleRange = new(45f, 165f);

        public float GetAttackChance(float angle) => Mathf.Clamp01(Mathf.Lerp(minAttackBehindChance, 1f, Mathf.Clamp01(Mathf.InverseLerp(attackBehindAngleRange.y, attackBehindAngleRange.x, angle))));

        public float GetAttackChance(Transform target) => GetAttackChance(Vector3.Angle(target.forward, target.position.ToXZ() - creature.transform.position.ToXZ()));

        public enum AttackType
        {
            Melee,
            Magic,
            Ranged
        }

        public enum AttackStage
        {
            WindUp,
            Attack,
            FollowThrough,
            End,
            Cancel
        }
    }
}