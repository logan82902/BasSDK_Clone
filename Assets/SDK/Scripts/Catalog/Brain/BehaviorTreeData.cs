using System;
using System.Collections;
using UnityEngine;
using ThunderRoad.AI;


using TriInspector;

namespace ThunderRoad
{
#if UNITY_EDITOR
    [EditorLoadPriority(5)]
#endif
    [DeclareBoxGroup("General", Title = "General")]
    public class BehaviorTreeData : CatalogData
    {
        [Group("General")]
        [LabelWidth(30f)]
        public Type type = Type.Misc;

        [Group("General")]
        [ShowInInspector]
        [AutoLabelWidth(AddedWidth = 5f)]
        public Node rootNode = null;

        [NonSerialized]
        public Blackboard blackboard;

        [NonSerialized]
        public Creature creature;


        public enum Type
        {
            MasterTree,
            SubTree,
            WIP,
            Misc,
        }
    }
}