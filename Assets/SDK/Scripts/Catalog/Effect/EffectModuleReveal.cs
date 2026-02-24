using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad.Reveal;


using TriInspector;

namespace ThunderRoad
{
    [DeclareBoxGroup("Reveal", Title = "Reveal")]
    public class EffectModuleReveal : EffectModule
    {
        [Group("Reveal")]
        public string maskTextureContainerAddress;
        [NonSerialized]
        public TextureContainer textureContainer;
        

        [Group("Reveal")]
        public EffectReveal.Direction applyOn = EffectReveal.Direction.Target;

        [Group("Reveal")]
        public TypeFilter typeFilter = TypeFilter.Default | TypeFilter.Body | TypeFilter.Outfit;

        [Flags]
        public enum TypeFilter
        {
            Default = 1,
            Body = 2,
            Outfit = 4,
        }

        [Group("Reveal")]
        public float depth = 1.2f;

        [Group("Reveal")]
        public float minSize = 0.05f;
        [Group("Reveal")]
        public float maxSize = 0.1f;
        [Group("Reveal")]
        public float penetrationSize = 0.1f;

        [Group("Reveal")]
        public Vector4 minChannelMultiplier = Vector4.one;
        [Group("Reveal")]
        public Vector4 maxChannelMultiplier = Vector4.one;
        [Group("Reveal")]
        public Vector4 penetrationChannelMultiplier = Vector4.one;

        [Group("Reveal")]
        public float offsetDistance = 0.05f;
        [Group("Reveal")]
        public bool allowItem = false;
        [Group("Reveal")]
        public bool allowRagdollPart = false;
        [Group("Reveal")]
        public RevealData[] revealData;

        public static List<EffectReveal> pool = new List<EffectReveal>();
        public static Transform poolRoot;



    }
}