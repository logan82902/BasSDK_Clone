using System;
using System.Collections;
using System.Collections.Generic;

using TriInspector;
using ThunderRoad.Modules;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ThunderRoad
{
    public class LoreSpawner : MonoBehaviour
    {
        #region Public Fields 
        public LorePackCondition.Visibility visibility;
        public List<string> loreConditionRequiredParameters;
        public List<string> loreConditionOptionalParameters;
        public bool isGroupLoreSpawn = false;
        [ShowIf("isGroupLoreSpawn")]
        public List<Transform> spawnPoints = new();
        public List<ILoreDisplay> loreDisplayers = new();
        public List<Item> loreItems = new();
        public bool IsPopulated => isPopulated;
        public bool ForceEnable = false;
        public bool SpawnOnStart = true;
        public bool ManualCulling = false;
        [ShowIf("ManualCulling")]
        public float CullingDistance = 50f;
        public bool useSpecificLorePack;
        [ShowIf("useSpecificLorePack")]
        [Tooltip("The lore pack name starts with the local path of the pack folder name")]
        public string specificLorePackName;

        public ItemSpawner parentSpawner;


        [ShowIf("DEBUG")]
        public LoreArea loreArea;
        #endregion

        #region Private 
        private LoreModule loreModule = null;
        private int lorePackId = -1;
        private bool hasRead = false;
        private bool isPopulated;
        private List<LoreScriptableObject.LoreData> loreData;
        private LoreScriptableObject.LorePack lorePack;
        private EffectData loreFoundEffectData;
        private string LoreFoundSfxID = "LoreFound";
        private bool initialized = false;
        #endregion


        public void Init()
        {
        }


        public void PopulateLore()
        {
        }


        public void MarkAsRead(bool playEffect)
        {
        }

    }
}
