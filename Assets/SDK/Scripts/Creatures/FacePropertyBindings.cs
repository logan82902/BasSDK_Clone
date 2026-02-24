using System.Collections.Generic;
using UnityEngine;


using TriInspector;

namespace ThunderRoad
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "ThunderRoad/Creatures/Face Property Binding")]
    public class FacePropertyBindings : ScriptableObject
    {
        [TableList(AlwaysExpanded = true)]
        public List<FaceAnimator.SerializeableBinding> allBindables;
    }
#endif
}
