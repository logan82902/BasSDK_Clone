using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using ThunderRoad.Modules;
using UnityEngine.ResourceManagement.ResourceLocations;
using Newtonsoft.Json;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(10)]
#endif
    public class GameModeData : CatalogData
    {
        public string name = "Unknown";
        public string nameLocalizationId = "";
        [Multiline]
        public string description = "";
        public string descriptionLocalizationId = "";
        public string warning = "";
        public string warningId = "";
        public string iconAddress;
        public int order = int.MaxValue;
        [NonSerialized]
        public IResourceLocation iconLocation;

        public State state = State.Show;

        [NonSerialized]
        public Texture2D iconLoaded;

        public enum State
        {
            Show,
            Disabled,
            Hidden,
        }


        public override IEnumerator OnCatalogRefreshCoroutine()
        {
            yield return Catalog.LoadLocationCoroutine<Texture2D>(iconAddress, location => iconLocation = location, id);
        }

        public bool refreshMapOnlyWhenPlayerNear = false;
        
        public bool saveGameOnSkillUnlock = false;
        public bool allowLevelSelection = true;
        public bool allowRefundCoreSkills = false;
        
        public bool defaultPlayerInvincibility = false;
        public bool defaultPlayerFallDamage = false;
        public bool defaultPlayerInfiniteFocus = false;
        public bool defaultPlayerFastCast = false;
        public bool defaultInfiniteImbue = false; 
        public bool defaultInfiniteSupply = false;
        public bool defaultClimbFree = false;
        public bool defaultEasyDismemberment = false;
        public bool defaultInfiniteArrows = false;
        public bool defaultArmorDetection = true;

        [Dropdown(nameof(GetAllLevelID))]
        public string levelHome;
        public string levelHomeModeName;
        public string levelHomeTravelSpawnerId;

        public bool hasTutorial;
        public string tutorialPlayerSpawnerId;

        /// <summary>
        /// At character creation the player can select the start inventory from this list
        /// If no choice thhen just make a list of 1 element withh the default values
        /// This can not be null or empty
        /// </summary>
        public List<InventoryStart> playerInventoryStart;

        [ShowInInspector]
        [Dropdown(nameof(GetAllValueTypes))]
        public string mainCurrency = "Gold";
        [ShowInInspector]
        public List<string> otherCurrencies = new();

        [ShowInInspector]
        [ListDrawerSettings(DontForceInline = true)]
        [ReferencePickerScriptButton]
        [ReadOnlyReferencePicker]
        public List<GameModeModule> modules = new();

        [ShowInInspector]
        public List<string> characterStatPrefabAddresses = new();

        [ShowInInspector]
        /// <summary>
        /// Difficulty options that are available for this gamemode to customise the difficulty (they have value from preset)
        /// </summary>
        [ReadOnlyReferencePicker]
        public List<OptionBase> difficultyOptions = new();

        [ShowInInspector]
        //[ListDrawerSettings(ShowFoldout = true, HideAddButton = true, HideRemoveButton = true, ShowPaging = false)]
        [LabelText("Difficulty Presets")]
        /// <summary>
        /// Presets of difficulty options for this gamemode to customise the difficulty
        /// </summary>
        public List<DifficultyPreset> difficultyPresets = new List<DifficultyPreset>();
        public int defaultDifficultyPreset = 0;

        [Header("WarningMessage")]
        [Dropdown(nameof(GetAllTextGroupID))]
        public string warningMessageGroupId;
        [Dropdown(nameof(GetAllTextId))]
        public string warningMessageTextId;


        [DropdownList(nameof(GetAllCheats))]
        [Header("MenuOption")]
        public List<string> cheatsAllowed;

#if UNITY_EDITOR
        [Button]
        public void AddDifficultyPreset()
        {
            DifficultyPreset difficultyPreset = new DifficultyPreset() { gameModeData = this };
            difficultyPreset.Refresh();
            difficultyPresets.Add(difficultyPreset);
        }
#endif

        public TriDropdownList<string> GetAllValueTypes() => Catalog.gameData.GetAllValueTypes();

        public TriDropdownList<string> GetAllContainerID() => Catalog.GetDropdownAllID(Category.Container);

        public TriDropdownList<string> GetAllLevelID() => Catalog.GetDropdownAllID(Category.Level);
        public TriDropdownList<string> GetAllTextGroupID() => Catalog.GetTextData().GetDropdownAllTextGroups();

        public TriDropdownList<string> GetAllTextId() => Catalog.GetTextData().GetDropdownAllTexts(warningMessageGroupId);

        public TriDropdownList<string> GetAllCheats()
        {
            TriDropdownList<string> dropdownList = new();
            dropdownList.Add("Cheats", "Cheats");
            dropdownList.Add("Skills", "Skills");
            dropdownList.Add("Spawn", "Spawn");
            dropdownList.Add("Progression", "Progression");
            return dropdownList;
        }

        /// <summary>
        /// This will return all difficulty options set in the data class, and also allow modders to override the options in code
        /// </summary>
        /// <returns></returns>
        public virtual List<OptionBase> GetDifficultyOptions()
        {
            return difficultyOptions;
        }

        public virtual bool TryGetSavedOption<T>(out T option) where T : OptionBase
        {
            option = null;
            List<OptionBase> options = GetDifficultyOptions();
            int count = options.Count;
            for (int i = 0; i < count; i++)
            {
                if(options[i] is T temp)
                {
                    option = temp;
                    return true;
                }
            }

            return false;
        }

        [JsonIgnore]
        public virtual bool TriggerWarningMessage => false;

        public virtual void OnWarningMessagegDoNotShowAgain() { }

    }
}