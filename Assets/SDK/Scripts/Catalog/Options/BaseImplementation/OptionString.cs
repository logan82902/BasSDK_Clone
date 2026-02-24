using System;
using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    public abstract class OptionStringBase : OptionBase<OptionIntValue>
    {
        protected int defaultIntValue;
        protected int currentIntValue;

        public abstract int StringCount();

        public abstract string GetString(int index);
        
        public virtual OptionStringValue DefaultStringValue()
        {
            string defaultValue = GetString(defaultIntValue);
            return new OptionStringValue(defaultValue);
        }

        public virtual OptionStringValue CurrentStringValue()
        {
            string currentValue = GetString(currentIntValue);
            return new OptionStringValue(currentValue);
        }
        
        public override string GetCurrentValueLabel()
        {
	        return GetString(currentIntValue);
        }
        
        public override string GetDefaultValueLabel()
        {
	        return GetString(defaultIntValue);
        }

        public override OptionValue DefaultValue()
        {
            return new OptionIntValue(defaultIntValue);
        }

        public override OptionValue CurrentValue()
        {
            return new OptionIntValue(currentIntValue);
        }

        public override void SetValue(OptionValue preset)
        {
            if (preset is OptionIntValue intPreset)
            {
                currentIntValue = intPreset.value;
                //clamp to min/max
                currentIntValue = Math.Clamp(currentIntValue, 0, StringCount());
            }
        }
    }

    [Serializable]
    [DeclareHorizontalGroup("Horiz")]
    public abstract class OptionStringList : OptionStringBase
    {
        public override bool IsHidden() => false; // should be visible by default
        public override bool IsLevelOption() => true; // should be set as level option by default

        public abstract List<string> GetOptionList();
        public abstract void SetOptionList(List<string> list);

        public override string GetString(int index)
        {
            if (index >= 0 && index < StringCount())
            {
                return GetOptionList()[index];
            }
            return $"INVALID[{index}]";
        }

        public override int StringCount()
        {
            if (GetOptionList().IsNullOrEmpty()) return 0;
            return GetOptionList().Count;
        }

        public override void SetValue(OptionValue preset)
        {
            if (GetOptionList().IsNullOrEmpty()) SetOptionList(new List<string>());
            base.SetValue(preset);
        }
    }

    [Serializable]
    public class OptionString : OptionStringList
    {
        [Group("Horiz")]
        public List<string> valueList = new();

        public override List<string> GetOptionList() => valueList;
        public override void SetOptionList(List<string> list) => valueList = list;
    }

    [Serializable]
    public class OptionStringLabel : OptionString
    {
        [Group("Horiz")]
        public List<string> labelList = new();

        public override string GetCurrentValueLabel()
        {
            if (currentIntValue >= 0 && currentIntValue < StringCount())
            {
                return labelList[currentIntValue];
            }
            return $"INVALID[{currentIntValue}]";
        }

        public override string GetDefaultValueLabel()
        {
            if (defaultIntValue >= 0 && defaultIntValue < StringCount())
            {
                return labelList[defaultIntValue];
            }
            return $"INVALID[{defaultIntValue}]";
        }
    }

    public abstract class OptionStringDropdown : OptionStringList
    {
        [Group("Horiz")]
        [DropdownList(nameof(GetValueOptions))]
        public List<string> valueList = new();

        public override List<string> GetOptionList() => valueList;
        public override void SetOptionList(List<string> list) => valueList = list;

        public abstract TriDropdownList<string> GetValueOptions();
    }

    [Serializable]
    public class OptionStringLabelDropdown : OptionStringDropdown
    {
        [Group("Horiz")]
        public List<string> labelList = new();

        public override string GetCurrentValueLabel()
        {
            if (currentIntValue >= 0 && currentIntValue < StringCount())
            {
                return labelList[currentIntValue];
            }
            return $"INVALID[{currentIntValue}]";
        }

        public override string GetDefaultValueLabel()
        {
            if (defaultIntValue >= 0 && defaultIntValue < StringCount())
            {
                return labelList[defaultIntValue];
            }
            return $"INVALID[{defaultIntValue}]";
        }

        public override TriDropdownList<string> GetValueOptions() => Catalog.GetDropdownAllID(Category.AreaCollection);
    }
}
