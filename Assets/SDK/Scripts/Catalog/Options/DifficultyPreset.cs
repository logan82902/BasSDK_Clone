using System;
using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad
{
	/// <summary>
	/// This is a collection of options for a gamemode to define its difficulty
	/// </summary>
	[Serializable]
	public class DifficultyPreset
	{
		public string name;
#if UNITY_EDITOR
        [Dropdown(nameof(GetAllTextGroupID))]
#endif
        public string localizationGroupId;

#if UNITY_EDITOR
        [Dropdown(nameof(GetAllTextId))]
#endif
        public string localizationId;

#if UNITY_EDITOR
        [Dropdown(nameof(GetAllTextId))]
#endif
        public string localizationDescriptionId;

		[NonSerialized]
		public GameModeData gameModeData;

        //this needs to hold difficulty options, along with the value they should be set to
        [LabelText("Drop Levels")]
        [LabelWidth(0.0001f)]
        [InlineProperty]
        public OptionPresets optionPresets = new();

		[Button]
		public void Refresh()
		{
			if (gameModeData == null) return;
			foreach (OptionBase option in gameModeData.difficultyOptions)
			{
				//add the option to the list if it doesn't exist
				if (!optionPresets.ContainsKey(option.name))
				{
					//check the type of the option and add the default value
					OptionValue defaultValue = null;
					if (option is Option optionInt)
					{
						defaultValue = optionInt.DefaultValue();
					}
					else if (option is OptionBoolean optionBoolean)
					{
						defaultValue = optionBoolean.DefaultValue();
					}
					else if (option is OptionEnumInt optionEnum)
					{
						defaultValue = optionEnum.DefaultValue();
					}
					optionPresets.Add(option.name, defaultValue);
				}

			}
		}

#if UNITY_EDITOR
        [Button]
        public void RemoveThisLevel() => gameModeData.difficultyPresets.Remove(this);
        public TriDropdownList<string> GetAllTextGroupID() => Catalog.GetTextData().GetDropdownAllTextGroups();

        public TriDropdownList<string> GetAllTextId() => Catalog.GetTextData().GetDropdownAllTexts(localizationGroupId);
#endif
	}
}
