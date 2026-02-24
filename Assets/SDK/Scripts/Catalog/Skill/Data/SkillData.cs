using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;
using TriInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ThunderRoad
{
    [Serializable]
    [AutoLabelWidth(MinWidth = 0f)]
#if UNITY_EDITOR
    [EditorLoadPriority(-5)]
#endif
    [DeclareBoxGroup("Skill Tree Display", Title = "Skill Tree Display")]
    [DeclareBoxGroup("Skill Popup", Title = "Skill Popup")]
    [DeclareBoxGroup("Skill Orb", Title = "Skill Orb")]
    [DeclareBoxGroup("Skill Tutorial", Title = "Skill Tutorial")]
    [DeclareBoxGroup("Skill Cost", Title = "Skill Cost")]
    [DeclareBoxGroup("Survival")]
    public abstract class SkillData : CatalogData, IContainerLoadable<SkillData>, ILootTableDrop
    {
        // Show disabled skills at the bottom of the Odin tree. It's reversed, I don't know why.
        public override string SortKey() => (!showInTree && !isDefaultSkill ? "AAA" : "")
                                            + (8 - (isDefaultSkill ? -1 : tier))
                                            + (isTierBlocker ? 0 : 8 - (isDefaultSkill ? -1 : tier))
                                            + id;
        [Group("Skill Tree Display")]
        [Dropdown(nameof(GetAllShardsId))]
        public string shardId = "Crystal_Small_01_Shard";

        [Group("Skill Tree Display")]
        public string prefabAddress = "Bas.Item.Misc.SkillOrb";

        [Group("Skill Tree Display")]
        public string meshAddress;

        [Group("Skill Tree Display")]
        public float meshSize = 1f;

        [Group("Skill Tree Display")]
        [InfoBox("0-indexed tier. Use 0 for Tier 1, 1 for Tier 2, and 2 for Tier 3.")]
        public int tier;

        [Group("Skill Tree Display")]
        public bool allowSkill = true;

        [Group("Skill Tree Display")]
        public bool forceAllowRefund = false;

        [Group("Skill Tree Display")]
        public bool showInTree = true;

        [Group("Skill Tree Display")]
        public bool hideInSkillMenu = false;

        [Group("Skill Tree Display")]
        public bool IsCombinedSkill => !string.IsNullOrEmpty(primarySkillTreeId) && !string.IsNullOrEmpty(secondarySkillTreeId) && !primarySkillTreeId.Equals(secondarySkillTreeId);

        [Group("Skill Popup")]
        public string skillTreeDisplayName;

        public string GetName() 
        {
            return skillTreeDisplayName;
        }

#if UNITY_EDITOR
        public override string GetCatalogPath()
        {
            string treeName = GetCatalogEditorTreeName();
            string result = $"Skills/{(treeName == "" ? "No Tree Set" : treeName)}";
            if (!groupPath.IsNullOrEmptyOrWhitespace()) result += $"/{groupPath}";
            if (result[^1] == '/') result = result[..^1];
            return result;
        }

        public override string GetDataName()
        {
            return $"{(!showInTree && !isDefaultSkill ? "-- " : "")}"
                                         + $"T{(isDefaultSkill ? 0 : tier + 1)}"
                                         + $"{(isTierBlocker ? "*" : "")} {id}";
        }

        public override Texture2D Icon()
        {
            if (icon != null) return AssetPreview.GetAssetPreview(icon);
            return null;
        }
#endif

        public string EditorPrefix()
        {
            if (string.IsNullOrEmpty(primarySkillTreeId)) return "";
            string prefix = allowSkill && showInTree ? "" : "[Disabled] ";
            return prefix
                   + $"{(IsCombinedSkill ? $"X-{string.Concat((primarySkillTreeId[0].ToString() + secondarySkillTreeId[0]).OrderBy(s => s))}" : primarySkillTreeId[0].ToString())}{tier + 1} ";
        }

        [Group("Skill Popup")]
        public string description;


        [Group("Skill Popup")]
        public string imageAddress = "";

        [Group("Skill Popup")]
        public string videoAddress = "";

        [Group("Skill Popup")]
        public string buttonSpriteSheetAddress = "Bas.Ui.SkillTree.Icons";
        [Group("Skill Popup")]
        public string buttonEnabledIconAddress;

        [Group("Skill Popup")]
        public string buttonDisabledIconAddress;

        [Group("Skill Orb")]
        public string orbIconAddress = "";

#if UNITY_EDITOR
        [NonSerialized]
        public Sprite icon;
#endif

        [Group("Skill Popup")]
        private int videoCount = 0;

        [Group("Skill Cost")]
        public int costOverride = -1;

        [Group("Skill Cost")]
        [ShowInInspector]
        [JsonIgnore]
        public int Cost
        {
            get
            {
                if (isTierBlocker) return 0;
                if (costOverride >= 0) return costOverride;
                float cost = Catalog.gameData?.baseSkillCost ?? 0;
                if (!IsCombinedSkill
                    && primarySkillTreeId != null
                    && Catalog.TryGetData(primarySkillTreeId, out SkillTreeData skillTreeData))
                {
                    cost *= skillTreeData.costMultiplier;
                }

                float tierCost = Catalog.gameData?.skillCostScalingMode switch
                {
                    GameData.ScalingMode.Exponential
                        => cost * Mathf.Pow(Catalog.gameData?.skillCostMultiplierPerTier ?? 2, tier),
                    _ => cost * (tier + 1)
                };
                return Mathf.FloorToInt(tierCost);
            }
        }

        [Group("Skill Cost")]
        [FormerlySerializedAs("defaultSkill")]
        public bool isDefaultSkill = false;
        
        [NonSerialized]
        public Texture image = null;
        [NonSerialized]
        public VideoClip video = null;

        public delegate void SkillLoadedEvent(SkillData skill, Creature creature);

        public static event SkillLoadedEvent OnSkillLoadedEvent;
        public static event SkillLoadedEvent OnLateSkillsLoadedEvent;
        public static event SkillLoadedEvent OnSkillUnloadedEvent;


        [Group("Skill Tree Display")]
        [Dropdown(nameof(GetAllSkillTreeID))]
        public string primarySkillTreeId;

        [NonSerialized]
        public SkillTreeData primarySkillTree;

        [Group("Skill Tree Display")]
        [Dropdown(nameof(GetAllSkillTreeID))]
        public string secondarySkillTreeId;

        [NonSerialized]
        public SkillTreeData secondarySkillTree;

        // Only one should be blocker by Tier
        [Group("Skill Tree Display")]
        public bool isTierBlocker = false;

        [Group("Survival")]
        public bool canSpawnAsReward = false;
        [Group("Survival")]
        public bool allowInRouletteMode = true;
        [Group("Survival")]
        public bool combatSkill = false;

        [Group("Survival")]
        public bool allowOnFirstWave = true;

        [Group("Survival")]
        [DropdownList(nameof(GetAllSkillID))]
        public List<string> requiredSkills = new();

        private const bool forceInvalidateIconNames = false;
        private const bool forceInvalidateButtonNames = false;
        private const bool forceInvalidateTierBoundMesh = false;
        private const bool forceOverrideToLocalisedText = false;
        private const bool forceOverrideVideoAddress = false;


        public string RemoveTreeAffix(string treeName)
        {
            if (treeName == null) return null;
            if (treeName.EndsWith("Tree") && treeName.Length > 4)
            {
                return treeName[..^4];
            }

            return treeName;
        }

        public string GetCatalogEditorTreeName()
        {
            if (string.Compare(primarySkillTreeId, secondarySkillTreeId, StringComparison.Ordinal) < 0)
            {
                return RemoveTreeAffix(primarySkillTreeId) + secondarySkillTreeId;
            }
            return RemoveTreeAffix(secondarySkillTreeId) + primarySkillTreeId;
        }

        public bool IsOnTree(string treeId)
        {
            return primarySkillTreeId == treeId || secondarySkillTreeId == treeId;
        }
        public string GetSkillTreeKey()
        {
            return GetSkillTreeKey(primarySkillTreeId, secondarySkillTreeId);
        }

        public static string GetSkillTreeKey(string primaryTreeName, string secondaryTreeName)
        {
            //combine the two skill tree ids in alphabetical order
            if (string.Compare(primaryTreeName, secondaryTreeName, StringComparison.Ordinal) < 0)
            {
                return primaryTreeName + secondaryTreeName;
            }
            return secondaryTreeName + primaryTreeName;
        }

        public void GetVideo(Action<VideoClip> onVideoLoaded)
        {
            if(video != null)
            {
                videoCount++;
                onVideoLoaded.Invoke(video);
            }

            Catalog.LoadAssetAsync(videoAddress,
                (VideoClip clip) =>
                {
                    if (clip == null) return;
                    videoCount++;
                    video = clip;
                    onVideoLoaded.Invoke(video);
                },
                "SkillData: " + id + " loading video");
        }

        public void ReleaseVideo()
        {
            videoCount--;
            if (videoCount > 0) return;
            var clip = video;
            video = null;
            if (clip != null)
                Catalog.ReleaseAsset(clip);
        }
#if UNITY_EDITOR
        public override void CatalogEditorRefresh()
        {
            icon = Catalog.EditorLoad<Sprite>(orbIconAddress);
        }
#endif
        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();

            if (forceInvalidateIconNames || string.IsNullOrEmpty(orbIconAddress))
            {
                orbIconAddress = $"Bas.Ui.SkillTree.Icons[{id}]";
            }
            
            primarySkillTree = Catalog.GetData<SkillTreeData>(primarySkillTreeId);
            secondarySkillTree = Catalog.GetData<SkillTreeData>(secondarySkillTreeId);

            if (isTierBlocker && primarySkillTree != null)
            {
                while (primarySkillTree.tierBlockers.Count <= tier)
                    primarySkillTree.tierBlockers.Add(null);

                primarySkillTree.tierBlockers[tier] = id;
            }

            if (forceOverrideToLocalisedText)
            {
                skillTreeDisplayName = $"{{{id}Name}}";
                description = $"{{{id}Description}}";
            }

            if (forceInvalidateTierBoundMesh && isTierBlocker)
            {
                prefabAddress = "Bas.Item.Misc.SkillOrb";
                meshAddress = $"Bas.Mesh.SkillTree.TierCrystal.{primarySkillTreeId}.T{tier + 1}";
                meshSize = 0.5f;
            }

            if (forceOverrideVideoAddress)
            {
                videoAddress = $"Bas.Video.Skill.{id}";
            }

            if (IsCombinedSkill)
            {
                (string treeA, string treeB) = string.CompareOrdinal(primarySkillTreeId, secondarySkillTreeId) > 0
                    ? (secondarySkillTreeId, primarySkillTreeId)
                    : (primarySkillTreeId, secondarySkillTreeId);

                if (forceInvalidateButtonNames || string.IsNullOrEmpty(buttonEnabledIconAddress))
                    buttonEnabledIconAddress = $"{buttonSpriteSheetAddress}[{treeA}_{treeB}_ButtonColor]";

                if (forceInvalidateButtonNames || string.IsNullOrEmpty(buttonDisabledIconAddress))
                    buttonDisabledIconAddress = $"{buttonSpriteSheetAddress}[{treeA}_{treeB}_Button]";
            } else if (primarySkillTree != null)
            {
                if (forceInvalidateButtonNames || string.IsNullOrEmpty(buttonEnabledIconAddress)
                    && string.IsNullOrEmpty(primarySkillTree.iconEnabledAddress))
                    buttonEnabledIconAddress = $"{buttonSpriteSheetAddress}[{primarySkillTreeId}_ButtonColor]";

                if (forceInvalidateButtonNames || string.IsNullOrEmpty(buttonDisabledIconAddress)
                    && string.IsNullOrEmpty(primarySkillTree.iconDisabledAddress))
                    buttonDisabledIconAddress = $"{buttonSpriteSheetAddress}[{primarySkillTreeId}_Button]";
            }
        }

        public override IEnumerator OnCatalogRefreshCoroutine()
        {
            requiredSkills ??= new List<string>();

            if (primarySkillTree is null) yield break;
            if (IsCombinedSkill)
            {
                // Known issue: If a modder implements non-blocker (i.e. orbiting or minor) dual-tree skills,
                //  they will not automatically require the central dual tree skill.
                requiredSkills.Add(primarySkillTree.tierBlockers[tier]);
                requiredSkills.Add(secondarySkillTree.tierBlockers[tier]);
            }
            else if (isTierBlocker && tier > 0)
            {
                requiredSkills.Add(primarySkillTree.tierBlockers[tier - 1]);
            }
            else if (!isTierBlocker)
            {
                requiredSkills.Add(primarySkillTree.tierBlockers[tier]);
            }
        }

        public void GetOrbIcon(Action<Sprite> callback)
        {
            Catalog.LoadAssetAsync(orbIconAddress, callback, id);
        }

        public void GetButtonIcon(bool enabled, Action<Sprite> callback)
        {
            string address = enabled ? buttonEnabledIconAddress : buttonDisabledIconAddress;
            if (string.IsNullOrEmpty(address) && !IsCombinedSkill)
                address = enabled ? primarySkillTree.iconEnabledAddress : primarySkillTree.iconDisabledAddress;
            Catalog.LoadAssetAsync(address, callback, id);
        }

        /// <summary>
        /// Called when the skill is loaded on a creature, the creature is the one who has the skill
        /// </summary>
        /// <param name="skillData"></param>
        /// <param name="creature"></param>
        public virtual void OnSkillLoaded(SkillData skillData, Creature creature)
        {
            OnSkillLoadedEvent?.Invoke(skillData, creature);
        }

        /// <summary>
        /// Called when the skill is unloaded from a creature, the creature is the one who has the skill
        /// </summary>
        /// <param name="skillData"></param>
        /// <param name="creature"></param>
        public virtual void OnSkillUnloaded(SkillData skillData, Creature creature)
        {
            OnSkillUnloadedEvent?.Invoke(skillData, creature);
        }

        public virtual void OnLateSkillsLoaded(SkillData skillData, Creature creature)
        {
            OnLateSkillsLoadedEvent?.Invoke(skillData, creature);
        }

        public void OnLoadedFromContainer(Container container)
        {
            throw new NotImplementedException();
        }
        
        public virtual ContainerContent InstanceContent()
        {
	        return new SkillContent(this);
        }


        public TriDropdownList<string> GetAllHandPoseID() => Catalog.GetDropdownAllID(Category.HandPose);

        public TriDropdownList<string> GetAllStatusEffectID() => Catalog.GetDropdownAllID(Category.Status);

        public TriDropdownList<string> GetAllSpellID() => Catalog.GetDropdownAllID<SpellData>();

        public TriDropdownList<string> GetAllShardsId() => Catalog.GetDropdownAllID(Category.Item);

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

        public TriDropdownList<string> GetAllSkillID() => Catalog.GetDropdownAllID(Category.Skill);

        public TriDropdownList<string> GetAllItemID() => Catalog.GetDropdownAllID(Category.Item);

        public TriDropdownList<string> GetAllSkillTreeID() => Catalog.GetDropdownAllID(Category.SkillTree);

        public TriDropdownList<string> GetAllDamagerID() => Catalog.GetDropdownAllID(Category.Damager);

        public TriDropdownList<string> GetAllMaterialID() => Catalog.GetDropdownAllID(Category.Material);
    }
}
