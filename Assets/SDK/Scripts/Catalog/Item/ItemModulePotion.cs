using System.Collections.Generic;
using UnityEngine;
using TriInspector;
using Newtonsoft.Json;
using System;

namespace ThunderRoad
{
    [Serializable]
    [DeclareBoxGroup("Cork", HideTitle = true)]
    [DeclareHorizontalGroup("Cork/Horiz")]
    [DeclareBoxGroup("Flow")]
    [DeclareHorizontalGroup("Flow/Angles")]
    [DeclareHorizontalGroup("Horiz")]
    [DeclareBoxGroup("Horiz/Primary", Title = "Primary Indicator")]
    [DeclareBoxGroup("Horiz/Secondary", Title = "Secondary Indicator")]
    public class ItemModulePotion : ItemModule
    {
        [PropertyOrder(0)]
        public float maxLevel = 50;
        [PropertyOrder(1)]
        [ListDrawerSettings(AlwaysExpanded = true)]
        public List<LiquidData.Content> contents;
        [PropertyOrder(2)]
        [Group("Cork/Horiz")]
        public bool popCorkOnAltUse;
        [Group("Cork/Horiz")]
        public float corkPopForce = 2f;
        [Group("Cork/Horiz")]
        public ForceMode corkPopForceMode = ForceMode.Impulse;

        [PropertyOrder(3)]
        [Group("Flow")]
        public LayerMask collisionLayer;
        [Group("Flow")]
        public float flowSpeed = 1;
        [Group("Flow/Angles")]
        public float flowMinAngle = 70;
        [Group("Flow/Angles")]
        public float flowMaxAngle = 100;

        [Group("Flow")]
        [Dropdown(nameof(GetAllEffectID))]
        public string effectFlowId = "PotionFlow";

        [PropertyOrder(4)]
        [Group("Horiz/Primary")]
        [Dropdown(nameof(GetAllEffectID))]
        public string primaryEffectId;
        [JsonProperty]
        public string levelEffectId
        {
            set
            {
                primaryEffectId = value;
            }
        }
        [PropertyOrder(5)]
        [Group("Horiz/Primary")]
        [GradientUsage(true)]
        public Gradient primaryIndicatorGradient;
        [JsonProperty]
        public Gradient levelIndicatorGradient
        {
            set
            {
                primaryIndicatorGradient = value;
            }
        }
#if UNITY_EDITOR
        [JsonIgnore]
        [ShowInInspector]
        [PropertyOrder(6)]
        [Group("Horiz/Primary")]
        [Dropdown(nameof(GetAllIndicatorTypes))]
        public Type primaryIndicatorType
        {
            get => primaryIndicator?.GetType() ?? typeof(LiquidData.NoIndicator);
            set => primaryIndicator = (LiquidData.Indicator)Activator.CreateInstance(value);
        }
#endif
        [PropertyOrder(7)]
        [Group("Horiz/Primary")]
        [HideReferencePicker]
        [InlineProperty]
        [LabelWidth(0.0001f)]
        public LiquidData.Indicator primaryIndicator = new LiquidData.LevelIndicator();

        [PropertyOrder(8)]
        [Group("Horiz/Secondary")]
        [Dropdown(nameof(GetAllEffectID))]
        public string secondaryEffectId;
        [JsonProperty]
        public string healthEffectId
        {
            set
            {
                secondaryEffectId = value;
            }
        }
        [PropertyOrder(9)]
        [Group("Horiz/Secondary")]
        [GradientUsage(true)]
        public Gradient secondaryIndicatorGradient;
        [JsonProperty]
        public Gradient healthIndicatorGradient
        {
            set
            {
                secondaryIndicatorGradient = value;
            }
        }
#if UNITY_EDITOR
        [JsonIgnore]
        [ShowInInspector]
        [PropertyOrder(10)]
        [Group("Horiz/Secondary")]
        [Dropdown(nameof(GetAllIndicatorTypes))]
        public Type secondaryIndicatorType
        {
            get => secondaryIndicator?.GetType() ?? typeof(LiquidData.NoIndicator);
            set => secondaryIndicator = (LiquidData.Indicator)Activator.CreateInstance(value);
        }
#endif
        [PropertyOrder(11)]
        [Group("Horiz/Secondary")]
        [HideReferencePicker]
        [InlineProperty]
        [LabelWidth(0.0001f)]
        public LiquidData.Indicator secondaryIndicator = new LiquidHealth.PlayerHealthIndicator();

        private EffectData primaryEffectData;
        private EffectData secondaryEffectData;

        public TriDropdownList<string> GetAllItemID() => Catalog.GetDropdownAllID(Category.Item);

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

        public TriDropdownList<Type> GetAllIndicatorTypes()
        {
            TriDropdownList<Type> result = new TriDropdownList<Type>();
            LiquidData.NoIndicator noIndicator = new LiquidData.NoIndicator();
            result.Add(noIndicator.GetName(), noIndicator.GetType());
            LiquidData.LevelIndicator levelIndicator = new LiquidData.LevelIndicator();
            result.Add(levelIndicator.GetName(), levelIndicator.GetType());
            System.Reflection.Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in loadedAssemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsSubclassOf(typeof(LiquidData.Indicator))) continue;
                    if (type == typeof(LiquidData.NoIndicator)) continue;
                    if (type == typeof(LiquidData.LevelIndicator)) continue;
                    LiquidData.Indicator indicator = (LiquidData.Indicator)Activator.CreateInstance(type);
                    result.Add(indicator.GetName(), indicator.GetType());
                }
            }
            return result;
        }

    }
}
