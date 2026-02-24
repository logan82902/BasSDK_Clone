using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using TriInspector;

namespace ThunderRoad.AssetSorcery
{
    [Serializable]
    public class AssetSorceryShaderSettings
    {
        [Tooltip("Rename the shader using this string")]
        public string customShaderName;

        public string outputShaderFName;

        public AssetSorceryPlatformRuntime.ePlatformAS PlatformAS = AssetSorceryPlatformRuntime.ePlatformAS.Desktop;
        
        [Tooltip("Toggles #pragma debug")]
        public bool debugShader;

        [Tooltip("Toggles the define for SW_TESSELLATION")]
        public bool tessellationDesktop;
        [Tooltip("Toggles the define for SW_TESSELLATION")]
        public bool tessellationMobile;
        
        public bool forceOpaqueDesktop; // Toggles the depth queue between OPAQUE and TRANSPARENT
        public bool forceOpaqueMobile; // Toggles the depth queue between OPAQUE and TRANSPARENT

        public int offsetQueueDesktop = 0;
        public int offsetQueueMobile = 0;

        public Shader desktopShader;
        public Shader mobileShader;
        public List<PassItem> passItems = new();

        public List<AssetSorceryShaderKeyword> shaderKeywords = new();
    }

  

    [Serializable]
    public class PassItem
    {
        [HideInInspector] public string displayName;

        public string passName;

        [HideInInspector]
        [ReadOnly]
        public bool present;

        [HideInInspector] public int indexSubShader;

        public bool enabledDesktop;
        public bool enabledMobile;

        //

        public void DisplayNameCalc()
        {
            var prefix = (present ? "" : " -- ") + (enabledDesktop ? "ON - " : "OFF - ") + (enabledMobile ? "ON - " : "OFF - ");
            displayName = prefix + passName;
        }

        public ShaderData.Subshader GetPassSubshader(Shader shader)
        {
            var shaderData = ShaderUtil.GetShaderData(shader);
            var subShader = shaderData.GetSerializedSubshader(indexSubShader);
            return subShader;
        }
    }
}
