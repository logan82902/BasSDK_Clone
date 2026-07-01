using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

using TriInspector;

namespace ThunderRoad
{
	[Serializable]
	[DeclareBoxGroup("Level")]
    [DeclareBoxGroup("Map")]
    [DeclareBoxGroup("Map location")]
	[DeclareBoxGroup("Cameras")]
	[DeclareBoxGroup("AI")]
    public class LevelData : CatalogData
	{
		[Group("Level")]
		public string name;
		[Group("Level")]
		[TextArea]
		[HideIf(nameof(HasSceneVariants))]
		public string description;
		[Group("Level")]
		[HideIf(nameof(HasSceneVariants))]
		public string descriptionLocalizationId;
		[Group("Level")]
		[HideIf(nameof(HasSceneVariants))]
		public string sceneAddress;
		[Group("Level")]
		public List<SceneVariant> sceneVariants;
		[NonSerialized]
		private IResourceLocation sceneLocation;
		
		public bool HasSceneVariants => !sceneVariants.IsNullOrEmpty();
		
		[Serializable]
		public class SceneVariant
		{
			public string id;
			public string name;
			public string nameLocalizationId; 
			public string description;
			public string descriptionLocalizationId;
			public string sceneAddress;
			[NonSerialized]
			public IResourceLocation sceneLocation;
		}
		
		public bool TryGetSceneVariant(string variantId, out SceneVariant sceneVariant)
		{
			sceneVariant = null;
			if (sceneVariants.IsNullOrEmpty()) return false;
			for (int i = 0; i < sceneVariants.Count; i++)
			{
				SceneVariant variant = sceneVariants[i];
				if (variant.id.Equals(variantId, StringComparison.OrdinalIgnoreCase))
				{
					sceneVariant = variant;
					return true;
				}
			}
			return false;
		}
		
		public string GetSceneNameLocIdOrDefault(string variantId = null)
		{
			if (!HasSceneVariants) return name;
			
			string val = null;
			if(TryGetSceneVariant(variantId, out SceneVariant sceneVariant))
			{
				val = sceneVariant.nameLocalizationId;
			}
			else
			{
				Debug.LogError($"Could not find scene variant {variantId}, falling back to first scene variant");
				val = sceneVariants[0].nameLocalizationId;
			}
			return val;
		}
		
		public string GetSceneNameOrDefault(string variantId = null)
		{
			if (!HasSceneVariants) return name;
			
			string val = null;
			if(TryGetSceneVariant(variantId, out SceneVariant sceneVariant))
			{
				val = sceneVariant.name;
			}
			else
			{
				Debug.LogError($"Could not find scene variant {variantId}, falling back to first scene variant");
				val = sceneVariants[0].name;
			}
			return val;
		}

		public string GetSceneDescriptionLocIdOrDefault(string variantId = null)
		{
			if (!HasSceneVariants) return descriptionLocalizationId;
			
			string val = null;
			if(TryGetSceneVariant(variantId, out SceneVariant sceneVariant))
			{
				val = sceneVariant.descriptionLocalizationId;
			}
			else
			{
				Debug.LogError($"Could not find scene variant {variantId}, falling back to first scene variant");
				val = sceneVariants[0].descriptionLocalizationId;
			}
			return val;
		}
		
		public string GetSceneDescriptionOrDefault(string variantId = null)
		{
			if (!HasSceneVariants) return description;
			
			string val = null;
			if(TryGetSceneVariant(variantId, out SceneVariant sceneVariant))
			{
				val = sceneVariant.description;
			}
			else
			{
				Debug.LogError($"Could not find scene variant {variantId}, falling back to first scene variant");
				val = sceneVariants[0].description;
			}
			return val;
		}
		
		public IResourceLocation GetSceneLocationOrDefault(string variantId = null)
		{
			if (!HasSceneVariants) return sceneLocation;
			
			IResourceLocation val = null;
			if (variantId.IsNullOrEmptyOrWhitespace())
			{
				// if no variant ID was specified, return the first (default) scene variant
				val = sceneVariants[0].sceneLocation;
			}
			else if(TryGetSceneVariant(variantId, out SceneVariant sceneVariant))
			{
				val = sceneVariant.sceneLocation;
			}
			else
			{
				val = sceneVariants[0].sceneLocation;
				Debug.LogError($"Could not find scene variant {variantId}, falling back to first scene variant: {val} - Level Id: {id}");
			}
			return val;
		}
		
