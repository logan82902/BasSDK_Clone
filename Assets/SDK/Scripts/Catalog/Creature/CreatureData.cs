using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using ThunderRoad.Manikin;


using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(0)]
#endif
    [DeclareBoxGroup("Pooling", Title = "Pooling")]
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("Stats", Title = "Stats")]
    [DeclareBoxGroup("Overlap sphere", Title = "Overlap sphere")]
    [DeclareBoxGroup("AI", Title = "AI")]
    [DeclareTabGroup("Tabs")]
    public class CreatureData : EntityData
    {
        [Group("Pooling")]
        public int pooledCount;

        [Group("Pooling")]
        public int androidPooledCount;

        [Group("General")]
        public string name;
        private int _creatureNameHash = -1;
        [JsonIgnore]
        public int creatureNameHash {
            get
            {
                if (_creatureNameHash == -1)
                {
                    _creatureNameHash = Animator.StringToHash(name);
                }
                return _creatureNameHash;
            }
        }
        [Group("General")]
        public CreatureType type;
        [Group("General")]
        public string prefabAddress;
        [Group("General")]
        public string manikinChannelLayersAddress = "Bas.Manikin.ChannelLayers";
        [JsonIgnore]
        public ManikinEditorLocationLabels manikinChannelLayers
        {
            get
            {
                if (channelLayersByID == null)
                {
                    channelLayersByID = new Dictionary<string, ManikinEditorLocationLabels>();
                }
                if (channelLayersByID.TryGetValue(id, out var channelLayers)) return channelLayers;
                return null;
            }
            set
            {
                if (channelLayersByID == null)
                {
                    channelLayersByID = new Dictionary<string, ManikinEditorLocationLabels>();
                }
                channelLayersByID[id] = value;
            }
        }
        public static Dictionary<string, ManikinEditorLocationLabels> channelLayersByID;
        [Group("General")]
        public string animatorBundleAddress;
        [JsonIgnore]
        public IResourceLocation prefabLocation;

        [Group("General")]
        public bool removeMeshWhenPooled;

        [Group("General")]
        public Gender gender = Gender.None;
        [Group("General")]
        [Tooltip("The density of the creatures body in KG/m³. Used to calculate mass of the ragdoll and for buoyancy calculations.")]
        public float density = 985f; // for humans

        [Group("General")]
        public bool allowStatus = true;

        [Group("Stats")]
        public int health = 50;
        [Group("Stats")]
        public float focus = 30f;
        [Group("Stats")]
        public float focusRegen = 2;
        [Group("Stats")]
        [LabelText("$" + nameof(MaxFocusLabel))]
        public float maxFocusPerSkill = 0.12f;
        [Group("Stats")]
        [LabelText("$" + nameof(FocusRegenLabel))]
        public float focusRegenPerSkill = 0f;
        [Group("Stats")]
        [LabelText("$" + nameof(MaxHealthLabel))]
        public float maxHealthPerSkill = 0.12f;

        public string MaxFocusLabel() => SkillPassiveLabel("Max Focus", maxFocusPerSkill);

        public string FocusRegenLabel() => SkillPassiveLabel("Focus Regen", focusRegenPerSkill);

        public string SkillPassiveLabel(string name, float value)
            => $"{name} Mult. per Skill (max +{value * 3 * 100}%)";

        [JsonIgnore] public string MaxHealthLabel => $"Max Health Mult. per Skill (max {health * (1 + maxHealthPerSkill * 3)})";

        [Group("Stats")]
        public float physicsOffDamageMult = 0.33f;

        [Group("Stats")]
        public float heatGainMultiplier = 1;

        [Group("Stats")]
        public float heatLossMultiplier = 1;

        [Group("Overlap sphere")]
        public float overlapRadius = 5;
        [Group("Overlap sphere")]
        public float overlapMinDelay = 0.5f;
        [Group("Overlap sphere")]
        public LayerMask overlapMask;

        [Group("General")]
        [Dropdown(nameof(GetAllContainerID))]
        public string containerID;

        [Group("General")]
        [Dropdown(nameof(GetAllFactionID), Searchable = false)]
        public int factionId;

        [Group("General")]
        [Dropdown(nameof(GetAllEthnicityID), Searchable = false)]
        public string ethnicityId = "Eradian";
        [Group("General")]
        public bool countTowardsMaxAlive = true;
        [Group("General")]
        public bool hasMetal = false;
        [Group("AI")]
        [Dropdown(nameof(GetAllBrainID))]
        public string brainId;

        [Group("Tabs"), Tab("Audio")]
        [Dropdown(nameof(GetAllEffectID))]
        public string focusReadyEffectId;
        [NonSerialized, JsonIgnore]
        public EffectData focusReadyEffect;
        [Group("Tabs"), Tab("Audio")]
        [Dropdown(nameof(GetAllEffectID))]
        public string focusFullEffectId;
        [NonSerialized, JsonIgnore]
        public EffectData focusFullEffect;
        [Group("Tabs"), Tab("Audio")]
        [Dropdown(nameof(GetAllEffectID))]
        public string jumpEffectId;
        [NonSerialized, JsonIgnore]
        public EffectData jumpEffectData;
        [Group("Tabs"), Tab("Audio")]
        [Dropdown(nameof(GetAllEffectID))]
        public string kickEffectId;
        [NonSerialized, JsonIgnore]
        public EffectData kickEffectData;
        [Group("Tabs"), Tab("Audio")]
        [Dropdown(nameof(GetAllEffectID))]
        public string playerFallDamageEffectId;
        [NonSerialized, JsonIgnore]
        public EffectData playerFallDamageEffectData;
        [Group("Tabs"), Tab("Audio")]
        [DropdownList(nameof(GetAllVoiceID))]
        public List<string> voices;

        [Group("Tabs"), Tab("Customization")]
        public float randomMinHeight;
        [Group("Tabs"), Tab("Customization")]
        public float randomMaxHeight;
        [Group("Tabs"), Tab("Customization")]
        public bool adjustHeightToPlayer;
        [Group("Tabs"), Tab("Customization")]
        public float adjustHeightToPlayerDelta = 0.05f;
        [Group("Tabs"), Tab("Customization")]
        public PossesionScaling possesionScaling = PossesionScaling.ScaleCreature;

        public enum PossesionScaling
        {
            ScaleCreature,
            ScalePlayer,
        }

        [Group("Tabs"), Tab("Customization")]
        [TableList]
        public List<EthnicGroup> ethnicGroups;

        [Group("Tabs"), Tab("Customization")]
        public List<Color> hairColorsPrimaryShared;
        [Group("Tabs"), Tab("Customization")]
        public List<Color> hairColorsSecondaryShared;
        [Group("Tabs"), Tab("Customization")]
        public List<Color> hairColorsSpecularShared;
        [Group("Tabs"), Tab("Customization")]
        public List<Color> eyesColorsIrisShared;
        [Group("Tabs"), Tab("Customization")]
        public List<Color> eyesColorsScleraShared;
        [Group("Tabs"), Tab("Customization")]
        public List<Color> skinColorsShared;

        [Serializable]
        [DeclareBoxGroup("Custom Materials", Title = "Custom Materials")]
        [DeclareBoxGroup("Custom Parts", Title = "Custom Parts")]
        [DeclareBoxGroup("Colors", Title = "Colors")]
        [DeclareBoxGroup("Colors/Hair", Title = "Hair")]
        [DeclareBoxGroup("Colors/Eyes", Title = "Eyes")]
        [DeclareBoxGroup("Colors/Skin", Title = "Skin")]
        public class EthnicGroup
        {
            [JsonMergeKey]
            public string id;
            
            [Group("Custom Materials")]
            public string bodyMaterialAddressLod0;
            [Group("Custom Materials")]
            public string bodyMaterialAddressLod1;
            [Group("Custom Materials")]
            public string handsMaterialAddressLod0;
            [Group("Custom Materials")]
            public string handsMaterialAddressLod1;


            [Group("Custom Parts")]
            [DropdownList(nameof(GetAllHeadsId))]
            public List<string> allowedHeadsIDs;

            [Group("Colors/Hair")]
            public List<Color> hairColorsPrimary;
            [Group("Colors/Hair")]
            public List<Color> hairColorsSecondary;
            [Group("Colors/Hair")]
            public List<Color> hairColorsSpecular;
            [Group("Colors/Eyes")]
            public List<Color> eyesColorsIris;
            [Group("Colors/Eyes")]
            public List<Color> eyesColorsSclera;
            [Group("Colors/Skin")]
            public List<Color> skinColors;
        }

        [Group("Tabs"), Tab("Ragdoll")]
        [InlineProperty]
        public RagdollData ragdollData;

        [Serializable]
        public class RagdollData
        {
            public float standingMass = 65f;
            public float handledMass = 65f;
            public float ragdolledMass = 65f;
            public bool meshRaycast = true;
            public float destabilizedSpringRotationMultiplier = 0.5f;
            public float destabilizedDamperRotationMultiplier = 0.1f;
            public float destabilizedGroundSpringRotationMultiplier = 0.2f;
            public float fallAliveAnimationHeight = 0.5f;
            public float fallAliveDestabilizeHeight = 3;
            public float groundStabilizationMaxVelocity = 1;
            public float groundStabilizationMinDuration = 3;

            public float gripMaxCloseWeight = 0.9f;
            [Dropdown(nameof(GetAllEffectID))]
            public string gripEffectId;
            [NonSerialized, JsonIgnore]
            public EffectData gripEffectData;
            
            public float fingerSpeed = 5;

            [Dropdown(nameof(GetAllDamagerID))]
            public string bodyDefaultDamagerID;
            [NonSerialized, JsonIgnore]
            public DamagerData bodyDefaultDamagerData;

            [Dropdown(nameof(GetAllEffectID))]
            public string penetrationEffectId;
            [NonSerialized, JsonIgnore]
            public EffectData penetrationEffectData;
            public float penetrationEffectRepeatDistance = 0.05f;
            [NonSerialized, JsonIgnore]
            public bool hasPenetrationEffect;

            public List<PartData> parts = new List<PartData>();

            public TriDropdownList<string> GetAllDamagerID() => Catalog.GetDropdownAllID(Category.Damager);

            public TriDropdownList<string> GetAllItemID() => Catalog.GetDropdownAllID(Category.Item);

            public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);
        }

        [Serializable]
        public class PartData
        {
            public RagdollPart.Type bodyPartTypes;
            [Dropdown(nameof(GetAllDamagerID))]
            public string bodyDamagerID;
            [NonSerialized, JsonIgnore]
            public DamagerData bodyDamagerData;

            [Dropdown(nameof(GetAllDamagerID))]
            public string bodyAttackDamagerID;
            [NonSerialized, JsonIgnore]
            public DamagerData bodyAttackDamagerData;

            [Dropdown(nameof(GetAllEffectID))]
            public string sliceParentEffectid;
            [NonSerialized, JsonIgnore]
            public EffectData sliceParentEffectData;

            [Dropdown(nameof(GetAllEffectID))]
            public string sliceChildEffectId;
            [NonSerialized, JsonIgnore]
            public EffectData sliceChildEffectData;

            public float damageMultiplier = 1;

            public float penetrationPierceDeepDepth = 0.1f;
            public float penetrationPierceDeepDamageMultiplier = 1;

            public float penetrationSlashDamageMultiplier = 1;
            public float penetrationSkewerDamageMultiplier = 1;

            [Dropdown(nameof(GetAllEffectID))]
            public string penetrationDeepEffectId;
            [NonSerialized, JsonIgnore]
            public EffectData penetrationDeepEffectData;

            public bool sliceForceKill = true;
            public float sliceSeparationForce = 3;
            public float sliceVelocityMultiplier = 0.5f;

            public float locomotionVelocityCorrection = 130;

            public TriDropdownList<string> GetAllDamagerID() => Catalog.GetDropdownAllID(Category.Damager);

            public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);
        }


        [Group("Tabs"), Tab("Expression")]
        [Dropdown(nameof(GetAllExpressionId))]
        public string expressionAttackId;
        [NonSerialized, JsonIgnore]
        public ExpressionData expressionAttackData;
        [Group("Tabs"), Tab("Expression")]
        [Dropdown(nameof(GetAllExpressionId))]
        public string expressionPainId;
        [NonSerialized, JsonIgnore]
        public ExpressionData expressionPainData;
        [Group("Tabs"), Tab("Expression")]
        [Dropdown(nameof(GetAllExpressionId))]
        public string expressionDeathId;
        [NonSerialized, JsonIgnore]
        public ExpressionData expressionDeathData;
        [Group("Tabs"), Tab("Expression")]
        [Dropdown(nameof(GetAllExpressionId))]
        public string expressionChokeId;
        [NonSerialized, JsonIgnore]
        public ExpressionData expressionChokeData;
        [Group("Tabs"), Tab("Expression")]
        [Dropdown(nameof(GetAllExpressionId))]
        public string expressionAngryId;
        [NonSerialized, JsonIgnore]
        public ExpressionData expressionAngryData;
        [Group("Tabs"), Tab("Expression")]
        public Vector2 eyeVerticalAngleClamps = new(-10f, 15f);
        [Group("Tabs"), Tab("Expression")]
        public Vector2 eyeHorizontalAngleClamps = new(-20f, 20f);
        [Group("Tabs"), Tab("Expression")]
        public List<EyeClip> eyeClips = new();

        [Serializable]
        public class EyeClip
        {
            public string clipName = "";
            public string eyeTagFilter = "";
            public float duration = 0.33f;
            [MinMaxSlider(0f, 5f)]
            public Vector2 minMaxDelayPerEye = new(0f, 0f);
            public AnimationCurve openCurve = new(new Keyframe(0f, 1f), new Keyframe(0.5f, 0f), new Keyframe(1f, 1f));
            public bool playAutomaticallyWhileAlive = false;
            [MinMaxSlider(0f, 120f)]
            public Vector2 minMaxBetweenPlays = new(2.5f, 5f);
            [ReadOnly]
            [ShowInInspector]
            [NonSerialized, JsonIgnore]
            public bool active = false;
            [ReadOnly]
            [ShowInInspector]
            [NonSerialized, JsonIgnore]
            public float lastStartTime = -1f;
            [ReadOnly]
            [ShowInInspector]
            [NonSerialized, JsonIgnore]
            public float lastEndTime = -1f;
            [ReadOnly]
            [ShowInInspector]
            [NonSerialized, JsonIgnore]
            public float maxIndividualDelay = 0f;
            [ReadOnly]
            [ShowInInspector]
            [NonSerialized, JsonIgnore]
            public float nextPlayDelay = -1f;
            [NonSerialized, JsonIgnore]
            public Dictionary<CreatureEye, float> affectedEyes = new Dictionary<CreatureEye, float>();

            public EyeClip Clone()
            {
                EyeClip clone = new EyeClip();
                clone.clipName = clipName;
                clone.eyeTagFilter = eyeTagFilter;
                clone.duration = duration;
                clone.minMaxDelayPerEye = minMaxDelayPerEye;
                clone.openCurve = openCurve;
                clone.playAutomaticallyWhileAlive = playAutomaticallyWhileAlive;
                clone.minMaxBetweenPlays = minMaxBetweenPlays;
                return clone;
            }
        }

        public enum Gender
        {
            None,
            Male,
            Female,
        }

        [Group("Tabs"), Tab("Force")]
        public float forceMaxPosition = 3000;
        [Group("Tabs"), Tab("Force")]
        public float forceMaxRotation = 250;
        [Group("Tabs"), Tab("Force")]
        public Vector2 forcePositionSpringDamper = new(5000, 100);
        [Group("Tabs"), Tab("Force")]
        public Vector2 forceRotationSpringDamper = new(1000, 50);
        [Group("Tabs"), Tab("Force")]
        [Header("2Handed")]
        public Vector2 forcePositionSpringDamper2HMult = Vector2.one;
        [Group("Tabs"), Tab("Force")]
        public Vector2 forceSpringDamper2HNoDomMult = Vector2.one;
        [Group("Tabs"), Tab("Force")]
        public Vector2 forceRotationSpringDamper2HMult = Vector2.one;
        [Group("Tabs"), Tab("Force")]
        [Header("Climbing")]
        public float climbingForceMaxPosition = 3000;
        [Group("Tabs"), Tab("Force")]
        public float climbingForceMaxRotation = 100;
        [Group("Tabs"), Tab("Force")]
        public Vector2 climbingForcePositionSpringDamperMult = new(1, 3);
        [Group("Tabs"), Tab("Force")]
        [Header("Grip")]
        public float gripForceMaxPosition = 3000;
        [Group("Tabs"), Tab("Force")]
        public float gripForceMaxRotation = 100;
        [Group("Tabs"), Tab("Force")]
        public Vector2 gripForcePositionSpringDamperMult = new(1, 1);
        [Group("Tabs"), Tab("Force")]
        public Vector2 gripForceRotationSpringDamperMult = new(0.5f, 1);

        [Group("Tabs"), Tab("Locomotion")]
        public bool overrideGroundMask = false;
        [Group("Tabs"), Tab("Locomotion")]
        public LayerMask groundMask;
        [Group("Tabs"), Tab("Locomotion")]
        public bool destabilizeOnFall = true;
        [Group("Tabs"), Tab("Locomotion")]
        public float locomotionMass = 70f;
        [Group("Tabs"), Tab("Locomotion")]
        public float locomotionForwardSpeed = 0.2f;
        [Group("Tabs"), Tab("Locomotion")]
        public float locomotionBackwardSpeed = 0.2f;
        [Group("Tabs"), Tab("Locomotion")]
        public float locomotionStrafeSpeed = 0.2f;
        [Group("Tabs"), Tab("Locomotion")]
        public float locomotionRunSpeedAdd = 0.1f;
        [Group("Tabs"), Tab("Locomotion")]
        public float locomotionCrouchSpeed = 0.1f;
        [Group("Tabs"), Tab("Locomotion")]
        public float locomotionAirSpeed = 150f;
        [Group("Tabs"), Tab("Locomotion")]
        public float locomotionJumpForce = 0.3f;
        [Group("Tabs"), Tab("Locomotion")]
        public float locomotionJumpClimbVerticalMultiplier = 1f;
        [Group("Tabs"), Tab("Locomotion")]
        public float locomotionJumpClimbHorizontalMultiplier = 0.4f;
        [Group("Tabs"), Tab("Locomotion")]
        public float jumpClimbVerticalMaxVelocityRatio = 20f;
        [Group("Tabs"), Tab("Locomotion")]
        public float locomotionJumpMaxDuration = 0.6f;
        [Group("Tabs"), Tab("Locomotion")]
        public float locomotionGroundDrag = 3f;
        [Group("Tabs"), Tab("Locomotion")]
        public float locomotionFlyDrag = 0.3f;
        [Group("Tabs"), Tab("Locomotion")]
        public AnimationCurve playerFallDamageCurve = AnimationCurve.Linear(12, 0, 20, 200);
        [Group("Tabs"), Tab("Locomotion")]
        public AnimationCurve playerHortizontalFallDamageCurve = AnimationCurve.Linear(0, 0, 0, 0);
        [Group("Tabs"), Tab("Locomotion")]
        public float gripRecoverTime = 0.5f;

        [Group("Tabs"), Tab("Water")]
        public float waterLocomotionDragMultiplier = 3f;
        [Group("Tabs"), Tab("Water")]
        public float waterRagdollDragMultiplier = 6f;
        [Group("Tabs"), Tab("Water")]
        public AnimationCurve waterHandSpringMultiplierCurve = AnimationCurve.EaseInOut(0, 0.3f, 1, 0.1f);
        [Group("Tabs"), Tab("Water")]
        public float waterHandLocomotionVelocityCorrectionMultiplier = 0.7f;
        [Group("Tabs"), Tab("Water")]
        public AnimationCurve waterJumpForceMutiplierCurve = AnimationCurve.EaseInOut(0, 1f, 1, 0.25f);
        [Group("Tabs"), Tab("Water")]
        public float waterSwimUpForce = 10f;
        [Group("Tabs"), Tab("Water")]
        public float waterHandMovementForce = 10f;
        [Group("Tabs"), Tab("Water")]
        public float waterHandMovementMaxMagnitude = 10f;
        [Group("Tabs"), Tab("Water")]
        public AnimationCurve waterHandMovementForceCurve = AnimationCurve.EaseInOut(0, 0f, 1, 1f);
        [Group("Tabs"), Tab("Water")]
        public float waterMaxPlayerVelocityMagnitude = 3f;
        [Group("Tabs"), Tab("Water")]
        public float waterDrowningStartTime = 40f;
        [Group("Tabs"), Tab("Water")]
        public float waterDrowningWarningRatio = 0.7f;
        [Group("Tabs"), Tab("Water")]
        public float waterDrowningDamage = 10f;
        [Group("Tabs"), Tab("Water")]
        public float waterDrowningDamageInterval = 4f;

        [Group("Tabs"), Tab("Water")]
        [Dropdown(nameof(GetAllEffectID))]
        public string drownEffectId;
        [NonSerialized, JsonIgnore]
        public EffectData drownEffectData;

        protected static int index;

        #region TriInspector

        public TriDropdownList<string> GetAllHeadsId()
        {
            TriDropdownList<string> heads = new();
            foreach (CatalogData catalogData in Catalog.GetDataList(Category.Item).Where(i => i is ItemData))
            {
                var itemData = (ItemData)catalogData;
                var itemModuleWardrobe = itemData.GetModule<ItemModuleWardrobe>();

                if (itemModuleWardrobe != null && itemModuleWardrobe.category == Equipment.WardRobeCategory.Body)
                {
                    foreach (var creatureWardrobe in itemModuleWardrobe.wardrobes)
                    {
                        if (creatureWardrobe != null
                            && creatureWardrobe.manikinWardrobeData != null
                            && creatureWardrobe.manikinWardrobeData.channels.Contains("Head")
                            && creatureWardrobe.manikinWardrobeData.layers.Contains(GetLayer("Head", "Body")))
                        {
                            heads.Add(itemModuleWardrobe.itemData.id, itemModuleWardrobe.itemData.id);
                        }
                    }
                }
            }

            return heads;
        }
        public TriDropdownList<string> GetAllVoiceID() => Catalog.GetDropdownAllID(Category.Voice);

        public TriDropdownList<string> GetAllDamagerID() => Catalog.GetDropdownAllID(Category.Damager);

        public TriDropdownList<string> GetAllItemID() => Catalog.GetDropdownAllID(Category.Item);

        public TriDropdownList<string> GetAllContainerID() => Catalog.GetDropdownAllID(Category.Container);

        public TriDropdownList<string> GetAllBrainID() => Catalog.GetDropdownAllID(Category.Brain);

        public TriDropdownList<int> GetAllFactionID() => Catalog.GetDropdownAllFactionID();

        public TriDropdownList<string> GetAllEthnicityID()
        {
            //hardcode a list of IDs for now
            TriDropdownList<string> list = new();
            list.Add("Eradian", "Eradian");
            list.Add("Madene", "Madene");
            list.Add("Sentari", "Sentari");
            list.Add("Kharese", "Kharese");
            list.Add("Raike", "Raike");
            return list;
        }
        public TriDropdownList<string> GetAllSpellID() => Catalog.GetDropdownAllID<SpellData>();

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);
        public TriDropdownList<string> GetAllExpressionId() => Catalog.GetDropdownAllID(Category.Expression);
        public TriDropdownList<string> GetAllHandPoseId() => Catalog.GetDropdownAllID(Category.HandPose);

        #endregion

        public override int GetCurrentVersion()
        {
            return 6;
        }

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            expressionAttackData = Catalog.GetData<ExpressionData>(expressionAttackId);
            expressionPainData = Catalog.GetData<ExpressionData>(expressionPainId);
            expressionDeathData = Catalog.GetData<ExpressionData>(expressionDeathId);
            expressionChokeData = Catalog.GetData<ExpressionData>(expressionChokeId);
            expressionAngryData = Catalog.GetData<ExpressionData>(expressionAngryId);
            playerFallDamageEffectData = Catalog.GetData<EffectData>(playerFallDamageEffectId);
            drownEffectData = Catalog.GetData<EffectData>(drownEffectId);
            jumpEffectData = Catalog.GetData<EffectData>(jumpEffectId);
            kickEffectData = Catalog.GetData<EffectData>(kickEffectId);
            focusReadyEffect = Catalog.GetData<EffectData>(focusReadyEffectId);
            focusFullEffect = Catalog.GetData<EffectData>(focusFullEffectId);

            ragdollData ??= new RagdollData();
            ragdollData.penetrationEffectData = Catalog.GetData<EffectData>(ragdollData.penetrationEffectId);
            ragdollData.hasPenetrationEffect = ragdollData.penetrationEffectData != null;
            ragdollData.bodyDefaultDamagerData = Catalog.GetData<DamagerData>(ragdollData.bodyDefaultDamagerID);
            ragdollData.gripEffectData = Catalog.GetData<EffectData>(ragdollData.gripEffectId);
            int partsCount = ragdollData.parts.Count;
            for (var i = 0; i < partsCount; i++)
            {
                PartData partData = ragdollData.parts[i];
                partData.bodyDamagerData = Catalog.GetData<DamagerData>(partData.bodyDamagerID);
                partData.bodyAttackDamagerData = Catalog.GetData<DamagerData>(partData.bodyAttackDamagerID);
                partData.sliceChildEffectData = Catalog.GetData<EffectData>(partData.sliceChildEffectId);
                partData.sliceParentEffectData = Catalog.GetData<EffectData>(partData.sliceParentEffectid);
                partData.penetrationDeepEffectData = Catalog.GetData<EffectData>(partData.penetrationDeepEffectId);
            }
        }

        public int GetLayer(string channel, string layer)
        {
            if (manikinChannelLayers != null)
            {
                return manikinChannelLayers.GetLayer(channel, layer);
            }
            return -1;
        }

        public string GetLayerName(string channel, int layer)
        {
            if (manikinChannelLayers != null)
            {
                return manikinChannelLayers.GetLayerName(channel, layer);
            }
            return string.Empty;
        }
    }

    [Flags]
    public enum CreatureType
    {
        Human =  0b0001,
        Animal = 0b0010,
        Golem =  0b0100,
        Other =  0b1000
    }
}
