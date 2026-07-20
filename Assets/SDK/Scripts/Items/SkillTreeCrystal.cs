using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

using TriInspector;

namespace ThunderRoad
{
    [RequireComponent(typeof(Item))]
    public class SkillTreeCrystal : ThunderBehaviour
    {
        public RagdollHand hand;
        public HandlePose floatHandPose;

        [Header("Merge VFx")]
        public AnimationCurve glowCurve;
        public float timeGlowTransition = 1.0f;
        public float timeVfxTransition = 2.0f;
        public float timeVfxTransitionMin = 0.5f;

        [FormerlySerializedAs("mergeVfx")]
        [Group("Tabs"), Tab("Windows VFX")]
        public VisualEffect mergeVfxWindows;
        [FormerlySerializedAs("mergeVfxTarget")]
        [Group("Tabs"), Tab("Windows VFX")]
        public Transform mergeVfxTargetWindows;

        [FormerlySerializedAs("linkVfx")]
        [Group("Tabs"), Tab("Windows VFX")]
        public VisualEffect linkVfxWindows;
        [FormerlySerializedAs("linkVfxTarget")]
        [Group("Tabs"), Tab("Windows VFX")]
        public Transform linkVfxTargetWindows;
        [Group("Tabs"), Tab("Android VFX")]
        public VisualEffect mergeVfxAndroid;
        [Group("Tabs"), Tab("Android VFX")]
        public Transform mergeVfxTargetAndroid;
        [Group("Tabs"), Tab("Android VFX")]
        public VisualEffect linkVfxAndroid;
        [Group("Tabs"), Tab("Android VFX")]
        public Transform linkVfxTargetAndroid;
        
        public delegate void LinkEvent(SkillTreeCrystal crystal, SkillTreeCrystal other, bool linked);
        public static event LinkEvent OnLinkStateEvent;
        public delegate void MergeStartEvent(SkillTreeCrystal crystalA, SkillTreeCrystal crystalB);
        public static event MergeStartEvent OnMergeStart;
        public delegate void MergeEndEvent(SkillTreeCrystal crystal);
        public static event MergeEndEvent OnMergeComplete;

#if UNITY_EDITOR
        [Button]
        public void CopyVfxToAndroid()
        {
            var clone = Instantiate(linkVfxWindows.transform.parent, linkVfxWindows.transform.parent.parent);
            clone.name = "VFX_Android";

            mergeVfxAndroid = clone.Find("vfx_crystal_merge_point1").GetComponent<VisualEffect>();
            mergeVfxTargetAndroid = mergeVfxAndroid.transform.Find("Target");
            mergeVfxAndroid.visualEffectAsset = AssetDatabase.LoadAssetAtPath<VisualEffectAsset>(
                AssetDatabase.GUIDToAssetPath("8661053b521d2844b9d1683386d25e12"));
            mergeVfxAndroid.SetVector4("Color", Utils.UnHDR(mergeVfxWindows.GetVector4("Source Color")));
            
            linkVfxAndroid = clone.Find("vfx_crystal_link").GetComponent<VisualEffect>();
            linkVfxTargetAndroid = linkVfxAndroid.transform.Find("Target");
            linkVfxAndroid.visualEffectAsset = AssetDatabase.LoadAssetAtPath<VisualEffectAsset>(
                AssetDatabase.GUIDToAssetPath("a268070ac4c448a4a9ade95f605d8cda"));
            linkVfxAndroid.SetVector4("Color", Utils.UnHDR(linkVfxWindows.GetVector4("Source Color")));
        }
#endif        
        [NonSerialized]
        public VisualEffect mergeVfx;
        [NonSerialized]
        public Transform mergeVfxTarget;

        [NonSerialized]
        public VisualEffect linkVfx;
        [NonSerialized]
        public Transform linkVfxTarget;
        
        public float linkMaxDistance = 2.0f;



        [Header("Skill tree")]
        [Dropdown(nameof(GetAllSkillTreeID), AppendToField = true)]
        public string treeName;

        [ColorPickerField(Alpha = true, HDR = true)]
        public Color skillTreeEmissionColor = Color.white;


        [Header("Custom")]
        public bool overrideCrystalColors = false;

        [ColorPickerField(Alpha = true, HDR = true), ShowIf("overrideCrystalColors")]
        public Color baseColor;
        [ColorPickerField(Alpha = true, HDR = true), ShowIf("overrideCrystalColors")]
        public Color internalColor;
        [ColorPickerField(Alpha = true, HDR = true), ShowIf("overrideCrystalColors")]
        public Color animatedColor;
        [ColorPickerField(Alpha = true, HDR = true), ShowIf("overrideCrystalColors")]
        public Color emissionColor;
        [ColorPickerField(Alpha = true, HDR = true), ShowIf("overrideCrystalColors")]
        public Color linkVfxColor;
        [ColorPickerField(Alpha = true, HDR = true), ShowIf("overrideCrystalColors")]
        public Color mergeVfxColor;



        public TriDropdownList<string> GetAllSkillTreeID() => Catalog.gameData == null ? new TriDropdownList<string>() : Catalog.GetDropdownAllID(Category.SkillTree);
    }
}