		[Group("Level")]
		public bool showOnlyDevMode = false;

		[Group("Level")]
		public bool showInLevelSelection = true;

		[Group("Map")]
		public string worldMapId = "Eraden";

        [Group("Map")]
        [Tooltip("The audio container that will play when the player clicks to travel to this location from the worldmap")]
        public string worldMapTravelAudioContainerAddress = "Bas.AudioGroup.UI.LightClick";  
        [NonSerialized] public IResourceLocation worldMapTravelAudioContainerLocation;
        
		[Group("Map location")]
		public int mapLocationIndex;

		[Group("Map location")]
		public bool showOnMap = true;

		[Group("Map location")]
		public bool hideOnAndroid;

		[Group("Map location")]
		public string mapLocationIconAddress = "Bas.Icon.Location.Default";
		[NonSerialized]
		public IResourceLocation mapLocationIconLocation;
		[NonSerialized]
		public Sprite mapLocationIcon;

		[Group("Map location")]
		public string mapLocationIconHoverAddress = "Bas.Icon.Location.DefaultHover";
		[NonSerialized]
		public IResourceLocation mapLocationIconHoverLocation;
		[NonSerialized]
		public Sprite mapLocationIconHover;

		[Group("Map location")]
		public string mapPreviewImageAddress;
		[NonSerialized]
		public IResourceLocation mapPreviewImageLocation;
		[NonSerialized]
		public Sprite mapPreviewImage;

		[ShowInInspector]
		[ListDrawerSettings(AlwaysExpanded = true)]
		public List<Mode> modes;

		[Group("Cameras")]
		public List<CustomCameras> customCameras;

		[Group("AI")]
		public ThrowableReference throwableRefType = ThrowableReference.Item;
		[Group("AI")]
		public string improvisedThrowableID = null;

		public enum ThrowableReference
		{
			Item,
			Table
		}


		[Serializable]
		public struct CustomCameras
		{
			public Vector3 position;
			public Quaternion rotation;
		}

		[Serializable]
		[DeclareFoldoutGroup("Foldout", Title = "$" + nameof(name))]
		[DeclareHorizontalGroup("Foldout/Horiz")]
		public class Mode
		{
			[JsonMergeKey]
			[Group("Foldout/Horiz")]
			[AutoLabelWidth]
			public string name = "Default";
            [Group("Foldout/Horiz")]
            [AutoLabelWidth]
            public string displayName = "{Default}";
            [Group("Foldout")]
            [TextArea]
			public string description = "{NoDescription}";

			public TriDropdownList<string> GetAllGameModeID()
			{
				return Catalog.GetDropdownAllID(Category.GameMode);
			}

            [Group("Foldout")]
            [ListDrawerSettings(AlwaysExpanded = true)]
            [DropdownList(nameof(GetAllGameModeID))]
			public List<string> allowGameModes;

            [Group("Foldout")]
            public int mapOrder;

            [Group("Foldout")]
            public PlayerDeathAction playerDeathAction = PlayerDeathAction.AskReload;

            public enum PlayerDeathAction
            {
				None,
				AskReload,
				ReloadLevel,
				LoadHome,
				PermaDeath,
            }

            [ShowInInspector]
            [Group("Foldout")]
            [ReadOnlyReferencePicker]
			[ListDrawerSettings(DontForceInline = true)]
			public List<LevelModule> modules = new List<LevelModule>();
			[ShowInInspector]
            [Group("Foldout")]
            [HideReferencePicker]
            public List<OptionBase> availableOptions = new List<OptionBase>();


            public void DrawModuleGizmos()
            {
                foreach (LevelModule module in modules)
				{
					module.DrawGizmos();
				}
			}
        }

		public enum MusicType
		{
			Background,
			Combat,
		}

		public override int GetCurrentVersion()
		{
			return 3;
		}

	}
}
