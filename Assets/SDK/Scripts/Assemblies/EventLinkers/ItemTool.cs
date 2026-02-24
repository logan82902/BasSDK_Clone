using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace ThunderRoad
{
    public class ItemTool : ThunderBehaviour
    {
        public static Item currentEventItem;

        public bool clearAfterAction = false;
        public List<SelectionSetting> selectors = new List<SelectionSetting>();
        public UnityEvent<Item> itemEvent;

        [NonSerialized]
        public List<Item> selectedItems = new List<Item>();

        [Serializable]
        [AutoLabelWidth(AddedWidth = 5f)]
        [DeclareHorizontalGroup("TopRow", Sizes = new float[] { 0f, 110f })]
        [DeclareHorizontalGroup("Conditions")]
        [DeclareHorizontalGroup("GrabState")]
        public class SelectionSetting
        {
            [Group("TopRow")]
            public string idFilter = "";
            [Group("TopRow")]
            public int repeat = 1;
            [Group("Conditions")]
            public SelectionCondition condition = SelectionCondition.Random;
            [Group("Conditions")]
            [EnumToggleButtons]
            public VennState breakable = VennState.Either;
            [Header("Grab State")]
            [Group("GrabState")]
            [EnumToggleButtons]
            public VennState playerGrabState = VennState.Either;
            [Group("GrabState")]
            [EnumToggleButtons]
            public VennState NPCGrabState = VennState.Either;
        }

        public enum VennState
        {
            Either,
            Yes,
            No,
        }

        public enum SelectionCondition
        {
            Random,
            NewestSpawn,
            OldestSpawn,
            Closest,
            Furthest,
            NewestInteracted,
            OldestInteracted,
        }


        public void ClearSelections()
        {
        }

        public void PruneRandomly(int leaveBehind)
        {
        }

        public void SelectAllCurrentlyInZone(Zone zone)
        {
        }

        public void RemoveAllCurrentlyInZone(Zone zone)
        {
        }

        public void ZoneSetEventTarget(UnityEngine.Object itemObj, bool add)
        {
        }

        public void ZoneAddToSelection(UnityEngine.Object itemObj) => ZoneAddRemove(itemObj, true);

        public void ZoneRemoveFromSelection(UnityEngine.Object itemObj) => ZoneAddRemove(itemObj, false);

        protected void ZoneAddRemove(UnityEngine.Object itemObj, bool add)
        {
        }

        public void AddUsingSelectorIndex(int index)
        {
        }

        public void RemoveUsingSelectorIndex(int index)
        {
        }


        #region Dynamic invocation item tools
        public void RunEventWithItem(Item item)
        {
        }

        public void BreakEventTargetBreakable(Item item)
        {
        }

        public void DespawnItemInstantly(Item item)
        {
        }

        public void EquipItemToPlayerRightHand(Item item)
        {
        }

        public void EquipItemToPlayerLeftHand(Item item)
        {
        }


        public void SetPlayerOwner(Item item)
        {
        }

        public void SetShopkeeperOwner(Item item)
        {
        }

        public void ClearOwner(Item item)
        {
        }
        #endregion

        #region Selected item tools
        public void CopyComponentToSelection(Component component)
        {
            if (clearAfterAction) ClearSelections();
        }

        public void RemoveComponentFromSelection(Component component)
        {
            if (clearAfterAction) ClearSelections();
        }

        public void RunEventWithSelection()
        {
            if (clearAfterAction) ClearSelections();
        }

        public void ClearSelectionStatuses(string id)
        {
            if (clearAfterAction) ClearSelections();
        }

        public void BreakSelectedBreakables()
        {
            if (clearAfterAction) ClearSelections();
        }

        public void DespawnSelected(float delay)
        {
            if (clearAfterAction) ClearSelections();
        }

        public void EquipOneSelectedToPlayerRightHand()
        {
        }

        public void EquipOneSelectedToPlayerLeftHand()
        {
        }

        public void EquipOneSelectedToHand(RagdollHand hand)
        {
            if (clearAfterAction) ClearSelections();
        }

        public void SwapSelectedWith(string itemID)
        {
            if (clearAfterAction) ClearSelections();
        }

        public void SetSelectedPlayerOwned()
        {
        }

        public void SetSelectedShopkeeperOwned()
        {
        }

        public void ClearSelectedOwner()
        {
        }
        #endregion

        #region Event target item tools
        public void CopyComponentToEventTarget(Component component)
        {
        }

        public void RemoveComponentFromEventTarget(Component component)
        {
        }

        public void RunEventWithEventTarget()
        {
        }

        public void ClearEventTargetStatuses(string id)
        {
        }

        public void BreakEventTargetBreakable()
        {
        }

        public void DespawnEventTarget(float delay)
        {
        }

        public void EquipEventTargetToPlayerRightHand()
        {
        }

        public void EquipEventTargetToPlayerLeftHand()
        {
        }

        public void EquipEventTargetToHand(RagdollHand hand)
        {
        }

        public void SwapEventTargetWith(string itemID)
        {
        }

        public void SetEventTargetPlayerOwned()
        {
        }

        public void SetEventTargetShopkeeperOwned()
        {
        }

        public void ClearEventTargetOwner()
        {
        }
        #endregion
    }
}
