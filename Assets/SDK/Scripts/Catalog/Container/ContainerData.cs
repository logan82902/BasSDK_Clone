using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using TriInspector;

namespace ThunderRoad
{

    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(-5)]
#endif
    [DeclareBoxGroup("Container", Title = "Container")]
    [DeclareFoldoutGroup("Container/Debug", Title = "Debug")]
    public class ContainerData : CatalogData
    {
        [Group("Container")]
        public string displayName = "Unknown";
        [Group("Container")]
        [Multiline]
        public string description;

        [ShowInInspector]
        [Group("Container")]
        [HideReferencePicker]
        [ListDrawerSettings(AlwaysExpanded = true)]
        public List<ContainerContent> containerContents = new();

        [NonSerialized]
        [ShowInInspector]
        [Group("Container/Debug")]
        [ListDrawerSettings(AlwaysExpanded = true)]
        [InlineButton(nameof(TestGeneration))]
        public List<string> generationOutput;

        public void TestGeneration()
        {
            generationOutput = new List<string>();
            foreach (ContainerContent ct in containerContents)
            {
                generationOutput.Add(ct.GetOutput());
            }
        }


        public override int GetCurrentVersion()
        {
            return 1;
        }

        public override void OnCatalogRefresh()
        {
            base.OnCatalogRefresh();
            CleanContainerContents(Application.isPlaying);
        }

        public void CleanContainerContents(bool clearEmptyOrNullContents)
        {
            if (!containerContents.IsNullOrEmpty())
            {
                for (var i = containerContents.Count - 1; i >= 0; i--)
                {
                    if (clearEmptyOrNullContents && containerContents[i].referenceID.IsNullOrEmptyOrWhitespace())
                    {
                        Debug.LogWarning($"Null ID found on {containerContents[i].GetType().Name} ({containerContents[i].mergeKey}) in container {id}, removing from container");
                        containerContents.RemoveAt(i);
                        continue;
                    }
                    if (containerContents[i] is SkillContent { data: SpellData spell })
                        containerContents[i] = new SpellContent(spell);
                    containerContents[i].OnCatalogRefresh();
                }
            }
        }

        public override CatalogData Clone()
        {
            ContainerData clone = MemberwiseClone() as ContainerData;
            clone.containerContents = clone.containerContents.Select(item => item.Clone()).ToList();
            return clone;
        }

    }

    [AutoLabelWidth]
    [Serializable]
    public abstract class ContentCustomData { }

    /// <summary>
    /// Used to store liquid contents and maximum level
    /// </summary>
    [System.Serializable]
    public class ContentCustomDataLiquidContainer : ContentCustomData
    {
        public List<LiquidData.Content> LiquidContents;

        public ContentCustomDataLiquidContainer() => LiquidContents = new List<LiquidData.Content>();

    }

    /// <summary>
    /// Used to save animator parameter state
    /// </summary>
    [System.Serializable]
    public class ContentCustomDataAnimatorParams : ContentCustomData
    {
        [System.Serializable]
        public class AnimatorParam
        {
            public string name;
            public AnimatorControllerParameterType type = AnimatorControllerParameterType.Bool;
            public bool boolVal;
            public int intVal;
            public float floatVal;

            public AnimatorParam() { }

            public AnimatorParam(Animator animator, AnimatorControllerParameter param)
            {
                name = param.name;
                type = param.type;
                switch (param.type)
                {
                    case AnimatorControllerParameterType.Bool:
                    boolVal = animator.GetBool(param.nameHash);
                    break;
                    case AnimatorControllerParameterType.Int:
                    intVal = animator.GetInteger(param.nameHash);
                    break;
                    case AnimatorControllerParameterType.Float:
                    floatVal = animator.GetFloat(param.nameHash);
                    break;
                }
            }
        }

        public List<AnimatorParam> savedParams = new List<AnimatorParam>();

        public ContentCustomDataAnimatorParams()
        {
            savedParams = new List<AnimatorParam>();
        }

    }

    [System.Serializable]
    public class ContentCustomDataValueOverride : ContentCustomData
    {
        public string valueType = null;
        public float value = -1f;

        public ContentCustomDataValueOverride() { }

    }

    /// <summary>
    /// Allows you to store an order value for the content, useful for sorting
    /// </summary>
    [System.Serializable]
    public class ContentCustomOrder : ContentCustomData
    {
        public int order = 0;

        public ContentCustomOrder() { }

    }

    [AutoLabelWidth]
    [System.Serializable]
    public abstract class ContentState
    {
        /// <summary>
        /// Returns a cloned instance of the ContentState.
        /// It is recommended to implment this method and avoid CloneJson and instead do a deep copy of the object yourself where possible.
        /// </summary>
        /// <returns></returns>
        public virtual ContentState Clone()
        {
            return this.CloneJson();
        }
    }

    [System.Serializable]
    public class ContentStateWorn : ContentState
    {
        public override ContentState Clone()
        {
            return new ContentStateWorn();
        }
    }

    [System.Serializable]
    public class ContentStateHolder : ContentState
    {
        public string holderName;

        public ContentStateHolder() { }

        public ContentStateHolder(string holderName)
        {
            this.holderName = holderName;
        }
        public override ContentState Clone()
        {
            return new ContentStateHolder(holderName);
        }
    }

    [System.Serializable]
    public class ContentStatePlaced : ContentState
    {
        public Vector3 position;
        public Quaternion rotation;
        public bool kinematic;
        public string levelId;
        public float lastSpawnTime;

        public ContentStatePlaced() { }


        public override ContentState Clone()
        {
            return new ContentStatePlaced()
            {
                position = position,
                rotation = rotation,
                kinematic = kinematic,
                levelId = levelId,
                lastSpawnTime = lastSpawnTime
            };
        }
    }

    /// <summary>
    /// A class to set the state on an item so it wont appear in the inventory UI
    /// </summary>
    [System.Serializable]
    public class ContentStateIgnoredByInventory : ContentState
    {
        public override ContentState Clone()
        {
            return new ContentStateIgnoredByInventory();
        }
    }

}
