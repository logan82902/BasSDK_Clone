using System.Collections.Generic;
using UnityEngine;
using System.Collections;


using TriInspector;

namespace ThunderRoad
{
#if UNITY_EDITOR
    [EditorLoadPriority(10)]
#endif
    [DeclareBoxGroup("Stance basics", Title = "Stance basics")]
    public abstract class StanceData : CatalogData
    {
        [Group("Stance basics")]
        public BrainModuleStance.Stance baseStance = BrainModuleStance.Stance.Melee1H;

        public abstract IEnumerable<StanceNode> AllStanceNodes();

    }
}
