using System.Collections;
using UnityEngine;

using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    [AutoLabelWidth(AddedWidth = 5f, MinWidth = 0f)]
    public class BrainModuleElectrocute : BrainData.Module
    {
        public bool electrocutionImmune;

        public Vector2Int deadRotationSpringRange = new Vector2Int(10, 100);

        public int boltCount= 3;
        public float boltMinTime = 0.2f;

        public int imbueHitCount = 2;
        public float imbueHitMinTime = 2f;

    }
}