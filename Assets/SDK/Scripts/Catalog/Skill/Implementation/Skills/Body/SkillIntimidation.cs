using System;
using UnityEngine;


using TriInspector;

namespace ThunderRoad.Skill
{
    [DeclareBoxGroup("Intimidating kills", Title = "Intimidating kills")]
    [DeclareHorizontalGroup("Intimidating kills/Horiz1")]
    [DeclareHorizontalGroup("Intimidating kills/Horiz2")]
    [DeclareHorizontalGroup("Intimidating kills/Horiz3")]
    [DeclareHorizontalGroup("Intimidating kills/Horiz4")]
    public class SkillIntimidation : SkillData
    {
        // to be used to temporarily add a modifier to a fear value
        public static Vector2Int scareModifier = new Vector2Int();

#if UNITY_EDITOR
        [Group("Intimidating kills")]
#endif
        public float intimidateRadius = 6f;

#if UNITY_EDITOR
        [Group("Intimidating kills/Horiz1")]
        [InlineProperty]
        [HideLabel]
        [HideReferencePicker]
        [Title("Generic kill")]
#endif
        public FearAction genericKill = new();
#if UNITY_EDITOR
        [Group("Intimidating kills/Horiz1")]
        [InlineProperty]
        [HideLabel]
        [HideReferencePicker]
        [Title("Dismember kill")]
#endif
        public FearAction dismember = new();
#if UNITY_EDITOR
        [Group("Intimidating kills/Horiz2")]
        [InlineProperty]
        [HideLabel]
        [HideReferencePicker]
        [Title("Decapitate kill")]
#endif
        public FearAction decapitate = new();
#if UNITY_EDITOR
        [Group("Intimidating kills/Horiz2")]
        [InlineProperty]
        [HideLabel]
        [HideReferencePicker]
        [Title("Burn kill")]
#endif
        public FearAction burnKill = new();
#if UNITY_EDITOR
        [Group("Intimidating kills/Horiz3")]
        [InlineProperty]
        [HideLabel]
        [HideReferencePicker]
        [Title("Electrocution kill")]
#endif
        public FearAction electrocutionKill = new();
#if UNITY_EDITOR
        [Group("Intimidating kills/Horiz3")]
        [InlineProperty]
        [HideLabel]
        [HideReferencePicker]
        [Title("Impact kill")]
#endif
        public FearAction impactKill = new();
#if UNITY_EDITOR
        [Group("Intimidating kills/Horiz4")]
        [InlineProperty]
        [HideLabel]
        [HideReferencePicker]
        [Title("Impale kill")]
#endif
        public FearAction impaleKill = new();
#if UNITY_EDITOR
        [Group("Intimidating kills/Horiz4")]
        [InlineProperty]
        [HideLabel]
        [HideReferencePicker]
        [Title("Limb tear kill")]
#endif
        public FearAction ripping = new FearAction();

        [Serializable]
        [DeclareHorizontalGroup("Fields")]
        public class FearAction
        {
#if UNITY_EDITOR
            [Group("Fields")]
            [MinMaxSlider(0, 10, ShowFields = true)]
            [LabelWidth(165f)]
#endif
            public Vector2Int minMaxCreaturesScared = new(1, 2);
#if UNITY_EDITOR
            [Group("Fields")]
            [LabelWidth(65f)]
#endif
            public bool alliesOnly = true;

        }

    }
}
