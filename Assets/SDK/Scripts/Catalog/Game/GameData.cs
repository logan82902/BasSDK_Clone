using UnityEngine;
using System.Collections.Generic;
using System;

using TriInspector;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Collections;
using System.Linq;
using UnityEngine.Rendering;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [Serializable]
    [AutoLabelWidth(MinWidth = 0f)]
    [DeclareFoldoutGroup("Water Data", Expanded = false)]
    [DeclareFoldoutGroup("Fluids (Drag)", Expanded = false)]
    [DeclareFoldoutGroup("Platform Parameters", Expanded = false)]
    [DeclareFoldoutGroup("Collision Detection", Expanded = false)]
    [DeclareFoldoutGroup("Physic Material ID Map colours", Expanded = false)]
    [DeclareFoldoutGroup("Haptics", Expanded = false)]
    public class GameData
    {
        public const string savesFolder = "Default";

#if UNITY_EDITOR
        [JsonIgnore]
        [HideInInspector]
        public const string defaultEditorId = "ORIGINAL";
        [JsonIgnore]
        [HideInInspector]
        public const string overrideEditorId = "OVERRIDE";
        [JsonIgnore]
        [HideInInspector]
        public string filePath;
        [JsonIgnore]
        [HideInInspector]
        public string editorId = defaultEditorId;
        [JsonIgnore]
        [HideInInspector]
        public List<GameData> overriders;
        [JsonIgnore]
        [HideInInspector]
        public bool isOverrider => editorId != defaultEditorId;

        public string GetFolderName()
        {
            string path = System.IO.Path.GetDirectoryName(filePath);
            return path.Substring(path.LastIndexOf('\\') + 1);
        }
#endif

        public bool overrideSaveCharacters = false;
        public bool overrideSaveOptions = false;
        public bool disableCharacterCustomization = false;
        public bool disableCharacterDeletion = false;

        public bool showControlPanelOnStart = false;
        public bool arcadeControlPanel;

        public bool fullscreen = false;
        public int screenWidth = 1920;
        public int screenHeight = 1080;
        
        [NonSerialized]
        public string sourceFolders;

        public string GetLatestOverrideFolder()
        {
            return sourceFolders.Split('+').Last();
        }


        public string defaultPlayerMaleCreatureID = "PlayerDefaultMale";
        public string defaultPlayerFemaleCreatureID = "PlayerDefaultFemale";


        [Dropdown(nameof(GetAllEffectID))]
        public string spellWheelEffectId;
        [NonSerialized]

        [Dropdown(nameof(GetAllHandPoseID))]
        public string SpellWheelOpenHandPoseId;
        [NonSerialized]
        public HandPoseData SpellWheelOpenHandPoseData;

        [Dropdown(nameof(GetAllHandPoseID))]
        public string SpellWheelCloseHandPoseId;
        [NonSerialized]
        public HandPoseData SpellWheelCloseHandPoseData;

        [Dropdown(nameof(GetAllHandPoseID))]
        public string uiPointingCloseHandPoseId;
        [NonSerialized]
        public HandPoseData uiPointingCloseHandPoseData;


        [Dropdown(nameof(GetAllEffectID))]
        public string skillOrbInHandEffectId;
        [Dropdown(nameof(GetAllEffectID))]
        public string defaultWheelOrbEffectId;
        public string defaultPostProcessAddress;
        [TableList()]
        public List<PostProcessProfile> postProcessProfiles = new();

        [Serializable]
        public class PostProcessProfile
        {
            public string id;
            public string address;
            [NonSerialized]
            public VolumeProfile volumeProfile;
        }

        [ColorPickerField(Alpha = true, HDR = true)]
        public Color[] tierColors;

        public string heartBeatLoopAddress;
        public string deathEffectId = "Death";
        public float deathSlowMoRatio = 0.05f;
        public float deathSlowMoDuration = 0.25f;
        public AnimationCurve deathSlowMoEnterCurve;
        public AnimationCurve deathSlowMoExitCurve;

        public AnimationCurve mergeDistanceCurve = AnimationCurve.EaseInOut(0f, 0.2f, 1f, 0.8f);
        public int mergeLeftoverShardMultiplier = 4;

        public float playerRespawnDelay = 10f;
        [Group("Platform Parameters")]
        public PlatformParameters pc = new();
        [Group("Platform Parameters")]
        public PlatformParameters android = new();
        [NonSerialized]
        public PlatformParameters platformParameters;

        [Serializable]
        public class PlatformParameters
        {
            public bool dungeonStaticBatching = true;
            public int maxHomeItem = 30;
            public int maxWaveAlive = 20;
            public int maxRoomNpc = 10;
            public int itemSpawnerMaxItems = 100;
            public int itemSpawnerDelay = 0;
            public float mirrorQuality = 1f;

            public float cleanDeadLastInteractionDelay = 7;
            public float cleanObjectsLastInteractionDelay = 7;

            public float aiCycleMinSpeed = 0.1f;
            public float aiCycleMaxSpeed = 2;

            public int poolingAudioCount = 20;
            public int poolingVfxCount = 10;
            public int poolingShaderCount = 100;
            public int poolingRevealCount = 100;
            public int poolingMeshCount = 100;
            public int poolingLightCount = 100;

            public bool enableEffectAudio = true;
            public bool enableEffectVfx = true;
            public bool enableEffectParticle = true;
            public bool enableEffectDecal = true;
            public bool enableEffectMesh = true;
            public bool enableEffectReveal = true;
            public bool enableEffectLight = true;
        }

        [Serializable]
        public class Category
        {
            public string name = "Unknown";
            public string group;
            public string iconAddress;
            [NonSerialized]
            public IResourceLocation iconLocation;
            [NonSerialized]
            public int itemsCount;
        }

        [TableList]
        [HideReferencePicker]
        public List<Faction> factions;

        [Serializable]
        public class Faction
        {
            public int id;
            public string name;
            [LabelWidth(0.0001f)]
            public AttackBehaviour attackBehaviour = AttackBehaviour.Default;

            public enum AttackBehaviour
            {
                Default,
                Ignored,
                Passive,
                Agressive,
            }
        }

        [TableList]
        [HideReferencePicker]
        public List<Category> categories;
        public List<string> holderSlots;
        public int baseSkillCost = 4;
        public ScalingMode skillCostScalingMode = ScalingMode.Linear;
        [ShowIf(nameof(skillCostScalingMode), ScalingMode.Exponential)]
        public float skillCostMultiplierPerTier = 2;

        public ScreenshotFormat screenshotFormat = ScreenshotFormat.JPG;
        public int screenshotJpegQuality = 75;

        public enum ScalingMode
        {
            Linear,
            Exponential
        }

        public enum ScreenshotFormat
        {
            JPG,
            PNG,
            All,
        }

        public List<LayerName> locomotionPhysicsLayers = new List<LayerName>();
        public string endGameMenuPrefabAddress;
        public int itemIconResolution = 256;
        public float throwVelocity = 3;
        public float throwMultiplier = 2;
        [MinMaxSlider(0f, 100f, ShowFields = true)]
        public Vector2 collisionEnterVelocityRange = new(1f, 7f);
        [MinMaxSlider(0f, 20f, ShowFields = true)]
        public Vector2 collisionStayVelocityRange = new(0.5f, 3f);
        public Vector2 penetrationVelocityEffectRange = new(0f, 1f);
        public int maxObjectCollision = 10;
        public int maxFramesToKeepCollision = 5;
        public int maxCollisionStayCheckPerFrame = 100;
        public int cleanFarObjectsSqrDistance = 62500; //250m squared
        public float motionBlurIntensity = 0.35f;

        public float droppedItemImbueDrainNpc = 10;
        public float droppedItemImbueDrainPlayer = 5;

        public bool useDynamicMusic = false;

        public string errorAudioGroupAddress;
        [NonSerialized]
        public AudioContainer errorAudioGroup;

        [Group("Collision Detection")]
        public CollisionDetection collisionDetection;

        [Serializable]
        public class IDMapColors
        {
            public int id;
            public Color color;
            public string label;
        }

        public Color GetIDMapColor(int id)
        {

            foreach (IDMapColors sc in physicMaterialIDMapColours)
            {
                if (sc.id == id)
                {
                    return sc.color;
                }
            }

            return Color.black;
        }

        public int GetIDMapId(Color color)
        {
            foreach (IDMapColors sc in physicMaterialIDMapColours)
            {
                if (sc.color == color)
                {
                    return sc.id;
                }
            }
            //we havent found one at this point, so we can try clamping the color to the nearest one
            float minDistance = float.MaxValue;
            int id = -1;
            foreach (IDMapColors sc in physicMaterialIDMapColours)
            {
                Vector3 colorSample = new Vector3(sc.color.r, sc.color.g, sc.color.b);
                Vector3 colorToCheck = new Vector3(color.r, color.g, color.b);
                float distance = Vector3.Distance(colorSample, colorToCheck);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    id = sc.id;
                }
            }
            return id;

        }


        [Group("Physic Material ID Map colours")]
        [Tooltip("The colours used to represent the different physic material ids in the ID map texture, 16 ids are supported")]
        public List<IDMapColors> physicMaterialIDMapColours = new List<IDMapColors>();

        [Serializable]
        public class CollisionDetection
        {
            public CollisionDetectionMode grabbed = CollisionDetectionMode.ContinuousSpeculative;
            public CollisionDetectionMode dropped = CollisionDetectionMode.Discrete;
            public CollisionDetectionMode throwed = CollisionDetectionMode.ContinuousDynamic;
            public CollisionDetectionMode telekinesis = CollisionDetectionMode.ContinuousSpeculative;
            public CollisionDetectionMode npcAttack = CollisionDetectionMode.ContinuousSpeculative;
            public CollisionDetectionMode kick = CollisionDetectionMode.ContinuousSpeculative;
        }
        [Group("Haptics")]
        public Haptics haptics;

        [Serializable]
        public class Haptics
        {
            [Header("Clips")]
            public HapticClip hit;
            public HapticClip bowShoot;
            public HapticClip telekinesisThrow;
            public HapticClip spellSelected;
            [Header("Telekinesis")]
            public Vector2 telekinesisIntensity = new Vector2(0.05f, 0.2f);
            public float telekinesisViveCosmosMultiplier = 0.1f;
            public Vector2 telekinesisPeriod = new Vector2(0.06f, 0.03f);
            public float telekinesisFocus = 0.1f;
            public AnimationCurve telekinesisMassIntensity = AnimationCurve.Linear(0.5f, 0.6f, 10, 3f);
            [Header("Bow draw")]
            public float bowDrawPeriod = 0.02f;
            public float bowDrawIntensity = 0.8f;
            [Header("Rubbing")]
            public float rubbingPeriod = 0.02f;
            public float rubbingIntensity = 0.4f;
            [Header("Penetration")]
            public float penetrationPeriod = 0.01f;
            public float penetrationIntensity = 0.6f;
        }

        [Serializable]
        public class HapticClip
        {
            public HapticClip()
            {
                this.curveIntensity = AnimationCurve.Linear(0, 0.05f, 1, 0.2f);
                this.curvePeriod = AnimationCurve.Linear(0, 0.05f, 1, 0.05f);
                this.duration = 1f;
            }

            public HapticClip(AnimationCurve curveIntensity)
            {
                this.curveIntensity = curveIntensity ?? AnimationCurve.Linear(0, 0.05f, 1, 0.2f);
                this.curvePeriod = AnimationCurve.Linear(0, 0.05f, 1, 0.05f);
                this.duration = 1f;
            }

            public HapticClip(AnimationCurve curveIntensity, float amplitude, float duration)
            {
                this.curveIntensity = curveIntensity ?? AnimationCurve.Linear(0, 0.05f, 1, 0.2f);
                this.curvePeriod = AnimationCurve.Linear(0, 0.05f, 1, 0.05f);
                this.duration = duration;
            }

            public AnimationCurve curveIntensity;
            public AnimationCurve curvePeriod;
            public float duration;
        }



        public TriDropdownList<string> GetAllValueTypes()
        {
            TriDropdownList<string> dropdownList = new();
            dropdownList.Add("Gold", "Gold");
            dropdownList.Add("CrystalShard", "CrystalShard");
            return dropdownList;
        }

        public TriDropdownList<string> GetCategories()
        {
            TriDropdownList<string> dropdownList = new();
            foreach (Category category in categories)
            {
                dropdownList.Add(category.name, category.name);
            }
#if UNITY_EDITOR
            if (overriders.IsNullOrEmpty()) return dropdownList;
            foreach (GameData overrider in overriders)
            {
                foreach (Category category in overrider.categories)
                {
                    if (dropdownList.Contains(new TriDropdownItem<string>() { Value = category.name }, new TriDropdownItemComparer<string>()))
                    {
                        continue;
                    }
                    dropdownList.Add($"{category.name} ({overrider.GetFolderName()})", category.name);
                }
            }
#endif
            return dropdownList;
        }

        public TriDropdownList<string> GetCategoryGroups()
        {
            TriDropdownList<string> categoryGoups = new();
            foreach (Category category in categories)
            {
                if (categoryGoups.Contains(new TriDropdownItem<string>() { Value = category.group }, new TriDropdownItemComparer<string>()))
                {
                    continue;
                }
                categoryGoups.Add(category.group, category.group);
            }
#if UNITY_EDITOR
            if (overriders.IsNullOrEmpty()) return categoryGoups;
            foreach (GameData overrider in overriders)
            {
                foreach (Category category in overrider.categories)
                {
                    if (categoryGoups.Contains(new TriDropdownItem<string>() { Value = category.group }, new TriDropdownItemComparer<string>()))
                    {
                        continue;
                    }
                    categoryGoups.Add($"{category.group} ({overrider.GetFolderName()})", category.group);
                }
            }
#endif
            return categoryGoups;
        }


        public Faction GetFaction(int factionId)
        {
            foreach (Faction faction in factions)
            {
                if (faction.id == factionId)
                {
                    return faction;
                }
            }
            return null;
        }

        public TriDropdownList<int> GetFactions()
        {
            TriDropdownList<int> dropdownList = new();
            foreach (Faction faction in factions)
            {
                dropdownList.Add(faction.name, faction.id);
            }
#if UNITY_EDITOR
            if (overriders.IsNullOrEmpty()) return dropdownList;
            foreach (GameData overrider in overriders)
            {
                foreach (Faction faction in overrider.factions)
                {
                    if (dropdownList.Contains(new TriDropdownItem<int>() { Value = faction.id }, new TriDropdownItemComparer<int>()))
                    {
                        continue;
                    }
                    dropdownList.Add($"{faction.name} ({overrider.GetFolderName()})", faction.id);
                }
            }
#endif
            return dropdownList;
        }

        public TriDropdownList<string> GetAllLevelID() => Catalog.GetDropdownAllID(ThunderRoad.Category.Level);

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(ThunderRoad.Category.Effect);

        public TriDropdownList<string> GetAllHandPoseID() => Catalog.GetDropdownAllID(ThunderRoad.Category.HandPose);



        public void OnCatalogRefresh()
        {
            //check if any of the physic material id map colours have duplicate ids, or duplicate colours or are out of range
            var duplicateIds = physicMaterialIDMapColours.GroupBy(x => x.id).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
            if (duplicateIds.Count > 0)
            {
                string join = string.Join(", ", duplicateIds);
                Debug.LogError($"Duplicate ids found in physicMaterialIDMapColours: {join}");
            }
            var duplicateColours = physicMaterialIDMapColours.GroupBy(x => x.color).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
            if (duplicateColours.Count > 0)
            {
                string join = string.Join(", ", duplicateColours);
                Debug.LogError($"Duplicate colours found in physicMaterialIDMapColours: {join}");
            }
            if (physicMaterialIDMapColours.Any(sc => sc.id < 0 || sc.id > 15))
            {
                var values = physicMaterialIDMapColours.Where(sc => sc.id is < 0 or > 15).Select(sc => sc.id);
                string join = string.Join(", ", values);
                Debug.LogError($"Invalid ids found in physicMaterialIDMapColours: {join}");
            }
        }


        public IEnumerator OnCatalogRefreshCoroutine()
        {
            yield return null;
        }
    }
}
