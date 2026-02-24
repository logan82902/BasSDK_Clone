using System;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
    [SkipInTypeDropdown]
    [DeclareHorizontalGroup("Horiz")]
    public abstract class ItemStat<T> : IStats<T>
    {
        [Group("Horiz")]
        [LabelWidth(40f)]
        public string name;

        public abstract T GetValue();

        protected ItemStat() { }

        protected ItemStat(string name) => this.name = name;
    }

    [Serializable]
    public class ItemStatFloat : ItemStat<float>
    {
        [Group("Horiz")]
        [LabelWidth(50f)]
        public float value;

        public ItemStatFloat() : base() { }

        public ItemStatFloat(string name, float value) : base(name) => this.value = value;

        public override float GetValue() => value;
    }

    [Serializable]
    [DeclareHorizontalGroup("Horiz", Sizes = new float[] { 0f, 0f, 105f })]
    public class ItemStatInt : ItemStat<int>
    {
        [Group("Horiz")]
        [LabelWidth(50f)]
        public int value;
        //shows with stars by default
        [Group("Horiz")]
        [LabelWidth(90f)]
        public bool useStarIcons = true;

        public ItemStatInt() : base() { }

        public ItemStatInt(string name, int value) : base(name) => this.value = value;

        public override int GetValue() => value;
    }

    [Serializable]
    public class ItemStatString : ItemStat<string>
    {
        [Group("Horiz")]
        [LabelWidth(50f)]
        public string value;

        public ItemStatString() : base() { }

        public ItemStatString(string name, string value) : base(name) => this.value = value;

        public override string GetValue() => value;
    }

    [Serializable]
    public class ItemStatBool : ItemStat<bool>
    {
        [Group("Horiz")]
        [LabelWidth(50f)]
        public bool value;

        public ItemStatBool() : base() { }

        public ItemStatBool(string name, bool value) : base(name) => this.value = value;

        public override bool GetValue() => value;
    }
}
