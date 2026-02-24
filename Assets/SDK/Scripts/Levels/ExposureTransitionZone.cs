using UnityEngine;
using UnityEditor;


using TriInspector;

namespace ThunderRoad
{
    [HelpURL("https://kospy.github.io/BasSDK/Components/ThunderRoad/ExposureTransitionZone")]
    [RequireComponent(typeof(BoxCollider))]
    public class ExposureTransitionZone : ExposureSetterZone
    {

        [Tooltip("The exposure to set when the player leaves zone via red portal.")]
        public float exposureOnExit = 0f;
        
    }
}
