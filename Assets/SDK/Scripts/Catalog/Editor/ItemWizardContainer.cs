// Copyright (c) WarpFrog. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriInspector;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

namespace ThunderRoad
{
    [HideMonoScript]
    [DeclareBoxGroup("Box", HideTitle = true)]
    [DeclareHorizontalGroup("Box/Horiz", Sizes = new float[] { 80f, 0f })]
    [DeclareBoxGroup("Box/Horiz/Assets", Title = "Assets")]
    [DeclareVerticalGroup("Box/Horiz/Verti")]
    [DeclareBoxGroup("Box/Horiz/Verti/Path", Title = "Save Folder")]
    [DeclareBoxGroup("Box/Horiz/Verti/Information", Title = "Item Info")]
    [DeclareHorizontalGroup("Box/Horiz/Verti/Information/Top")]
    [DeclareBoxGroup("Box/Horiz/Verti/Overrides", Title = "Overrides")]
    [DeclareHorizontalGroup("Box/Horiz/Verti/Overrides/Row", Sizes = new float[] { 0f, 0f, 0f, 0f, 110f})]
    [DeclareHorizontalGroup("Box/Horiz/Verti/References")]
    public class ItemWizardContainer : ScriptableObject
    {
        [HideInInspector]
        public ItemData baseData;

        [Title("Item", HorizontalLine = false)]
        [HideLabel]
        [PreviewField(Height = 70f)]
        [OnValueChanged(nameof(ItemPrefabSet))]
        [Group("Box/Horiz/Assets")]
        [PropertyOrder(1)]
        public Item itemPrefab;
        private Item lastSetItem;
        [Title("Icon", HorizontalLine = false)]
        [HideLabel]
        [PreviewField(Height = 70f)]
        [Group("Box/Horiz/Assets")]
        [PropertyOrder(2)]
        public Texture2D icon;
        [Title("Icon Close", HorizontalLine = false)]
        [HideLabel]
        [PreviewField(Height = 70f)]
        [Group("Box/Horiz/Assets")]
        [PropertyOrder(3)]
        public Texture2D iconClose;

        [Group("Box/Horiz/Verti/Path")]
        [HideLabel]
        [InlineButton(nameof(SetPathUsingBrowser), "Browse...", ButtonWidth = 70f)]
        public string saveFolder;

        [Group("Box/Horiz/Verti/Information/Top")]
        [LabelText("New ID")]
        [LabelWidth(45f)]
        public string customId;
        [Group("Box/Horiz/Verti/Information/Top")]
        [LabelWidth(80f)]
        [InlineButton(nameof(CopyDisplayName))]
        public string displayName;
        [Group("Box/Horiz/Verti/Information/Top")]
        [LabelWidth(45f)]
        public string author;
        [Group("Box/Horiz/Verti/Information")]
        [LabelWidth(65f)]
        [InlineButton(nameof(CopyDescription))]
        public string description;

        [Group("Box/Horiz/Verti/Overrides/Row")]
        [HideReferencePicker]
        [InlineProperty]
        [LabelText("Override Value")]
        [LabelWidth(85f)]
        public WizardOverride<float> value = new WizardOverride<float>((itemData, value) => itemData.value = value);
        [Group("Box/Horiz/Verti/Overrides/Row")]
        [HideReferencePicker]
        [InlineProperty]
        [LabelText("Override Tier")]
        [LabelWidth(85f)]
        public WizardOverride<int> tier = new WizardOverride<int>((itemData, tier) => itemData.tier = tier);
        [Group("Box/Horiz/Verti/Overrides/Row")]
        [HideReferencePicker]
        [InlineProperty]
        [LabelText("Override Mass")]
        [LabelWidth(85f)]
        public WizardOverride<float> mass = new WizardOverride<float>((itemData, mass) => itemData.mass = mass);
        [Group("Box/Horiz/Verti/Overrides/Row")]
        [HideReferencePicker]
        [InlineProperty]
        [LabelText("Override Flags")]
        [LabelWidth(85f)]
        public WizardOverride<ItemFlags> flags = new WizardOverride<ItemFlags>((itemData, flags) => itemData.flags = flags);
        [Group("Box/Horiz/Verti/Overrides/Row")]
        [LabelWidth(95f)]
        public bool clearWhooshes = false;

