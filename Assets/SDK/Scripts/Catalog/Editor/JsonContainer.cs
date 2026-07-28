using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TriInspector;
using UnityEditor;
using UnityEngine;

namespace ThunderRoad
{
    [System.Serializable]
    public abstract class JsonContainer : ScriptableObject
    {
    }

    [HideMonoScript]
    [System.Serializable]
    [DeclareBoxGroup("Box", HideTitle = true)]
    [DeclareVerticalGroup("Box/Vertical")]
    [DeclareBoxGroup("Box/Vertical/Data", Title = "Data")]
    [DeclareHorizontalGroup("Box/Vertical/Data/TopLine", Sizes = new float[] { 0f, 250f, 100f, 250f })]
    [DeclareHorizontalGroup("Box/Vertical/Data/Midline")]
    [DeclareHorizontalGroup("Box/Vertical/Data/BottomLine", Sizes = new float[] { 0f, 200f })]
    [DeclareBoxGroup("Box/Vertical/Modding", Title = "Modding")]
    [DeclareHorizontalGroup("Box/Vertical/Modding/TopLine", Sizes = new float[] { 0f, 80f })]
    [DeclareVerticalGroup("Box/Vertical/Object")]
    [CreateAssetMenu(menuName = "ThunderRoad/JsonContainer")]
    public class CatalogDataContainer : JsonContainer
    {
        [Group("Box/Vertical/Data/TopLine")]
        [ShowInInspector, LabelWidth(15f)]
        [PropertyOrder(0)]
        [DisableIf(nameof(overrideLock))]
        public string id
        {
            get => catalogData.id;
            set
            {
                if (!canChangePath) catalogData.filePath = catalogData.filePath.Replace($"_{catalogData.id}.json", $"_{value}.json");
                catalogData.id = value;
            }
        }

        [Group("Box/Vertical/Data/Midline")]
        [ShowInInspector, LabelWidth(105)]
        [PropertyOrder(1)]
        [DisableIf(nameof(lockData))]
        public BuildSettings.ContentFlag sensitiveContent
        {
            get => catalogData.sensitiveContent;
            set => catalogData.sensitiveContent = value;
        }

        [Group("Box/Vertical/Data/Midline")]
        [ShowInInspector, LabelWidth(150)]
        [PropertyOrder(2)]
        [DisableIf(nameof(lockData))]
        public BuildSettings.ContentFlagBehaviour sensitiveFilterBehaviour
        {
            get => catalogData.sensitiveFilterBehaviour;
            set => catalogData.sensitiveFilterBehaviour = value;
        }

        [Group("Box/Vertical/Data/TopLine")]
        [ShowInInspector, LabelWidth(45)]
        [ReadOnly]
        [PropertyOrder(4)]
        public int hashId => catalogData.hashId;

        [Group("Box/Vertical/Data/TopLine")]
        [ShowInInspector, LabelWidth(50)]
        [ReadOnly]
        [PropertyOrder(5)]
        public int version => catalogData.version;

        [Group("Box/Vertical/Data/TopLine")]
        [ShowInInspector, LabelWidth(70)]
        [PropertyOrder(6)]
        [DisableIf(nameof(overrideLock))]
        public string groupPath
        {
            get => catalogData.groupPath;
            set => catalogData.groupPath = value;
        }
        private bool overrideLock => lockData || catalogData.isOverrider;

        [Group("Box/Vertical/Data/BottomLine")]
        [ShowInInspector, LabelWidth(60)]
        [ReadOnly]
        [PropertyOrder(7)]
        public string filePath => catalogData.filePath;

        [Group("Box/Vertical/Modding/TopLine")]
        [ShowInInspector, LabelWidth(50)]
        [HideReferencePicker]
        [InlineProperty]
        [ReadOnly]
        [PropertyOrder(14)]
        public string source => catalogData.GetOwnerString();


        [Group("Box/Vertical/Modding/TopLine")]
        [ShowInInspector, LabelWidth(65)]
        [HideReferencePicker]
        [InlineProperty]
        [ReadOnly]
        [PropertyOrder(15)]
        public bool isOverride => catalogData.isOverrider;

        [Group("Box/Vertical/Modding")]
        [ShowInInspector]
        [ShowIf(nameof(hasOverriders))]
        [ReadOnly]
        [PropertyOrder(10)]
        [AutoLabelWidth(AddedWidth = 5f)]
        public string changers
        {
            get
            {
                if (catalogData.overriderDatas.IsNullOrEmpty()) return string.Empty;
                string result = "";
                foreach (var c in catalogData.overriderDatas) result += c.GetOwnerString() + " (" + c.editorGuid.Substring(0, 4) + "), ";
                return result;
            }
        }

