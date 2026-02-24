using System;
using System.Collections.Generic;
using ThunderRoad.Skill.Spell;
using UnityEngine;

using TriInspector;

namespace ThunderRoad.Skill.SpellPower
{
    [Serializable]
    [DeclareBoxGroup("SlowTime", Title = "SlowTime")]
    [DeclareBoxGroup("Focus", Title = "Focus")]
    [DeclareBoxGroup("HyperFocus", Title = "HyperFocus")]
    public class SpellPowerSlowTime : SpellPowerData
    {
        [Group("SlowTime")]
        [Dropdown(nameof(GetAllEffectID))]
        public string effectId;
        [NonSerialized]
        public EffectData effectData;
        [Group("SlowTime")]
        public bool scaleToMixerParameter;
        [Group("SlowTime")]
        public string scaleToMixerParameterName;
        [Group("SlowTime")]
        public AnimationCurve enterCurve;
        [Group("SlowTime")]
        public AnimationCurve exitCurve;
        [Group("Focus")]
        public float scale = 0.5f;
        [Group("Focus")]
        public float focusConsumptionStart = 5f;
        [Group("Focus")]
        public float focusConsumption = 2.5f;
        [Group("Focus")]
        public float focusMinToUse = 10;
        [Group("HyperFocus")]
        public string hyperFocusSkillID = "HyperFocus";
        [Group("HyperFocus")]
        protected SkillHyperFocus hyperFocusSkill;

        [Group("Focus")]
        [Dropdown(nameof(GetAllEffectID))]
        public string focusDepletedEffectId;
        [NonSerialized]
        public EffectData focusDepletedEffectData;

        public delegate void TimeScaleChangeEvent(SpellPowerSlowTime spell, float scale);

        public static event TimeScaleChangeEvent OnTimeScaleChangeEvent;
        
        public delegate void SlowTimeEvent(SpellPowerSlowTime slowTime, bool hyperFocus);
        public event SlowTimeEvent OnFocusRunOutEvent;

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            effectData = Catalog.GetData<EffectData>(effectId);
            focusDepletedEffectData = Catalog.GetData<EffectData>(focusDepletedEffectId);
        }

    }

}
