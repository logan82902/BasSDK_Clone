using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceLocations;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    [DeclareBoxGroup("Holder", Title = "Holder")]
    public class HolderData : InteractableData
    {
        [Group("Holder")]
        [Dropdown(nameof(GetAllItemID))]
        public string spawnItemID;
        [Group("Holder")]
        public int spawnQuantity = 0;
        [Group("Holder")]
        public bool allowInfiniteSupplyCheat = false;
        [Group("Holder")]
        public DespawnProtection itemDespawnCondition;
        [Group("Holder")]
        public int maxQuantity = 1;
        [Group("Holder")]
        public string targetAnchor;
        [Group("Holder")]
        public bool forceAllowTouchOnPlayer;
        [Group("Holder")]
        public bool highlighterPlayerView = false;
        [Group("Holder")]
        public FilterLogic tagFilter = FilterLogic.AnyExcept;
        [Group("Holder")]
        [DropdownList(nameof(GetAllHolderSlots))]
        public List<string> slots;

        [Group("Holder")]
        public FilterLogic typeFilter = FilterLogic.AnyExcept;
        [Group("Holder")]
        public List<ItemData.Type> itemTypes;
        [Group("Holder")]
        public FilterLogic itemFilter = FilterLogic.AnyExcept;
        [Group("Holder")]
        [DropdownList(nameof(GetAllItemID))]
        public List<string> itemIds;

        [Group("Holder")]
        public bool locked = false;
        [Group("Holder")]
        public bool warnIfNotAllowed = false;
        [Group("Holder")]
        public bool warnIfInUse = false;
        [Group("Holder")]
        public bool warnIfLocked = false;
        [Group("Holder")]
        public bool grabFromHandle = true;
        [Group("Holder")]
        public bool playerGrabFromHandle = true;
        [Group("Holder")]
        public bool grabWithTrigger = false;
        [Group("Holder")]
        public bool disableWhenHolstered = false;
        [Group("Holder")]
        public bool allowTeleGrab = true;
        [Group("Holder")]
        public bool hideFromFpv;
        [Group("Holder")]
        public float unsnapColliderEnableDelay = 0f;

        [Group("Holder")]
        public GrabTeleport grabTeleport = GrabTeleport.Disabled;
        [Group("Holder")]
        public bool disableObjectProximityHighlighter = false;
        [Group("Holder")]
        public bool delegateToParentHolder;
        [Group("Holder")]
        public bool ignoreEmptyChildHolders = false;
        [Group("Holder")]
        public float cleanItemsDuration = 0;
        [Group("Holder")]
        public float cleanItemsStep = 10f;

        public TriDropdownList<string> GetAllItemID() => Catalog.GetDropdownAllID(Category.Item);

        public TriDropdownList<string> GetAllHolderSlots() => Catalog.GetDropdownHolderSlots();

        public enum DespawnProtection
        {
            Normal,
            ParentItem,
            ParentCreature,
            Never,
        }

        public enum GrabTeleport
        {
            Disabled,
            Enabled,
            IfParentHolder
        }

        [Group("Holder")]
        public string audioContainerAddress;
        [NonSerialized]
        public IResourceLocation audioContainerLocation;

    }
}
