using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using TriInspector;

namespace ThunderRoad
{
	public class EffectSpawner : MonoBehaviour
    {
        [Dropdown(nameof(GetAllEffectID), AppendToField = true)]
        public string effectId;

        public bool autoIntensity;
        [ShowIf("autoIntensity")]
        public AnimationCurve intensityCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));
        protected float playTime;

        public bool spawnOnStart = true;
        
        [Range(0, 1)]
        public float intensity;

        [Range(0, 1)]
        public float speed;

        public bool useMainGradient;
        [GradientUsage(true)]
        public Gradient mainGradient;

        public bool useSecondaryGradient;
        [GradientUsage(true)]
        public Gradient secondaryGradient;

        public bool useCustomParent = false;
        public Transform customParent;
        public Transform source;
        public Transform target;

        public Mesh mesh;
        public Renderer mainRenderer;
        public Renderer secondaryRenderer;
#pragma warning disable 0109 // unity this.collider is obsolete
        public new Collider collider;
#pragma warning restore 0109


        public TriDropdownList<string> GetAllEffectID() => Catalog.GetDropdownAllID(Category.Effect);

 // ProjectCore
        
        [Button]
        public void Stop()
        {
            
        }
        
        [Button]
        public void Spawn()
        {
        }
    }
}
