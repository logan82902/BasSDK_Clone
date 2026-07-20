using System;
using UnityEngine;

using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    [DeclareBoxGroup("Split/Left/Wheel display", Title = "Wheel display")]
    [DeclareBoxGroup("Split/Left/AI", Title = "AI")]
    [DeclareBoxGroup("Split/Left/Mana", Title = "Mana")]
    public class SpellCastData : SpellData
    {
        [Group("Split/Left/Wheel display")]
        public string wheelDisplayName;

        [Group("Split/Left/Wheel display")]
        public bool hasOrder = false;
        [Group("Split/Left/Wheel display")]
        public int order;

        [HideInInspector]
        public int? Order => hasOrder ? order : null;
        [Group("Split/Left/Wheel display")]
        [Dropdown(nameof(GetAllEffectID))]
        public string iconEffectId;
        [NonSerialized]
        public EffectData iconEffectData;
        [Group("Split/Left/AI")]
        public CastType aiCastType;
        [Group("Split/Left/AI")]
        [EnableIf(nameof(aiCastType), CastType.CastLoop)]
        public float loopMaxDuration = 5f;
        [Group("Split/Left/AI")]
        public float aiCastMinDistance = 1;
        [Group("Split/Left/AI")]
        public float aiCastMaxDistance = 20;
        [Group("Split/Left/AI")]
        public float aiCastGestureLength = 0.7f;

        public enum CastType
        {
            CastSimple,
            CastLoop,
        }

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