        [Group("Box/Horiz/Verti/References")]
        [HideReferencePicker]
        [ListDrawerSettings(Draggable = false, HideAddButton = true, HideRemoveButton = true, AlwaysExpanded = true)]
        public List<ItemData.ColliderGroup> colliderGroups;
        [Group("Box/Horiz/Verti/References")]
        [HideReferencePicker]
        [ListDrawerSettings(Draggable = false, HideAddButton = true, HideRemoveButton = true, AlwaysExpanded = true)]
        public List<ItemData.Damager> damagers;
        [Group("Box/Horiz/Verti/References")]
        [HideReferencePicker]
        [ListDrawerSettings(Draggable = false, HideAddButton = true, HideRemoveButton = true, AlwaysExpanded = true)]
        public List<ItemData.Interactable> interactables;

        [System.Serializable]
        [DeclareBoxGroup("Box", HideTitle = true)]
        [DeclareHorizontalGroup("Box/Horiz", Sizes = new float[] { 15f, 0f})]
        public class WizardOverride<T>
        {
            [Group("Box/Horiz")]
            [HideLabel]
            public bool overrideValue;
            [Group("Box/Horiz")]
            [HideLabel]
            [EnableIf(nameof(overrideValue))]
            public T value;
            [NonSerialized]
            public Action<ItemData, T> overrideAction;

            public WizardOverride(Action<ItemData, T> overrideAction)
            {
                this.overrideAction = overrideAction;
            }

            public void Run(ItemData itemData)
            {
                if (!overrideValue) return;
                overrideAction?.Invoke(itemData, value);
            }

            public void Clear()
            {
                overrideValue = false;
                value = default;
            }
        }

        public void ItemPrefabSet()
        {
            if (itemPrefab == lastSetItem) return;
            if (itemPrefab == null)
            {
                colliderGroups = new();
                damagers = new();
                interactables = new();
                lastSetItem = null;
                return;
            }
            string itemPrefabAddress = Catalog.GetAddressFromPrefab(itemPrefab);
            if (!itemPrefabAddress.IsNullOrEmptyOrWhitespace())
            {
                icon = Catalog.EditorLoad<Texture2D>(itemPrefabAddress + ".Icon");
                iconClose = Catalog.EditorLoad<Texture2D>(itemPrefabAddress + ".IconClose");
                if (!itemPrefab.itemId.IsNullOrEmptyOrWhitespace()) customId = itemPrefab.itemId;
                colliderGroups = new List<ItemData.ColliderGroup>();
                FindAllAddMatching<ColliderGroup, ItemData.ColliderGroup>(colliderGroups, (cg, icg) =>
                {
                    icg.transformName = cg.gameObject.name;
                    icg.colliderGroupId = cg.colliderGroupId;
                    if (icg.colliderGroupId.IsNullOrEmptyOrWhitespace())
                    {
                        foreach (ItemData.ColliderGroup baseCG in baseData.colliderGroups)
                        {
                            if (baseCG.transformName == icg.transformName)
                            {
                                icg.colliderGroupId = baseCG.colliderGroupId;
                                break;
                            }
                        }
                    }
                });
                damagers = new List<ItemData.Damager>();
                FindAllAddMatching<Damager, ItemData.Damager>(damagers, (d, id) =>
                {
                    id.transformName = d.gameObject.name;
                    foreach (ItemData.Damager baseDamager in baseData.damagers)
                    {
                        if (baseDamager.transformName == id.transformName)
                        {
                            id.damagerID = baseDamager.damagerID;
                            break;
                        }
                    }
                });
                interactables = new List<ItemData.Interactable>();
                FindAllAddMatching<Interactable, ItemData.Interactable>(interactables, (i, ii) =>
                {
                    ii.transformName = i.gameObject.name;
                    ii.interactableId = i.interactableId;
                    if (ii.interactableId.IsNullOrEmptyOrWhitespace())
                    {
                        foreach (ItemData.Interactable baseInteractable in baseData.Interactables)
                        {
                            if (baseInteractable.transformName == ii.transformName)
                            {
                                ii.interactableId = baseInteractable.interactableId;
                                break;
                            }
                        }
                    }
                });
            }
            else
            {
                Debug.LogError("Item prefab in wizard is not addressable! The wizard cannot run if the item is not addressable");
            }
            lastSetItem = itemPrefab;
        }

