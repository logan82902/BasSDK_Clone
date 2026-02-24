using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine.Profiling;
using Newtonsoft.Json;

using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(0)]
#endif
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    [DeclareBoxGroup("Pooling", Title = "Pooling")]
    [DeclareBoxGroup("Cycle", Title = "Cycle")]
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("BehaviorTree", Title = "BehaviorTree")]
    public class BrainData : CatalogData
    {
        [Group("Pooling")]
        public int pooledCount;
        [Group("Cycle")]
        public bool cycle = true;
        [Group("Cycle")]
        [ShowIf("cycle", true)]
        public float cyclePlayerMinDistance = 2f;
        [Group("Cycle")]
        [ShowIf("cycle", true)]
        public float cyclePlayerMaxDistance = 20f;
        [Group("Cycle")]
        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public float cycleSpeed = 1;

        [Group("General")]
        public int difficulty = 0;
        [Group("General")]
        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public bool isActive;

        [Group("BehaviorTree")]
        [Dropdown(nameof(GetAllBehaviorTreeID))]
        public string treeID;
        [Group("BehaviorTree")]
        public bool treeDebugLog = false;
        [Group("BehaviorTree")]
        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        public bool hasTree;
        [Group("BehaviorTree")]
        public bool updateTree = true;

        [ShowInInspector]
        [SerializeReference]
        [ReadOnlyReferencePicker]
        [ListDrawerSettings(DontForceInline = true, AlwaysExpanded = true)]
        public List<Module> modules;

        [System.Serializable]
        [DeclareHorizontalGroup("UsingPartHoriz")]
        public abstract class Module
        {
            [JsonMergeKey]
            [JsonProperty("$type")]
            [JsonIgnore]
            public Type type => GetType();

            [ShowInInspector]
            [ReadOnly]
            [Group("UsingPartHoriz")]
            [LabelWidth(100)]
            [NonSerialized]
            [JsonIgnore]
            public bool useHead, useArmLeft, useArmRight, useTorso, useLegs;

            [JsonIgnore]
            public Creature creature { get; protected set; }

        }

        public TriDropdownList<string> GetAllBehaviorTreeID() => Catalog.GetDropdownAllID(Category.BehaviorTree);

        public override int GetCurrentVersion()
        {
            return 2;
        }

        public T GetModule<T>(bool autoAddModuleIfMissing = true) where T : Module, new()
        {
            
            for (int index = modules.Count - 1; index >= 0; index--)
            {
                Module brainModule = modules[index];
                if (brainModule.GetType().IsSameOrSubclassOf(typeof(T)))
                {
                             
                    return brainModule as T;
                }
            }
            if (autoAddModuleIfMissing)
            {
                T t = new T();
                modules.Add(t);
                return t;
            }
            return null;
        }
    }
}
