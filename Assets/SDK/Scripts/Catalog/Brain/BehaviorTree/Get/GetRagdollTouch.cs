using UnityEngine;
using TriInspector;

namespace ThunderRoad.AI.Get
{
    [DeclareBoxGroup("Outputs", Title = "Outputs")]
    [DeclareHorizontalGroup("Outputs/Horiz", Sizes = new float[] { 215f, 0f })]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class GetRagdollTouch : ActionNode
    {
        const string SkulkerSkillId = "Skulker";
        public enum TouchDamage
        {
            Either,
            NoDamage,
            OnlyIfDamage
        }

        public enum TouchSource
        {
            AnyCreature,
            OnlyEnemy,
            OnlyFriend,
            OnlyPlayer,
        }

        public enum TouchType
        {
            AnyTouch,
            OnlyPart,
            OnlyItem
        }


        public enum BodyState
        {
            Any,
            StandingAlive,
            DeadOrDestabilized,
            DeadOnly,
        }

        public TouchDamage damageCondition = TouchDamage.Either;
        public TouchSource factionCondition = TouchSource.OnlyEnemy;
        public TouchType typeCondition = TouchType.AnyTouch;
        public BodyState bodyStateCondition = BodyState.StandingAlive;
        public float rememberDuration = 20f;
        [Group("Outputs/Horiz")]
        [LabelWidth(200)]
        public bool outputCreatureAsCurrentTarget = true;
        [Group("Outputs/Horiz")]
        [LabelWidth(200)]
        [DisableIf(nameof(outputCreatureAsCurrentTarget))]
        public string outputCreatureVariableName = "";
        [Group("Outputs/Horiz")]
        [LabelWidth(200)]
        public string outputPartTransformVariableName = "";

        public GetRagdollTouch() { }

    }
}