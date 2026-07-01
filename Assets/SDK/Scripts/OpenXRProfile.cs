using System;
using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace ThunderRoad
{
    [CreateAssetMenu(menuName = "ThunderRoad/Config/OpenXRProfile")]
    public class OpenXRProfile : ScriptableObject
    {
#if UNITY_EDITOR
        [Serializable]
        public class OpenXRConfig
        {
            [ReadOnly]
            [HideLabel]
            [Title("$" + nameof(Title))]
            [GUIColor("$GetColor")]
            [NonSerialized]
            [ShowInInspector]
            [PropertyOrder(0)]
            public string info;

            private string Title => $"{featureName} | {buildTargetGroup}";

            [NonSerialized]
            [HideInInspector]
            public string featureName;

            [PropertyOrder(1)]
            [Dropdown(nameof(GetAllFeatures))]
            [OnValueChanged(nameof(OnFeatureIdChanged))]
            [HideLabel]
            public string featureId;

            [NonSerialized]
            [HideInInspector]
            public OpenXRProfile profile;

            [HideInInspector]
            public BuildTargetGroup buildTargetGroup;
            
            private Color GetColor()
            {
                if (profile != null)
                {
                    if (buildTargetGroup != profile.buildTargetGroup)
                    {
                        return Color.red; // Invalid build target group
                    }
                }
                return Color.white;
            }
            
            private void OnFeatureIdChanged() => profile.OnValidate();
            private IEnumerable<TriDropdownItem<string>> GetAllFeatures()
            {
                List<(OpenXRFeatureAttribute attribute, OpenXRFeature feature)> featuresWithAttributes = new();
                FeatureHelpers.RefreshFeatures(buildTargetGroup);
                OpenXRSettings openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(buildTargetGroup);
                if (openXRSettings == null)
                {
                    yield break;
                }
                OpenXRFeature[] features = openXRSettings.GetFeatures();
                foreach (OpenXRFeature feature in features)
                {

                    //grab the OpenXRFeatureAttribute on the feature to get its nice name
                    if (TryGetFeatureAttribute(feature, out OpenXRFeatureAttribute featureAttribute))
                    {
                        //skip hidden features
                        if (featureAttribute.Hidden)
                        {
                            continue;
                        }
                        //skip features already in the list on the build settings
                        if (this.profile.openXRFeatures.Any(f => f.featureId == featureAttribute.FeatureId && f.buildTargetGroup == buildTargetGroup))
                        {
                            continue;
                        }
                        featuresWithAttributes.Add((featureAttribute, feature));
                    }
                }
                //sort the featuresWithAttribute by the type of the feature alphabetically
                featuresWithAttributes.Sort((a, b) => string.Compare(a.feature.GetType().Name, b.feature.GetType().Name, StringComparison.Ordinal));
                foreach ((OpenXRFeatureAttribute attribute, OpenXRFeature feature) in featuresWithAttributes)
                {
                    yield return new TriDropdownItem<string>() { Text = $"{attribute.UiName} | {attribute.FeatureId}", Value = attribute.FeatureId };
                }
            }
        }

        [Group("Build Target")]
        public BuildTargetGroup buildTargetGroup = BuildTargetGroup.Standalone;

        
        [Group("OpenXR Features")]
        [ListDrawerSettings(Draggable = false, HideAddButton = true)]
        public List<OpenXRConfig> openXRFeatures = new();

        [Group("OpenXR Features")]
        [Button]
        public void AddOpenXRFeature()
        {
            OpenXRConfig newFeature = new() { buildTargetGroup = buildTargetGroup, profile = this };
            openXRFeatures.Add(newFeature);
        }


        public void SetOpenXRFeaturesAndProfiles(BuildTargetGroup activeBuildTargetGroup)
        {
            if (activeBuildTargetGroup != buildTargetGroup)
            {
                Debug.LogError($"Cannot set OpenXR features and profiles for build target group {activeBuildTargetGroup}. This build settings is for {buildTargetGroup} only.");
                return;
            }
            SetOpenXRFeaturesAndProfiles();
        }

        [Button]
        [Group("OpenXR Features")]
        public void SetOpenXRFeaturesAndProfiles()
        {
            //get all features
            FeatureHelpers.RefreshFeatures(buildTargetGroup);
            OpenXRSettings openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(buildTargetGroup);
            if (openXRSettings == null)
            {
                Debug.LogError($"No OpenXRSettings found for build target group {buildTargetGroup}");
                return;
            }
            StringBuilder sb = new StringBuilder();
            OpenXRFeature[] features = openXRSettings.GetFeatures();
            foreach (OpenXRFeature feature in features)
            {
                bool shouldEnable = false;
                //grab the OpenXRFeatureAttribute on the feature to get its nice name
                if (TryGetFeatureAttribute(feature, out OpenXRFeatureAttribute featureAttribute))
                {
                    //skip hidden features
                    if (featureAttribute.Hidden)
                    {
                        continue;
                    }
                    string id = featureAttribute.FeatureId;
                    //check if the feature is in the openXRFeatures list
                    OpenXRConfig openXRFeature = openXRFeatures.FirstOrDefault(f => f.featureId == id && f.buildTargetGroup == buildTargetGroup);
                    if (openXRFeature != null)
                    {
                        //if the feature is in the list, enable it
                        shouldEnable = true;
                    }
                }
                sb.AppendLine($"Setting feature {feature.name} to {shouldEnable}");
                feature.enabled = shouldEnable;

            }
            Debug.Log(sb.ToString());
            EditorUtility.SetDirty(openXRSettings);

        }

        public static bool TryGetFeatureAttribute(OpenXRFeature feature, out OpenXRFeatureAttribute featureAttribute)
        {
            featureAttribute = feature.GetType().GetCustomAttributes(typeof(OpenXRFeatureAttribute), false).FirstOrDefault() as OpenXRFeatureAttribute;
            return featureAttribute != null;
        }
        
        private void OnValidate()
        {
            //loop through the features and update the openXRFeatures list
            foreach (OpenXRConfig openXRFeature in openXRFeatures)
            {
                openXRFeature.profile = this; //set the build settings reference
                //check if the build target matches the current build target group
                if (openXRFeature.buildTargetGroup != BuildTargetGroup.Unknown && openXRFeature.buildTargetGroup != buildTargetGroup)
                {
                    openXRFeature.info = "Invalid build target";
                    continue; //skip if the build target group does not match
                }
                //set the build target group to the current build target group
                openXRFeature.buildTargetGroup = buildTargetGroup;
                if (string.IsNullOrEmpty(openXRFeature.featureId))
                {
                    openXRFeature.info = "No feature selected";
                    continue;
                }

                OpenXRFeature feature = FeatureHelpers.GetFeatureWithIdForBuildTarget(buildTargetGroup, openXRFeature.featureId);
                if (feature != null)
                {
                    //grab the OpenXRFeatureAttribute on the feature to get its nice name
                    if (TryGetFeatureAttribute(feature, out OpenXRFeatureAttribute featureAttribute))
                    {
                        openXRFeature.featureId = featureAttribute.FeatureId;
                        openXRFeature.info = featureAttribute.Desc;
                        openXRFeature.featureName = featureAttribute.UiName;
                    }
                }
                else
                {
                    openXRFeature.featureId = "Unknown Feature";
                    openXRFeature.info = "Feature not found";
                    openXRFeature.featureName = "Unknown Feature";
                }
            }
        }
#endif
    }
}
