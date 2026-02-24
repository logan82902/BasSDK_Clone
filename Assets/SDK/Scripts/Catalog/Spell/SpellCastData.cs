using System;
using UnityEngine;

using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    [DeclareBoxGroup("Wheel display", Title = "Wheel display")]
    [DeclareBoxGroup("AI", Title = "AI")]
    [DeclareBoxGroup("Mana", Title = "Mana")]
    public class SpellCastData : SpellData
    {
        [Group("Wheel display")]
        public string wheelDisplayName;

        [Group("Wheel display")]
        public bool hasOrder = false;
        [Group("Wheel display")]
        public int order;

        [HideInInspector]
        public int? Order => hasOrder ? order : null;
        [Group("Wheel display")]
        [Dropdown(nameof(GetAllEffectID))]
        public string iconEffectId;
        [NonSerialized]
        public EffectData iconEffectData;
        [Group("AI")]
        public CastType aiCastType;
        [Group("AI")]
        [EnableIf(nameof(aiCastType), CastType.CastLoop)]
        public float loopMaxDuration = 5f;
        [Group("AI")]
        public float aiCastMinDistance = 1;
        [Group("AI")]
        public float aiCastMaxDistance = 20;
        [Group("AI")]
        public float aiCastGestureLength = 0.7f;

        public enum CastType
        {
            CastSimple,
            CastLoop,
        }

        [Group("Mana")]
        public float minMana = 5;

        public new SpellCastData Clone()
        {
            return this.MemberwiseClone() as SpellCastData;
        }


        public override int GetCurrentVersion()
        {
            return 0;
        }
    }
}
