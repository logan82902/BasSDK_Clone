using UnityEngine;
using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Levels/ContainerSpawner.html")]
    [DeclareBoxGroup("Properties", Title = "Properties")]
    [DeclareBoxGroup("Utilities", Title = "Utilities")]
    public class ContainerSpawner : MonoBehaviour
    {
        #region VARIABLES

        [Group("Properties")]
        [Dropdown(nameof(GetAllContainerId))]
        public string containerId;

        [Group("Properties")]
        public bool pooled;

        [Group("Properties")]
        public bool spawnAll = false;

        [Group("Properties")]
        public bool allowDuplicates = true;

        [Group("Properties")]
        public bool spawnOnStart = true;

        [Group("Properties")]
        public bool disallowDespawn;

        [Group("Properties")]
        public bool holderIsPivot = false;

        [PropertySpace(5, 20)]
        public List<Transform> spawnPoints = new();

        public Item.Owner spawnOwner = Item.Owner.None;

        #endregion


        /// <summary>
        /// If the spawner isn't registering the files this will quickly allow the artist to do a one-click fix.
        /// </summary>
        [Group("Utilities")]
        [Button("Reload JSON")]
        private void ReloadJson() => Catalog.EditorLoadAllJson(true);

        private TriDropdownList<string> GetAllContainerId()
        {
            TriDropdownList<string> items = new();
            items.AddRange(Catalog.GetDropdownAllID<ContainerData>());
            items.AddRange(Catalog.GetDropdownAllID<LootTableBase>());

            return items;
        }
    }
}
