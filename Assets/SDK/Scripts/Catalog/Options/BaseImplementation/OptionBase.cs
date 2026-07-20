using Newtonsoft.Json;
using System;
using TriInspector;

namespace ThunderRoad
{
	[Serializable]
    [SkipInTypeDropdown]
    [AutoLabelWidth(MinWidth = 0f)]
	[DeclareFoldoutGroup("Foldout", Title = "$" + nameof(inspectorName))]
    public abstract class OptionBase
	{
		[JsonMergeKey]
		[Group("Foldout")]
		public string name; //id of the option

		[JsonIgnore]
		public string inspectorName => $"{name} \"{displayName}\" ({GetType().Name})";

        [Group("Foldout")]
        [Dropdown(nameof(GetAllTextGroupId))]
        public string localizationGroup = TextData.DEFAULT_TEXT_GROUP;
        [Group("Foldout")]
        [Dropdown(nameof(GetAllTextId))]
        public string displayName;
        [Group("Foldout")]
        [Dropdown(nameof(GetAllTextId))]
        public string description;

        public virtual bool IsHidden() => false;
		public virtual bool IsLevelOption() => false;
		public abstract OptionValue DefaultValue();
		public abstract OptionValue CurrentValue();
		public abstract void SetValue(OptionValue preset);
		public virtual void SetDescription() {}
		public abstract string GetCurrentValueLabel();
		public abstract string GetDefaultValueLabel();

        public TriDropdownList<string> GetAllTextGroupId() => Catalog.GetTextData().GetDropdownAllTextGroups();

        public TriDropdownList<string> GetAllTextId() => Catalog.GetTextData().GetDropdownAllTexts(TextData.DEFAULT_TEXT_GROUP);
    }

    [Serializable]
    public abstract class OptionBase<T> : OptionBase, IEquatable<OptionBase<T>> where T : OptionValue, new()
    {
        public bool Equals(OptionBase<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return name == other.name;
		}
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((OptionBase<T>)obj);
		}
		public override int GetHashCode()
		{
			return (name != null ? name.GetHashCode() : 0);
		}
		public static bool operator ==(OptionBase<T> left, OptionBase<T> right)
		{
			return Equals(left, right);
		}
		public static bool operator !=(OptionBase<T> left, OptionBase<T> right)
		{
			return !Equals(left, right);
        }
    }
}
