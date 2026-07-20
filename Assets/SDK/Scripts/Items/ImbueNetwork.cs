using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace ThunderRoad
{
    [AddComponentMenu("ThunderRoad/Items/Imbue Network")]
    [DeclareHorizontalGroup("Groups")]
    [DeclareVerticalGroup("Groups/Sources")]
    [DeclareVerticalGroup("Groups/Targets")]
    public class ImbueNetwork : ThunderBehaviour
    {
        public List<GameObject> sourceCollections = new List<GameObject>();
        public List<GameObject> targetCollections = new List<GameObject>();

    }
}
