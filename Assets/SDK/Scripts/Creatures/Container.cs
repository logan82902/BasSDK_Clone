using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;

using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/Creatures/Container.html")]
    [AddComponentMenu("ThunderRoad/Container")]
    public class Container : MonoBehaviour
    {
        public LoadContent loadContent = LoadContent.None;
        public string loadPlayerContainerID;

        [Dropdown(nameof(GetAllContainerID))]
        public string containerID;
        public bool loadOnStart = true;

        public Item.Owner spawnOwner = Item.Owner.None;
        public List<Holder> linkedHolders = new List<Holder>();

        public enum LoadContent
        {
            None,
            ContainerID,
            PlayerInventory,
        }

        public TriDropdownList<string> GetAllContainerID() => Catalog.GetDropdownAllID(Category.Container);

        public void Load()
        {
        }

        public void ClearLinkedHolders()
        {
        }

        public void LoadFromContainerId()
        {
        }

        public void LoadFromPlayerInventory()
        {
        }

    }
}

