using System;
using TriInspector;

namespace ThunderRoad
{
	/// <summary>
	/// This is a reusable class to create options for levels or gamemodes
	/// </summary>
	[Serializable]
	public class Option : OptionBase<OptionIntValue>
    {
        [Group("Foldout")]
        public int minValue;
        [Group("Foldout")]
        public int maxValue;
        [Group("Foldout")]
        public int defaultValue;
        [Group("Foldout")]
        public int stepValue = 1;
		protected int currentValue;
		
		public Option()
		{
			currentValue = defaultValue;
		}

		public override bool IsHidden() => false; // should be visible by default
		public override bool IsLevelOption() => true; // should be set as level option by default
		
		public virtual int MinValue() => minValue;
		public virtual int MaxValue() => maxValue;

		public override OptionValue DefaultValue()
		{
			return new OptionIntValue(defaultValue);
		}

		public override OptionValue CurrentValue()
		{
			return new OptionIntValue(currentValue);
		}

		public override void SetValue(OptionValue preset)
		{
			if(preset is OptionIntValue intPreset)
			{
				currentValue = intPreset.value;
			}
		}
		public override string GetCurrentValueLabel()
		{
			return currentValue.ToString();
		}
		public override string GetDefaultValueLabel()
		{
			return defaultValue.ToString();
		}
	}
}
