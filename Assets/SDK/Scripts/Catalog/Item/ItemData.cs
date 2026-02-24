using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Collections;
using System.Globalization;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using TriInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ThunderRoad
{
    [Flags]
    public enum ItemFlags
    {
        None = 0,
        Throwable = 1,
        Heavy = 2,
        Spinnable = 4,
        Jabbing = 8,
        Defensive = 16,
        Ricochet = 32,
        Piercing = 64,
        Electromagnetic = 128,
        Blunt = 256,
        NoChargeSapping = 512,
        NoMoltenArc = 1024
    }

    [Serializable]
    [AutoLabelWidth(AddedWidth = 0f, MinWidth = 0f)]
#if UNITY_EDITOR
    [EditorLoadPriority(-10)]
    [DeclareHorizontalGroup("Split")]
    [DeclareVerticalGroup("Split/Left")]
    [DeclareBoxGroup("Split/Left/General", Title = "General")]
    [DeclareBoxGroup("Split/Left/Prefab", Title = "Addressables")]
    [DeclareHorizontalGroup("Split/Left/Prefab/horiz", Sizes = new float[] { 0f, 18f * 9f })]
    [DeclareVerticalGroup("Split/Left/Prefab/horiz/vert")]
    [DeclareHorizontalGroup("Split/Left/Prefab/horiz/previews")]
    [DeclareHorizontalGroup("Split/Left/Prefab/pooling")]
    [DeclareBoxGroup("Split/Left/Inventory", Title = "Inventory")]
    [DeclareHorizontalGroup("Split/Left/Inventory/Audiohoriz")]
    [DeclareBoxGroup("Split/Left/Holder", Title = "Holder")]
    [DeclareHorizontalGroup("Split/Left/Holder/Snaphoriz")]
    [DeclareBoxGroup("Split/Left/Rigidbody", Title = "Rigidbody")]
    [DeclareBoxGroup("Split/Left/Bonus", Title = "Bonus")]
    [DeclareBoxGroup("Split/Left/Collisions", Title = "Collisions")]
    [DeclareBoxGroup("Split/Left/Water", Title = "Water")]
    [DeclareBoxGroup("Split/Left/Object", Title = "Object")]
    [DeclareBoxGroup("Split/Left/Holder/Imbuing", Title = "Imbuing")]
    [DeclareVerticalGroup("Split/Right")]
    [DeclareBoxGroup("Split/Right/Modules", Title = "Modules")]
#endif
    public class ItemData : EntityData, IContainerLoadable<ItemData>, ILootTableDrop
    {
        [LabelWidth(200f)]
        [Group("Split/Left/General")]
        public string localizationId;
        [Group("Split/Left/General")]
        public string displayName;
        [Group("Split/Left/General")]
        [TextArea(4, 14)]
        public string description;
        [Group("Split/Left/General")]
        public string author = "Unknown";
        [Group("Split/Left/General")]
        [Dropdown(nameof(GetAllValueTypes))]
        public string valueType = "Gold";
        [Group("Split/Left/General")]
        public float value;
        [Group("Split/Left/General")]
        public float rewardValue;


        [Group("Split/Left/General")]
        public int tier;

        [NonSerialized]
        public string tierString;

        [NonSerialized]
        public string weight;

        [NonSerialized]
        public string size;

        [Group("Split/Left/General")]
        public ItemFlags flags;

        [Group("Split/Left/General")]
        public int levelRequired;
        [Group("Split/Left/General")]
        public Type type = Type.Misc;
        [Group("Split/Left/General")]
        [Dropdown(nameof(GetCategories))]
        public string category;
        [Group("Split/Left/General")]
        public string update = string.Empty;
        [Group("Split/Left/General")]
        [Dropdown(nameof(GetAllEffectID))]
        public string iconEffectId;

        [NonSerialized]
        public EffectData iconEffectData;
        [Group("Split/Left/General")]
        public CenterType preferredItemCenter = CenterType.Mass;
        [Group("Split/Left/General")]
        public bool drainImbueWhenIdle = true;

        [Group("Split/Left/General")]
        [Tooltip("Forces skills like Arcwire, Inferno Staff and Gravity Hammer to use a particular button (instead of what is configured as Alt Use)")]
        public ImbueUseButton forceImbueUseButton = ImbueUseButton.None;


        // PREFAB
        [Group("Split/Left/Prefab/horiz/vert")]
        [LabelWidth(90f)]
        public string prefabAddress;

        [NonSerialized]
        public IResourceLocation prefabLocation;
        [Group("Split/Left/Prefab/horiz/vert")]
        [ShowInInspector]
        [LabelWidth(90f)]
        public string iconAddress;

        [NonSerialized]
        public IResourceLocation iconLocation;

        [Group("Split/Left/Prefab/horiz/vert")]
        [ShowInInspector]
        [LabelWidth(110f)]
        public string closeUpIconAddress;

        [NonSerialized]
        public IResourceLocation closeUpIconLocation;

        [Group("Split/Left/Prefab/horiz/previews")]
        [NonSerialized]
        [ShowInInspector]
        [HideLabel]
        [DisableInPlayMode]
#if UNITY_EDITOR
        [OnValueChanged(nameof(EditorUpdatePrefabAddress))]
        [PreviewField(LinesHeight = 3)]
        public GameObject editorPrefab;

        [NonSerialized]
        [ShowInInspector]
        [Group("Split/Left/Prefab/horiz/previews")]
        [HideLabel]
        [PreviewField(LinesHeight = 3)]
        [OnValueChanged(nameof(EditorUpdateIconAddress))]
#endif
        public Sprite icon;

        [NonSerialized]
        [ShowInInspector]
        [Group("Split/Left/Prefab/horiz/previews")]
        [HideLabel]
        [PreviewField(LinesHeight = 3)]
#if UNITY_EDITOR
        [OnValueChanged(nameof(EditorUpdateCloseUpIconAddress))]
#endif
        public Sprite closeUpIcon;

        [Group("Split/Left/Prefab/pooling")]
        [LabelWidth(120f)]
        [LabelText("PCVR Pooled Count")]
        public int pooledCount;

        [Group("Split/Left/Prefab/pooling")]
        [LabelWidth(125f)]
        public int androidPooledCount;

        public enum Type
        {
            Misc,
            Weapon,
            Quiver,
            Potion,
            Prop,
            Body,
            Shield,
            Wardrobe,
            Spell,
            Crystal,
            Valuable,
            Tool,
            Food,
            Note,
        }

        static int spawningAsyncCount;

#if UNITY_EDITOR
        protected void EditorUpdatePrefabAddress()
        {
            if (editorPrefab) prefabAddress = Catalog.GetAddressFromPrefab(editorPrefab);
        }

        protected void EditorUpdateIconAddress()
        {
            if (icon) iconAddress = Catalog.GetAddressFromPrefab(icon);
        }

        protected void EditorUpdateCloseUpIconAddress()
        {
            if (closeUpIcon) closeUpIconAddress = Catalog.GetAddressFromPrefab(closeUpIcon);
        }

        public override string GetCatalogPath()
        {
            string result = $"Items/{type}";
            if (!groupPath.IsNullOrEmptyOrWhitespace()) result += $"/{groupPath}";
            if (result[result.Length - 1] == '/') result = result.Substring(0, result.Length - 1);
            return result;
        }

        public override Texture2D Icon()
        {
            bool needIcon = allowedStorage.HasFlag(ItemData.Storage.Inventory) && !despawnOnStoredInInventory;
            if (allowedStorage.HasFlag(ItemData.Storage.Container)) needIcon = true;
            if (needIcon && icon != null)
            {
                return AssetPreview.GetAssetPreview(icon);
            }
            return null;
        }

        public override string GetFileNameString()
        {
            return base.GetFileNameString().Replace("_", $"_{type.ToString()}_");
        }
#endif

        [Group("Split/Left/Prefab")]
        [NonSerialized]
        [ShowInInspector]
        [ReadOnly]
        public List<Item.IconMarker> iconDamagerMarkers = new();

        [Group("Split/Left/Inventory")]
        public Storage allowedStorage;

        [Group("Split/Left/Inventory")]
        public bool despawnOnStoredInInventory;

        [Group("Split/Left/Inventory")]
        [Tooltip("If true, it can be stacked to an infinite amount")]
        public bool isStackable;
        [Group("Split/Left/Inventory")]
        public string consumableId;

        [NonSerialized]
        public int consumableHashId;

        [Group("Split/Left/Inventory/Audiohoriz")]
        public string inventoryAudioContainerAddress;
        [Group("Split/Left/Inventory")]
        public float inventoryAudioVolume_dB;

        [NonSerialized]
        public IResourceLocation inventoryAudioContainerLocation;

#if UNITY_EDITOR
        [Group("Split/Left/Inventory/Audiohoriz")]
        [LabelWidth(50)]
        [HideLabel]
        [NonSerialized]
        [ShowInInspector]
        [DisableInPlayMode]
        [OnValueChanged(nameof(EditorUpdateInventoryAudioContainerAddress))]
        public AudioContainer editorInventoryAudioContainer;

        protected void EditorUpdateInventoryAudioContainerAddress()
        {
            if (editorInventoryAudioContainer)
                inventoryAudioContainerAddress = Catalog.GetAddressFromPrefab(editorInventoryAudioContainer);
        }
#endif

        [Flags]
        public enum Storage
        {
            Inventory = 1,
            Container = 2,
            SandboxAllItems = 4,
        }

        [Group("Split/Left/Holder")]
        [Dropdown(nameof(GetAllHolderSlots))]
        public string slot;
        [Group("Split/Left/Holder/Snaphoriz")]
        public string snapAudioContainerAddress;
        [Group("Split/Left/Holder")]
        public float snapAudioVolume_dB;

        [NonSerialized]
        public IResourceLocation snapAudioContainerLocation;

#if UNITY_EDITOR
        [Group("Split/Left/Holder/Snaphoriz")]
        [LabelWidth(50)]
        [HideLabel]
        [NonSerialized]
        [ShowInInspector]
        [DisableInPlayMode]
        [OnValueChanged(nameof(EditorUpdateSnapAudioContainerAddress))]
        public AudioContainer editorSnapAudioContainer;

        protected void EditorUpdateSnapAudioContainerAddress()
        {
            if (editorSnapAudioContainer)
                snapAudioContainerAddress = Catalog.GetAddressFromPrefab(editorSnapAudioContainer);
        }
#endif

        public TriDropdownList<string> GetAllHolderSlots() => Catalog.GetDropdownHolderSlots();

        [Group("Split/Left/Rigidbody")]
        public bool overrideMassAndDrag;
        [Group("Split/Left/Rigidbody")]
        [ShowIf("overrideMassAndDrag")]
        public float mass = 1;
        [Group("Split/Left/Rigidbody")]
        [ShowIf("overrideMassAndDrag")]
        public float drag = 1;
        [Group("Split/Left/Rigidbody")]
        [ShowIf("overrideMassAndDrag")]
        public float angularDrag = 1;
        [Group("Split/Left/Rigidbody")]
        [Tooltip("Density in kg/m3, water is 1000, human: 985 wood: 700, bone: 1500, brick: 1700, porcelain/stone: 2500, steel: 7900, lead: 11500")]
        public float density = 1000;

        [Group("Split/Left/Bonus")]
        public float focusRegenMultiplier = 1;
        [Group("Split/Left/Bonus")]
        public float spellChargeSpeedPlayerMultiplier = 1;
        [Group("Split/Left/Bonus")]
        public float spellChargeSpeedNPCMultiplier = 1;

        [Group("Split/Left/Collisions")]
        public int collisionMaxOverride;
        [Group("Split/Left/Collisions")]
        public bool collisionEnterOnly;
        [Group("Split/Left/Collisions")]
        public bool collisionNoMinVelocityCheck;
        [Group("Split/Left/Collisions")]
        public LayerName forceLayer = LayerName.None;
        [Group("Split/Left/Collisions")]
        public bool diffForceLayerWhenHeld = false;
        [Group("Split/Left/Collisions")]
        public LayerName forceLayerHeld = LayerName.None;

        [Group("Split/Left/Water")]
        public AnimationCurve waterHandSpringMultiplierCurve = AnimationCurve.EaseInOut(0, 0.3f, 1, 0.15f);
        [Group("Split/Left/Water")]
        public AnimationCurve waterDragMultiplierCurve = AnimationCurve.EaseInOut(0, 1, 1, 10);
        [Group("Split/Left/Water")]
        public float waterSampleMinRadius = 0.2f;

        [Group("Split/Left/Object")]
        public float throwMultiplier = 1;

        [Group("Split/Left/Bonus")]
        public float runSpeedMultiplier = 1;

        [Group("Split/Left/Object")]
        public float flyRotationSpeed = 2;
        [Group("Split/Left/Object")]
        public float flyThrowAngle = 0;
        [Group("Split/Left/Object")]
        public bool allowFlyBackwards = false;

        [Group("Split/Left/Object")]
        public float telekinesisSafeDistance = 1;
        [Group("Split/Left/Object")]
        public float telekinesisThrowRatio = 1;
        [Group("Split/Left/Object")]
        public bool telekinesisAutoGrabAnyHandle;
        [Group("Split/Left/Object")]
        public bool grippable = false;
        [Group("Split/Left/Object")]
        public bool grabAndGripClimb = false;
        [Group("Split/Left/Object")]
        [Tooltip("Change mesh and collider layer to MovingItem when grabbed or gripped")]
        public bool playerGrabAndGripChangeLayer = true;

        public enum DamageTransfer
        {
            Self,
            Handler,
            None,
        }

        [Group("Split/Left/Holder")]
        [HideReferencePicker]
        public List<CustomSnap> customSnaps = new();

        [Group("Split/Left/Holder/Imbuing")]
        public bool drainImbueOnSnap = true;

        [Group("Split/Left/Holder/Imbuing")]
        [Tooltip("X → Time (seconds). Y → Imbue value (normalized [0; 1])")]
        public AnimationCurve imbueEnergyOverTimeOnSnap
            = AnimationCurve.EaseInOut(0, 1, 3, 0);

        [Serializable]
        public class CustomSnap
        {
            [JsonMergeKey]
            public string holderName;

            public Vector3 localPosition;
            public Vector3 localRotation;
        }

        [Group("Split/Right")]
        [HideReferencePicker]
        public List<ColliderGroup> colliderGroups = new();

        [Group("Split/Right")]
        [HideReferencePicker]
        public List<Damager> damagers = new();

        [Group("Split/Right")]
        [HideReferencePicker]
        public List<Interactable> Interactables = new();

        [Group("Split/Right")]
        [HideReferencePicker]
        public List<EffectHinge> effectHinges = new();

        [Group("Split/Right")]
        [HideReferencePicker]
        public List<Whoosh> whooshs = new();

        [Group("Split/Right")]
        [ShowInInspector]
        [SerializeReference]
        [ReadOnlyReferencePicker]
        [ListDrawerSettings(DontForceInline = true)]
        public List<ItemModule> modules;

        [Serializable]
        public class ColliderGroup
        {
            [JsonMergeKey]
            [LabelWidth(100f)]
            public string transformName = "Blades";
            [Dropdown(nameof(GetAllColliderGroupID))]
            [LabelWidth(100f)]
            public string colliderGroupId;

            [NonSerialized]
            public ColliderGroupData colliderGroupData;

            public TriDropdownList<string> GetAllColliderGroupID() => Catalog.GetDropdownAllID(Category.ColliderGroup);

            public void OnCatalogRefresh(string itemId)
            {
                if (!Catalog.TryGetData<ColliderGroupData>(colliderGroupId, out colliderGroupData))
                {
                    Debug.LogWarning(
                        $"ColliderGroupData not found for colliderGroupID {colliderGroupId} on transform {transformName} on item {itemId}");
                }
            }
        }

        [Serializable]
        public class Interactable
        {
            [JsonMergeKey]
            [LabelWidth(100f)]
            public string transformName = "Handle";
            [Dropdown(nameof(GetAllInteractableID))]
            [LabelWidth(100f)]
            public string interactableId;

            public TriDropdownList<string> GetAllInteractableID() => Catalog.GetDropdownAllID(Category.Interactable);
        }

        [Serializable]
        public class EffectHinge
        {
            [JsonMergeKey]
            [LabelWidth(100f)]
            public string transformName = "EffectHinge";
            [Dropdown(nameof(GetAllEffectID))]
            [LabelWidth(70f)]
            public string effectId;

            public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);
            public float minTorque = 5;
            public float maxTorque = 12;
        }

        [Serializable]
        public class Whoosh
        {
            [JsonMergeKey]
            [LabelWidth(100f)]
            public string transformName = "Whoosh";
            [Dropdown(nameof(GetAllEffectID))]
            [LabelWidth(70f)]
            public string effectId;

            public WhooshPoint.Trigger trigger = WhooshPoint.Trigger.Always;
            public bool stopOnSnap = true;
            public float minVelocity = 5;
            public float maxVelocity = 12;
            public float dampening = 0.1f;
            public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);
        }

        public TriDropdownList<string> GetAllValueTypes()
        {
            if (Catalog.gameData == null) return new TriDropdownList<string>() { needsValidation = false };
            return Catalog.gameData.GetAllValueTypes();
        }

        public TriDropdownList<string> GetCategories()
        {
            if (Catalog.gameData == null) return new TriDropdownList<string>() { needsValidation = false };
            return Catalog.gameData.GetCategories();
        }

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

        [Serializable]
        public class Damager
        {
            [JsonMergeKey]
            [LabelWidth(100f)]
            public string transformName = "Damager";
            [Dropdown(nameof(GetAllDamagerID))]
            [LabelWidth(90f)]
            public string damagerID;

            [NonSerialized]
            public DamagerData damagerData;

            public TriDropdownList<string> GetAllDamagerID() => Catalog.GetDropdownAllID(Category.Damager);

        }

        [NonSerialized]
        public ItemModuleAI moduleAI;


        public bool TryGetModule<T>(out T module) where T : ItemModule
        {
            module = null;
            if (!modules.TryGetCount(out int modulesCount)) return false;
            for (var i = 0; i < modulesCount; i++)
            {
                ItemModule itemModule = modules[i];
                if (itemModule is T foundModule)
                {
                    module = foundModule;
                    return true;
                }
            }

            return false;
        }

        public T GetModule<T>() where T : ItemModule
        {
            if (modules != null)
            {
                foreach (ItemModule itemModule in modules)
                {
                    if (itemModule is T) return itemModule as T;
                }
            }

            return null;
        }

        public bool HasModule<T>() where T : ItemModule
        {
            if (modules != null)
            {
                foreach (ItemModule itemModule in modules)
                {
                    if (itemModule is T) return true;
                }
            }

            return false;
        }

        public override int GetCurrentVersion()
        {
            return 4;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // METHODS
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void OnLoadedFromContainer(Container container)
        {
        }

        public ContainerContent InstanceContent()
        {
            return new ItemContent(this);
        }
    }

    public enum CenterType
    {
        Mass,
        Root,
        Renderer,
        MainHandle,
        Holder
    }
}
