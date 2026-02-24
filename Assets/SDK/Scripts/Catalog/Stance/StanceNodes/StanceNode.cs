using System;
using UnityEngine;


using TriInspector;
using Newtonsoft.Json;

namespace ThunderRoad
{
    [Serializable]
    [DeclareFoldoutGroup("StanceNode", Expanded = true, Title = "$niceID")]
    [DeclareHorizontalGroup("StanceNode/Horiz")]
    [DeclareVerticalGroup("StanceNode/Horiz/Fields")]
    [DeclareHorizontalGroup("StanceNode/Horiz/Fields/Row2")]
    [DeclareHorizontalGroup("StanceNode/Horiz/Fields/Row3")]
    [DeclareHorizontalGroup("StanceNode/Horiz/Fields/Row4")]
    public abstract class StanceNode
    {
        [JsonMergeKey]
        [LabelWidth(35)]
        [EnableIf(nameof(customID))]
        [Group("StanceNode/Horiz/Fields")]
        public string id = "[New stance animation]";
        [LabelWidth(65)]
        [LabelText("Animation")]
        [Group("StanceNode/Horiz/Fields/Row2")]
        public string address;
        [NonSerialized]
        [ShowInInspector]
        [HideLabel]
        [Group("StanceNode/Horiz/Fields/Row2")]
#if UNITY_EDITOR
        [OnValueChanged(nameof(EditorUpdateAddress))]
#endif
        public AnimationClip animationClip;
        [LabelWidth(105)]
        [Group("StanceNode/Horiz/Fields/Row3")]
        public float animationSpeed = 1;
        [LabelWidth(60)]
        [Group("StanceNode/Horiz/Fields/Row3")]
        [ShowIf("showDifficulty")]
        public int difficulty;
        [LabelWidth(130)]
        [Group("StanceNode/Horiz/Fields/Row3")]
        public bool allowPlayAndMove = true;
        [LabelWidth(40)]
        [Group("StanceNode/Horiz/Fields/Row4")]
        [ShowIf("showWeight")]
        public float weight;

        [NonSerialized]
        public StanceData stanceData;
        [NonSerialized]
        public bool populated = false;

        protected bool isPlaying => Application.isPlaying;

        public virtual bool customID => true;

        public virtual bool showDifficulty => false;

        public virtual bool showWeight => false;

        [JsonIgnore]
        protected string niceID => Utils.AddSpacesToSentence(id, true);

#if UNITY_EDITOR
        protected void EditorUpdateAddress() => address = animationClip ? Catalog.GetAddressFromPrefab(animationClip) : "";
#endif

    }
}
