using UnityEngine;
using System;
using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(5)]
#endif
    public class LiquidData : CatalogData
    {
        [Multiline]
        public string designation;
        public Color color;

        [Serializable]
        public class Content
        {
            [Dropdown(nameof(GetAllLiquidID))]
            public string liquidId;
            [NonSerialized]
            public LiquidData liquidData;
            public float level;

            public Content Clone()
            {
                Content clonedContent = MemberwiseClone() as Content;
                if (liquidData != null) clonedContent.liquidData = liquidData.Clone() as LiquidData;
                return clonedContent;
            }

            public Content()
            {
                this.liquidId = "";
                this.liquidData = null;
                this.level = 0;
            }

            public Content(LiquidData liquidData, float level)
            {
                this.liquidId = liquidData.id;
                this.liquidData = liquidData.Clone() as LiquidData;
                this.level = level;
            }

            public TriDropdownList<string> GetAllLiquidID() => Catalog.GetDropdownAllID(Category.Liquid);
        }

        [Serializable]
        [DeclareBoxGroup("Settings", Title = "Indicator Settings")]
        [AutoLabelWidth(MinWidth = 0f)]
        public abstract class Indicator
        {
            public abstract string GetName();

        }

        [Serializable]
        public class NoIndicator : Indicator
        {
            public override string GetName() => "None";

        }

        [Serializable]
        public class LevelIndicator : Indicator
        {
            public override string GetName() => "Level";

        }

    }
}