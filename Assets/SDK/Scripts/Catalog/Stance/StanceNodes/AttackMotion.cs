using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    [DeclareHorizontalGroup("StanceNode/Horiz", Sizes = new float[] { 470f, 0f })]
    [DeclareBoxGroup("StanceNode/Horiz/Attack", Title = "Attack")]
    [DeclareHorizontalGroup("StanceNode/Horiz/Attack/Motion", Sizes = new float[] { 80f, 80f, 150f, 0f, 60f, 0f, 150f, 0f })]
    [DeclareHorizontalGroup("StanceNode/Horiz/Attack/Timing")]
    [DeclareHorizontalGroup("StanceNode/Horiz/Attack/Transitions")]
    public class AttackMotion : StanceNode
    {
        // settings for the attack and all related information
        [LabelWidth(75)]
        [Group("StanceNode/Horiz/Fields/Row4")]
        public float skipChance = 0f;
        [LabelWidth(75)]
        [Group("StanceNode/Horiz/Fields/Row4")]
        public AnimationCurve rootZCurve = null;
        [Group("StanceNode/Horiz/Attack/Motion")]
        [HideLabel]
        public Interactable.HandSide attackSide = Interactable.HandSide.Right;
        [Group("StanceNode/Horiz/Attack/Motion")]
        [HideLabel]
        public ItemModuleAI.AttackType attackMotion;
        [Group("StanceNode/Horiz/Attack/Motion")]
        [LabelWidth(135)]
        public bool includeInMeleeRange = true;
        [Group("StanceNode/Horiz/Attack/Motion")]
        [LabelWidth(65)]
        public float minRange;
        [Group("StanceNode/Horiz/Attack/Motion")]
        [LabelWidth(45)]
        public bool sweep;
        [Group("StanceNode/Horiz/Attack/Motion")]
        [LabelWidth(65)]
        public float maxRange;
        [JsonProperty("range")]
        protected float range
        {
            set
            {
                minRange = value;
                maxRange = value;
            }
        }
        [Group("StanceNode/Horiz/Attack/Motion")]
        [LabelWidth(135)]
        public bool includeWeaponReach = true;
        [Group("StanceNode/Horiz/Attack/Motion")]
        [LabelWidth(80)]
        public float pushForce;
        [Group("StanceNode/Horiz/Attack/Timing")]
        [LabelWidth(40)]
        public float start;
        [Group("StanceNode/Horiz/Attack/Timing")]
        [LabelWidth(40)]
        public float peak;
        [Group("StanceNode/Horiz/Attack/Timing")]
        [LabelWidth(40)]
        public float end;
        [Group("StanceNode/Horiz/Attack/Transitions")]
        [LabelWidth(70)]
        [Dropdown(nameof(GetSourceIdleOptions))]
        public string sourceIdle = "Any";
        [Group("StanceNode/Horiz/Attack/Transitions")]
        [LabelWidth(60)]
        [Dropdown(nameof(GetEndIdleOptions))]
        public string endIdle = "Previous";
        [Group("StanceNode/Horiz/Attack/Transitions")]
        [LabelWidth(120)]
        [Dropdown(nameof(GetChainComboOptions))]
        public string comboChainAttack = "Random";

        [NonSerialized]
        public int? sortedIndex = 0;

        public override bool showDifficulty => true;

        public override bool showWeight => true;

        protected AttackMotion specificChainAttack = null;
        protected List<AttackMotion> leftChainAttacks;
        protected List<AttackMotion> bothChainAttacks;
        protected List<AttackMotion> rightChainAttacks;

        public TriDropdownList<string> GetSourceIdleOptions() => GetIdleOptions(true);

        public TriDropdownList<string> GetEndIdleOptions() => GetIdleOptions(false);

        public TriDropdownList<string> GetIdleOptions(bool source)
        {
            TriDropdownList<string> options = new() { source ? new TriDropdownItem<string>() { Text = "Any", Value = "Any" } : new TriDropdownItem<string>() { Text = "Previous", Value = "Previous" } };
            if (!source)
            {
                options.Add("RandomInc", "RandomInc");
                options.Add("RandomExc", "RandomExc");
            }
            if (stanceData is MeleeStanceData meleeStanceData) options.AddRange(GetNodeListOptions(meleeStanceData.idles));
            return options;
        }

        public TriDropdownList<string> GetChainComboOptions()
        {
            TriDropdownList<string> options = new() { new TriDropdownItem<string>() { Text = "None", Value = "None" }, new TriDropdownItem<string>() { Text = "Random", Value = "Random" } };
            if (stanceData is MeleeStanceData meleeStanceData) options.AddRange(GetNodeListOptions(meleeStanceData.chainAttacks));
            return options;
        }

        protected TriDropdownList<string> GetNodeListOptions<T>(List<T> nodes) where T : StanceNode
        {
            TriDropdownList<string> options = new();
            for (int i = 0; i < nodes.Count; i++)
            {
                options.Add(nodes[i].id, nodes[i].id);
            }
            return options;
        }

    }
}
