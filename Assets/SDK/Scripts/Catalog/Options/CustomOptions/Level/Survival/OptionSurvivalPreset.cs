using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TriInspector;

namespace ThunderRoad
{
    public class OptionSurvivalPreset : OptionBase<OptionIntValue>
    {
        [Group("Foldout")]
        public int defaultValue;
        [Group("Foldout")]
        public int currentValue;

        [Group("Foldout")]
        public List<SurvivalPreset> presets;

        public override OptionValue DefaultValue() => new OptionIntValue(defaultValue);
        public int Count => presets?.Count ?? 0;

        public bool TryGetCurrent(out SurvivalPreset preset) => TryGetPreset(currentValue, out preset);

        public bool TryGetPreset(int index, out SurvivalPreset value)
        {
            value = default;
            if (presets is not { Count: > 0 }) return false;
            value = presets[ClampValue(index)];
            return true;
        }

        public int ClampValue(int value) => Math.Clamp(value, 0, presets.Count - 1);

        public override OptionValue CurrentValue() => new OptionIntValue(currentValue);

        public override void SetValue(OptionValue preset)
        {
            if (preset is OptionIntValue intValue) currentValue = ClampValue(intValue.value);

            SetDescription();
        }

        public override void SetDescription()
        {
            if (TryGetCurrent(out var option))
            {
                description = option.LocalizedDesc();
            }
        }

        public override string GetCurrentValueLabel()
        {
            return TryGetPreset(ClampValue(currentValue), out var preset) ? preset.LocalizedName() : "";
        }

        public override string GetDefaultValueLabel()
        {
            return TryGetPreset(ClampValue(defaultValue), out var preset) ? preset.LocalizedName() : "";
        }

	}
    
	[Serializable]
	public class OptionSurvivalPresetValue : OptionValue
	{
		public OptionSurvivalPresetValue()
		{ }
		
		public OptionSurvivalPresetValue(SurvivalPreset value) : base()
		{
			this.value = value;
		}
		public SurvivalPreset value;
		public override object Value() => value;
	}
}