using UnityEngine;


using TriInspector;

namespace ThunderRoad
{
	[AddComponentMenu("ThunderRoad/Creatures/IK Controller")]
    public class IKControllerFIK : IkController
    {
        [Header("Final IK")]
        public AnimationCurve stretchArmsCurve;
        public AnimationCurve stretchLegsCurve;
        public float headTorsoBendWeightMultiplier = 0.5f;

    }
}
