using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(5)]
#endif
    [DeclareBoxGroup("Patrol", Title = "Patrol Enemies")]
    [DeclareHorizontalGroup("Patrol/Horiz")]
    [DeclareBoxGroup("Alert", Title = "Alert Enemies")]
    [DeclareHorizontalGroup("Alert/Horiz")]
    [DeclareBoxGroup("Rare", Title = "Rare Enemies")]
    [DeclareHorizontalGroup("Rare/Horiz")]
    [DeclareBoxGroup("Std", Title = "Standard Waves")]
    [DeclareHorizontalGroup("Std/Horiz")]
    [DeclareBoxGroup("Arena", Title = "Arena Waves")]
    [DeclareHorizontalGroup("Arena/Horiz")]
    [DeclareBoxGroup("End", Title = "End Waves")]
    [DeclareHorizontalGroup("End/Horiz")]
    public class EnemyConfig : CatalogData
    {
        [Serializable]
        [DeclareHorizontalGroup("Horiz")]
        public class DescriptionByLevelId
        {
            [GroupNext("Horiz")]
            [LabelWidth(60f)]
            public string levelId;
            [LabelWidth(80f)]
            public string description;
        }

        public const string GroupId = "EnemyConfig";

        [Header("UI")]
        public string iconAddress;
        public bool customIconColor = false;

        [ShowIf("customIconColor")]
        [Tooltip("Icon color")]
        public Color iconColor = Color.white;
        [Tooltip("Banner color")]
        public Color color;
        public string name;
        public string nameLocalizationId;

        [ListDrawerSettings(AlwaysExpanded = true)]
        [HideReferencePicker]
        public List<DescriptionByLevelId> descriptionsByLevelId;

        [GroupNext("Patrol/Horiz")]
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllCreatureTableID))]
        public string patrolMixId;
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllCreatureTableID))]
        public string patrolMeleeId;
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllCreatureTableID))]
        public string patrolRangedId;

        [GroupNext("Alert/Horiz")]
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllCreatureTableID))]
        public string alertMixId;
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllCreatureTableID))]
        public string alertMeleeId;
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllCreatureTableID))]
        public string alertRangedId;

        [GroupNext("Rare/Horiz")]
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllCreatureTableID))]
        public string rareMixId;
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllCreatureTableID))]
        public string rareMeleeId;
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllCreatureTableID))]
        public string rareRangedId;

        [GroupNext("Std/Horiz")]
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllWavesID))]
        public string mixedStd;
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllWavesID))]
        public string meleeOnlyStd;
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllWavesID))]
        public string meleeFocusedStd;
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllWavesID))]
        public string rangedFocusedStd;

        [GroupNext("Arena/Horiz")]
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllWavesID))]
        public string mixedArena;
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllWavesID))]
        public string meleeOnlyArena;
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllWavesID))]
        public string meleeFocusedArena;
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllWavesID))]
        public string rangedFocusedArena;

        [GroupNext("End/Horiz")]
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllWavesID))]
        public string mixedEnd;
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllWavesID))]
        public string meleeOnlyEnd;
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllWavesID))]
        public string meleeFocusedEnd;
        [LabelWidth(140f)]
        [Dropdown(nameof(GetAllWavesID))]
        public string rangedFocusedEnd;


        public TriDropdownList<string> GetAllCreatureTableID()
        {
            TriDropdownList<string> dropdownList = new() { new TriDropdownItem<string>() { Text = "None", Value = "" } };
            dropdownList.needsValidation = Catalog.IsJsonLoaded();
            List<string> creatureTableIds = Catalog.GetDataList(Category.CreatureTable).OfType<CreatureTable>().Select(x => x.id).ToList();
            foreach (string id in creatureTableIds)
            {
                dropdownList.Add(id, id);
            }
            return dropdownList;
        }

        public TriDropdownList<string> GetAllWavesID()
        {
            TriDropdownList<string> dropdownList = new() { new TriDropdownItem<string>() { Text = "None", Value = "" } };
            List<string> waveIds = Catalog.GetDataList(Category.Wave).OfType<WaveData>().Select(x => x.id).ToList();
            foreach (string id in waveIds)
            {
                dropdownList.Add(id, id);
            }
            return dropdownList;
        }
    }
}