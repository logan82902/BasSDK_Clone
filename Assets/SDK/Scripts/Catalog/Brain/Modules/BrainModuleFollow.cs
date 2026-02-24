using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    [DeclareBoxGroup("Follow", Title = "Follow")]
    [DeclareBoxGroup("Followers", Title = "Followers")]
    public class BrainModuleFollow : BrainData.Module
    {
        public enum FollowBehavior
        {
            NeverFollow,
            OnlyWhenSet,
            SearchForFollowable
        }

        [Group("Follow")]
        public FollowBehavior followBehavior = FollowBehavior.OnlyWhenSet;
        [Group("Follow")]
        public List<Brain.State> clearFollowBrainStates = new();

        [Group("Followers")]
        public bool allowFollowers = true;
        [Group("Followers")]
        public int maxFollowers = 3;

    }
}