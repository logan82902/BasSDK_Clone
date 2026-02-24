using System;
using System.Collections.Generic;

using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    [DeclareHorizontalGroup("Effect")]
    public class EffectBundle
    {
        [Dropdown(nameof(GetAllEffectID))]
        [HideLabel]
        [Group("Effect")]
        public string effectId;
        [NonSerialized]
        public EffectData effectData;

        [DropdownList(nameof(GetAllEffetModuleTypes))]
        [LabelText("Ignored module(s)")]
        [Group("Effect")]
        public List<string> ignoredEffects;
        [NonSerialized]
        public Type[] ignoredEffectTypes;

        public EffectBundle Clone()
        {
            EffectBundle clone = MemberwiseClone() as EffectBundle;
            clone.ignoredEffects = new List<string>(ignoredEffects);
            return clone;
        }

        public void OnCatalogRefresh()
        {
            if (effectId != null && effectId != "") effectData = Catalog.GetData<EffectData>(effectId);
            if (ignoredEffects != null)
            {
                List<Type> types = new List<Type>();
                foreach (string ignoreEffect in ignoredEffects)
                {
                    types.Add(Type.GetType(ignoreEffect));
                }
                ignoredEffectTypes = types.ToArray();
            }
        }

        public TriDropdownList<string> GetAllEffetModuleTypes()
        {
            TriDropdownList<string> dropdownList = new();
            dropdownList.Add("None", null);
            foreach (Type type in typeof(EffectModule).Assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(EffectModule)))
                {
                    dropdownList.Add(new TriDropdownItem<string>() { Text = type.ToString(), Value = type.ToString() });
                }
            }
            return dropdownList;
        }

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);
    }
}
