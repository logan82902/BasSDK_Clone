using System.Collections.Generic;
using TMPro;
using UnityEngine;

using TriInspector;

namespace ThunderRoad
{
    public class UISelectionListButtonsGameModeDifficultyPreset : UISelectionListButtons
    {
        public GameObject difficultyParent;
        public TextMeshProUGUI presetDescriptionText;
        public UISelectionListButtonsLevelModeOption LevelOptionPrefab = null;
        public Transform optionsPanel = null;
        public UIText optionDescriptionText;

        [Header("Custom text")]
        [Dropdown(nameof(GetAllTextGroupID))]
        public string presetCustomLocalizedGroupId;

        [Dropdown(nameof(GetAllTextId))]
        public string presetCustomLocalizedName;

        [Dropdown(nameof(GetAllTextId))]
        public string presetCustomLocalizedDescription;

        public TriDropdownList<string> GetAllTextGroupID() => Catalog.GetTextData().GetDropdownAllTextGroups();

        public TriDropdownList<string> GetAllTextId() => Catalog.GetTextData().GetDropdownAllTexts(presetCustomLocalizedGroupId);

    }
}