        private bool hasOverriders => !catalogData.overriderDatas.IsNullOrEmpty();

        [Group("Box/Vertical/Object")]
        [SerializeReference]
        [ShowInInspector]
        [HideReferencePicker]
        [InlineProperty]
        [HideLabel]
        [PropertyOrder(20)]
        [DisableIf(nameof(lockData))]
        public CatalogData catalogData;

        private bool lockData
        {
            get
            {
                if (Application.isPlaying) return false;
                return catalogData.owner == null;
            }
        }

        [SerializeField]
        [HideInInspector]
        private bool dataSet = false;
        [SerializeField]
        [HideInInspector]
        private bool managed = true;

        private bool canSet => !managed && !dataSet;
        private bool canClear => !managed && dataSet;

        [Button]
        [ShowIf(nameof(canSet))]
        public void SetData(string id)
        {
            managed = false;
            if (!Catalog.IsJsonLoaded()) Catalog.EditorLoadAllJson(true, true, true);
            foreach (var category in Catalog.baseTypeCategories)
            {
                if (Catalog.GetData(category.Category, id) is CatalogData cd)
                {
                    SetData(cd);
                    return;
                }
            }
        }

        public void SetData(CatalogData data)
        {
            dataSet = true;
            catalogData = data;
        }

        [Button]
        [ShowIf(nameof(canClear))]
        public void ClearData()
        {
            dataSet = false;
            catalogData = null;
        }

        private bool canChangePath => !catalogData.realFilePath.IsNullOrEmptyOrWhitespace() && !lockData;
        private string changePathText => canChangePath ? "Change JSON Path" : "Save JSON to change path";

        [Button("$" + nameof(changePathText))]
        [Group("Box/Vertical/Data/BottomLine")]
        [PropertyOrder(8)]
        [EnableIf(nameof(canChangePath))]
        public void ChangeJSONPath()
        {
            string currentFileName = Path.GetFileNameWithoutExtension(catalogData.realFilePath);
            string currentFolderPath = catalogData.realFilePath.Replace($"{currentFileName}.json", "");
            string newPath = EditorUtility.SaveFilePanel("Move to...", currentFolderPath, currentFileName, "json");
            if (!catalogData.realFilePath.IsNullOrEmptyOrWhitespace() && File.Exists(catalogData.realFilePath))
            {
                File.Move(catalogData.realFilePath, newPath);
            }
            catalogData.realFilePath = newPath;
            catalogData.filePath = newPath;
        }
    }

    [HideMonoScript]
    [System.Serializable]
    [DeclareBoxGroup("Box", HideTitle = true)]
    [DeclareBoxGroup("Box/Path", Title = "Path")]
    [DeclareHorizontalGroup("Box/Path/Horiz", Sizes = new float[] { 0f, 200f })]
    [DeclareBoxGroup("Box/Data", Title = "Data")]
    public class GameDataContainer : JsonContainer
    {
        [ShowInInspector]
        [Group("Box/Path/Horiz")]
        [PropertyOrder(0)]
        [ReadOnly]
        public string filePath => gameData?.filePath;

        [SerializeReference]
        [ShowInInspector]
        [PropertyOrder(2)]
        [Group("Box/Data")]
        [HideLabel]
        [DisableIf(nameof(lockData))]
        [InlineProperty]
        [HideReferencePicker]
        public GameData gameData;

        private bool isBaseData => gameData == Catalog.gameData;

        private bool lockData
        {
            get
            {
                if (Application.isPlaying) return false;
                return isBaseData;
            }
        }

        [Button("Change JSON Path")]
        [Group("Box/Path/Horiz")]
        [PropertyOrder(1)]
        [DisableIf(nameof(isBaseData))]
        public void ChangeJSONPath()
        {
            string currentFileName = Path.GetFileNameWithoutExtension(gameData.filePath);
            string currentFolderPath = gameData.filePath.Replace($"{currentFileName}.json", "");
            string newPath = EditorUtility.SaveFilePanel("Move to...", currentFolderPath, currentFileName, "json");
            if (!gameData.filePath.IsNullOrEmptyOrWhitespace() && File.Exists(gameData.filePath))
            {
                File.Move(gameData.filePath, newPath);
            }
            gameData.filePath = newPath;
        }
    }
}
