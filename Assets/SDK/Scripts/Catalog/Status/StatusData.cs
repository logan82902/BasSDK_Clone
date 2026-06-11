using System;
using System.Collections.Generic;
using System.Linq;

using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    public enum StackType
    {
        Refresh,
        Stack,
        None,
        Infinite
    }

#if UNITY_EDITOR
    [EditorLoadPriority(10)]
#endif
    [DeclareBoxGroup("Status", Title = "Status")]
    [DeclareBoxGroup("Conditions", Title = "Conditions")]
    [DeclareBoxGroup("Effect", Title = "Effect")]
    public class StatusData : CatalogData
    {
        [ShowInInspector]
        [Group("Status")]
        [Dropdown(nameof(GetStatusTypes))]
        public Type type = typeof(Burning);

        [Group("Status")]
        public CreatureType bypassImmunity;

        [Group("Status"), InfoBox("If true, this status will be ignored by ThunderEntity.ClearByType().")]
        public bool ignoreClearByType;

        [Group("Status")]
        [InfoBox("$" + nameof(StackTypeDescription), TriMessageType.None)]
        public StackType stackType;
        
        public string StackTypeDescription => stackType switch
        {
            StackType.Refresh => "Refreshes the duration every time you apply a status.",
            StackType.Stack => "Status duration will accumulate with every application.",
            StackType.None => "Does not stack. Will always last for the duration of the first stack applied.",
            StackType.Infinite => "Never automatically expires.",
            _ => throw new ArgumentOutOfRangeException($"StackType out of range: {stackType}")
        };

        [Group("Conditions")]
        public bool forceAllowOnCreatures = false;

        [Group("Conditions")]
        public bool allowOnItems = true;


        [Group("Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string creatureEffectId;

        [NonSerialized]
        public EffectData creatureEffectData;

        [Group("Effect")]
        [Dropdown(nameof(GetAllEffectID))]
        public string itemEffectId;

        [NonSerialized]
        protected EffectData itemEffectData;
        
        public delegate void StatusEvent(Status status);
        public event StatusEvent OnFirstApplyEvent;
        public event StatusEvent OnFullRemoveEvent;

        public void InvokeOnFirstApplyEvent(Status status) => OnFirstApplyEvent?.Invoke(status);

        public void InvokeOnFullRemoveEvent(Status status) => OnFullRemoveEvent?.Invoke(status);

        public TriDropdownList<Type> GetStatusTypes()
        {
            TriDropdownList<Type> dropdownList = new();

            foreach (Type statusType in AppDomain.CurrentDomain
                         .GetAssemblies()
                         .SelectMany(assembly => assembly.GetTypes())
                         .Where(type => typeof(IStatus).IsAssignableFrom(type)))
            {
                dropdownList.Add(new TriDropdownItem<Type>() { Text = statusType.ToString(), Value = statusType });
            }

            return dropdownList;
        }

        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);
        public TriDropdownList<string> GetAllAnimationID() => Catalog.GetDropdownAllID(Category.Animation);

    }

    [Serializable]
    public class StatusEntry
    {
        public string id;
        public float duration;
        public float parameter;
        public StatusEntry() { }

        public StatusEntry(string id, float duration, float parameter)
        {
            this.id = id;
            this.duration = duration;
            this.parameter = parameter;
        }

        public static implicit operator StatusEntry((string id, float duration, float? parameter) data)
            => new(id: data.id, duration: data.duration, parameter: data.parameter ?? -1);

        public static implicit operator StatusEntry((string id, float duration) data)
            => new(id: data.id, duration: data.duration, parameter: -1);

        public static implicit operator StatusEntry(string id)
            => new(id: id, duration: Mathf.Infinity, parameter: -1);

        public void Deconstruct(out string id, out float duration, out float? parameter)
        {
            id = this.id;
            duration = this.duration;
            parameter = this.parameter == -1 ? null : this.parameter;
        }
    }
}
