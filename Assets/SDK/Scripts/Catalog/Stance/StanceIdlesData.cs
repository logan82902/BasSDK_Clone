using System.Collections.Generic;


using TriInspector;

namespace ThunderRoad
{
    [DeclareBoxGroup("Stance basics", Title = "Stance basics")]
    public abstract class StanceIdlesData<T> : StanceData where T : IdlePose
    {
        [Group("Stance basics")]
        [HideReferencePicker]
        public List<T> idles = new();

        public override IEnumerable<StanceNode> AllStanceNodes()
        {
            for (int i = 0; i < idles.Count; i++) yield return idles[i];
        }

    }
}
