using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    [DeclareBoxGroup("Damage", Title = "Damage")]
    [DeclareBoxGroup("Heat", Title = "Heat")]
    [DeclareBoxGroup("Animation", Title = "Animation")]
    [DeclareBoxGroup("Charring", Title = "Charring")]
    [DeclareBoxGroup("Spread", Title = "Spread")]
    [DeclareBoxGroup("Effect", Title = "Effect")]
    public class StatusDataBurning : StatusData
    {
        public const string Heat = "Heat";
        public const string HeatGainMult = "HeatGainMult";
        public const string HeatLossMult = "HeatLossMult";
        public const string Char = "CharAmount";

        [Group("Damage")]
        public float burnDelay = 0.5f;
        [Group("Damage")]
        public float damagePerTick = 0.75f;
        [Group("Damage")]
        public float damagePerTickPlayer = 0.75f;

        [Group("Heat")]
        public float heatReductionPerSecond = 1f;

        [Group("Heat")]
        public float heatReductionPerSecondKilled = 2f;

        [Group("Heat")]
        public float heatReductionPerSecondPlayer = 4f;

        [Group("Heat")]
        [Dropdown(nameof(GetAllAnimationID))]
        public string heatAnimationId = "HumanFireDeaths";
        [NonSerialized]
        public AnimationData heatAnimationData;

        [Group("Heat")]
        public float maxHeat = 100;

        [Group("Animation")]
        public float animationStartTimeMax = 0.5f;

        [Group("Charring")]
        public float fullCharTime = 10f;

        [Group("Charring")]
        public float charRevealStep = 0.02f;

        [Group("Charring")]
        public AnimationCurve charResistanceCurve = AnimationCurve.Linear(0, 1, 1, 0.2f);

        [Group("Charring")]
        public bool allowVisualCharring = true;

        [Group("Charring")]
        public bool freezeOnFullChar = true;

        [Group("Charring")]
        public bool weakenJointsOnFullChar = true;

        [Group("Charring")]
        public float fullCharCharJointMultiplier = 1;

        [Group("Spread")]
        [Dropdown(nameof(GetAllEffectID))]
        public string igniteEffectId = "SpellFireBurningSpread";

        [Group("Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string smokingMainEffectId;

        [NonSerialized]
        public EffectData smokingMainEffectData;

        [Group("Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string smokingLimbEffectId;

        [NonSerialized]
        public EffectData smokingLimbEffectData;

        [Group("Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string burningMainEffectId;

        [NonSerialized]
        public EffectData burningMainEffectData;

        [Group("Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string burningLimbEffectId;

        [NonSerialized]
        public EffectData burningLimbEffectData;

        [NonSerialized]
        public EffectData igniteEffectData;
    }
}
