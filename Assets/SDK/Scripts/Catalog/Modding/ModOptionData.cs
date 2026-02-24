using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

using TriInspector;

namespace ThunderRoad
{
    [SkipInTypeDropdown]
#if UNITY_EDITOR
    [EditorLoadPriority(0)]
#endif
    [DeclareBoxGroup("Display", Title = "Display")]
    [DeclareBoxGroup("Category and sorting", Title = "Category and sorting")]
    public abstract class ModOptionData<T> : CustomData
    {
        [Serializable]
        [DeclareHorizontalGroup("Horiz")]
        public class Option
        {
            [Group("Horiz")]
            [LabelWidth(40f)]
            public string text;
            [Group("Horiz")]
            [LabelWidth(40f)]
            public T value;

        }

        public enum InteractionType
        {
            ArrowList, //  left/right arrows with the value in the middle (<- Value ->)
            ButtonList, //button which cycles through the options every time its pressed (Value)
            Slider, // horizontal slider with left/right arrows
        }

        [Group("Display")]
        public InteractionType interactionType = InteractionType.ArrowList;
        [Group("Display")]
        public string optionName;
        [Group("Display")]
        public string optionLocalizationID;
        [Group("Display")]
        public string tooltip = "";
        [Group("Display")]
        public int defaultOptionIndex = -1;
        [Group("Display")]
        public bool storeValueInSave = true;
        [Group("Category and sorting")]
        public string category;
        [Group("Category and sorting")]
        public int categoryOrder = int.MaxValue;
        [Group("Category and sorting")]
        public int optionOrder = int.MaxValue;
        [HideReferencePicker]
        public List<Option> options;

    }
}