        private void FindAllAddMatching<T, V>(List<V> addList, Action<T, V> setInfo) where T : MonoBehaviour where V : new()
        {
            foreach (T mono in itemPrefab.GetComponentsInChildren<T>())
            {
                V monoInfo = new V();
                setInfo(mono, monoInfo);
                addList.Add(monoInfo);
            }
        }

        public void CopyDisplayName()
        {
            displayName = baseData?.displayName ?? string.Empty;
        }

        public void CopyDescription()
        {
            description = baseData?.description ?? string.Empty;
        }

        public void SetPathUsingBrowser()
        {
            saveFolder = EditorUtility.OpenFolderPanel("Select folder", Path.GetDirectoryName(baseData.realFilePath), "Mod folder");
        }

        [Button]
        [Group("Box/Horiz/Assets")]
        [PropertyOrder(0)]
        public void Clear()
        {
            itemPrefab = null;
            icon = null;
            iconClose = null;
            customId = string.Empty;
            displayName = string.Empty;
            author = string.Empty;
            description = string.Empty;
            value.Clear();
            tier.Clear();
            mass.Clear();
            flags.Clear();
            clearWhooshes = false;
        }

        public MessageType IsReadyForExport(out string text)
        {
            MessageType result = MessageType.None;
            void UpdateResult(MessageType target)
            {
                result = (MessageType)Mathf.Max((int)result, (int)target);
            }
            text = "Issues found:";
            if (saveFolder.IsNullOrEmptyOrWhitespace())
            {
                UpdateResult(MessageType.Error);
                text += "\n !! Save folder is empty or whitespace";
            }
            if (customId.IsNullOrEmptyOrWhitespace())
            {
                UpdateResult(MessageType.Error);
                text += "\n !! Item needs an ID";
            }
            if (Catalog.GetAddressFromPrefab(itemPrefab).IsNullOrEmptyOrWhitespace())
            {
                UpdateResult(MessageType.Error);
                text += "\n !! Item prefab not set to addressable";
            }
            if (!icon)
            {
                UpdateResult(MessageType.Info);
                text += "\n  - No icon set";
            }
            if (icon && Catalog.GetAddressFromPrefab(icon).IsNullOrEmptyOrWhitespace())
            {
                UpdateResult(MessageType.Warning);
                text += "\n  ! Icon is not set to addressable";
            }
            if (!iconClose)
            {
                UpdateResult(MessageType.Info);
                text += "\n  - No closeup icon set";
            }
            if (iconClose && Catalog.GetAddressFromPrefab(iconClose).IsNullOrEmptyOrWhitespace())
            {
                UpdateResult(MessageType.Warning);
                text += "\n  ! Closeup icon is not set to addressable";
            }
            if (author.IsNullOrEmptyOrWhitespace())
            {
                UpdateResult(MessageType.Info);
                text += "\n  - Author name not set";
            }
            if (displayName.IsNullOrEmptyOrWhitespace())
            {
                UpdateResult(MessageType.Info);
                text += "\n  - Display name not set";
            }
            if (description.IsNullOrEmptyOrWhitespace())
            {
                UpdateResult(MessageType.Info);
                text += "\n  - Description not set";
            }
            return result;
        }

        public ItemData GetModifiedData()
        {
            ItemData cloned = baseData.CloneJson();
            cloned.filePath = saveFolder + "/Item_" + cloned.category + "_" + cloned.id + ".json";
            cloned.prefabAddress = Catalog.GetAddressFromPrefab(itemPrefab);
            cloned.iconAddress = Catalog.GetAddressFromPrefab(icon);
            cloned.closeUpIconAddress = Catalog.GetAddressFromPrefab(iconClose);
            cloned.id = customId;
            cloned.localizationId = customId;
            cloned.displayName = displayName;
            cloned.description = description;
            cloned.author = author;
            value.Run(cloned);
            tier.Run(cloned);
            mass.Run(cloned);
            flags.Run(cloned);
            if (clearWhooshes) cloned.whooshs = new List<ItemData.Whoosh>();
            cloned.colliderGroups = colliderGroups;
            cloned.damagers = damagers;
            cloned.Interactables = interactables;
            return cloned;
        }
    }
}
