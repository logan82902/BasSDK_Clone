using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "ThunderRoad/Creatures/Golem/Self-imbue config")]
    [DeclareBoxGroup("Self-imbue", Title = "Self-imbue")]
    public class GolemImbue : GolemAbility
    {
#if UNITY_EDITOR
        [Group("Self-imbue")]
#endif
        public float selfImbueDuration = 10f;
#if UNITY_EDITOR
        [Group("Self-imbue")]
#endif
        public float contactDamage = 5f;
#if UNITY_EDITOR
        [Group("Self-imbue")]
#endif
        public float timeBetweenDamage = 0.5f;
#if UNITY_EDITOR
        [Group("Self-imbue")]
#endif
        public bool ungripOnDamage = false;
#if UNITY_EDITOR
        [Group("Self-imbue")]
#endif
        public bool disarmAllowed = false;
#if UNITY_EDITOR
        protected TriDropdownList<string> GetAllEffectIDs() => Catalog.GetDropdownAllID(Category.Effect);

        [Group("Self-imbue")]
        [Dropdown(nameof(GetAllEffectIDs))]
#endif
        public string bodyEffectID;
#if UNITY_EDITOR
        [Group("Self-imbue")]
        [Dropdown(nameof(GetAllEffectIDs))]
#endif
        public string startEffectID;
#if UNITY_EDITOR
        [Group("Self-imbue")]
#endif
        public List<Golem.InflictedStatus> appliedStatuses = new();

    }
}
