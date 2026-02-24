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
	[DeclareBoxGroup("Modes")]
	[DeclareBoxGroup("Cameras")]
	[DeclareBoxGroup("AI")]
    public class LevelData : CatalogData
	{
		[Group("Level")]
		public string name;
		[Group("Level")]
		[TextArea]
		public string description;
		[Group("Level")]
		public string descriptionLocalizationId;
		[Group("Level")]
		public string sceneAddress;
		[NonSerialized]
		public IResourceLocation sceneLocation;
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

		[Group("Modes"), ShowInInspector]
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
		public class Mode
		{
			[JsonMergeKey]
			public string name = "Default";
			public string displayName = "{Default}";
			[TextArea]
			public string description = "{NoDescription}";

			public TriDropdownList<string> GetAllGameModeID()
			{
				return Catalog.GetDropdownAllID(Category.GameMode);
			}

			[DropdownList(nameof(GetAllGameModeID))]
			public List<string> allowGameModes;

			public int mapOrder;

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
			public List<LevelModule> modules = new List<LevelModule>();
			[ShowInInspector]
			public List<OptionBase> availableOptions = new List<OptionBase>();

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
