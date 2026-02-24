using System;
using System.Collections.Generic;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    public class MusicDynamicModuleMap : UnitySerializedDictionary<string, MusicDynamicModule> { }

    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(10)]
#endif
    [DeclareBoxGroup("Music", Title = "Music")]
    public class Music : CatalogData
    {
        [Serializable]
        [AutoLabelWidth]
        [DeclareBoxGroup("Transition", Title = "$" + nameof(transitionName))]
        [DeclareHorizontalGroup("Transition/Horiz")]
        [DeclareBoxGroup("Transition/Horiz/TransitionIn", Title = "TransitionIn")]
        [DeclareBoxGroup("Transition/Horiz/PreTransition", Title = "PreTransition")]
        [DeclareBoxGroup("Transition/Horiz/TransitionOut", Title = "TransitionOut")]
        public class MusicTransition
        {
            public enum TransitionType
            {
                OnBeat,
                OnBar,
                Immediate,
                OnGrid
            }

            private string sourceName => sourceGroup.IsNullOrEmptyOrWhitespace() ? "None" : sourceGroup;
            private string destinationName => destinationGroup.IsNullOrEmptyOrWhitespace() ? "None" : destinationGroup;
            private string transitionName => $"{sourceName} -> {destinationName}";

            [Group("Transition")]
            [Dropdown(nameof(GetAllMusicGroupID))]
            public string sourceGroup;

            [Group("Transition")]
            [Dropdown(nameof(GetAllMusicGroupID))]
            public string destinationGroup;

            [Group("Transition")]
            [ShowInInspector]
            [Dropdown(nameof(GetAllMusicGroupID))]
            public string musicGroup;

            [Group("Transition/Horiz/TransitionIn")]
            // Number of beat/bar/grid  to wait  to start transition (0 mean on next beat/bar/grid)
            public int timeBeforeTransition;

            [Group("Transition/Horiz/TransitionIn")]
            // Type to define multiplier with timeBeforeTransition
            public TransitionType transitionType;

            [Group("Transition/Horiz/PreTransition")]
            // Number of beat/bar/grid to start pre-transition. (before transition time) In  this  timing both source music and transition will be play during this time
            public int timePreTransition;

            [Group("Transition/Horiz/PreTransition")]
            // Type to define multiplier with timePreTransition
            public TransitionType preTransitionType = TransitionType.Immediate;

            [Group("Transition/Horiz/TransitionOut")]
            // Number of beat/bar/grid to wait after transition time to start destination music (0 mean on next beat/bar/grid)
            public int timeBeforeDestStart;

            [Group("Transition/Horiz/TransitionOut")]
            // Type to define multiplier with timeBeforeDestStart
            public TransitionType transitionDestStartType;

            public TriDropdownList<string> GetAllMusicGroupID() => Catalog.GetDropdownAllID(Category.MusicGroup);
        }

        [NonSerialized]
        public List<MusicGroup> loadedMusicGroup;

        public float volumeDb = 0.0f;

        [Group("Music")]
        [ListDrawerSettings(AlwaysExpanded = true)]
        [DropdownList(nameof(GetAllMusicGroupID))]
        public List<string> groupsToLoad;

        [Group("Music")]
        [HideReferencePicker]
        [ListDrawerSettings(AlwaysExpanded = true)]
        public List<MusicTransition> transitions;

        [ShowInInspector]
        [InlineProperty]
        [LabelWidth(0.0001f)]
        public MusicDynamicModuleMap dynamicModules = new();

        public TriDropdownList<string> GetAllMusicGroupID() => Catalog.GetDropdownAllID(Category.MusicGroup);

    }
}