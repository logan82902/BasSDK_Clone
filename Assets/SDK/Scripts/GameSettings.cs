using System.Collections.Generic;
using UnityEngine;

using TriInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ThunderRoad
{
    [DeclareBoxGroup("Version", Title = "Version")]
    [DeclareHorizontalGroup("Version/Default")]
    [DeclareBoxGroup("Info", Title = "Info")]
    [DeclareBoxGroup("Main Menu", Title = "Main Menu")]
    [DeclareBoxGroup("Game Mode", Title = "Game Mode")]
    [DeclareBoxGroup("UI", Title = "UI")]
    [DeclareBoxGroup("Prefabs", Title = "Prefabs")]
    [DeclareBoxGroup("Telemetry", Title = "Telemetry")]
    public class GameSettings : ScriptableObject
    {

        [Group("Version")]
        public string minModVersion;

        [Group("Info")]
        public string productName;

        [Group("Info")]
        public string appIdentifier;


        [DropdownList(nameof(GetJsonFolders))]
        public List<string> editorLoadDefaultFolders;

        public static List<string> loadDefaultFolders
        {
            get
            {
                return ThunderRoadSettings.current.game.editorLoadDefaultFolders;
            }
            set
            {
                ThunderRoadSettings.current.game.editorLoadDefaultFolders = value;
            }
        }


    #if UNITY_EDITOR
        [MenuItem("ThunderRoad (SDK)/Load All JSON")]
    #endif
    
        [Button("Load all JSON")]
        public static void LoadAllJson()
        {
            Catalog.EditorLoadAllJson(true, force:true);
        }

        public static string GetVersionString(bool stripBuild = false)
        {
            return $"0.0.0.0";
        }

        public TriDropdownList<string> GetJsonFolders()
        {
            TriDropdownList<string> dropdownList = new();
            foreach (string folderName in FileManager.GetFolderNames(FileManager.Type.JSONCatalog, FileManager.Source.Default))
            {
                if (folderName == ".git")
                {
                    continue;
                }
                dropdownList.Add(folderName, folderName);
            }
            return dropdownList;
        }
    }
}