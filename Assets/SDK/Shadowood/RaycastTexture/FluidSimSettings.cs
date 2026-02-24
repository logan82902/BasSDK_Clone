#if FLUXY
using System;
using Fluxy;
using TriInspector;

using UnityEngine;

[Serializable]
[DeclareFoldoutGroup("NormalMap Distort", Expanded = false)]
public class FluidSimSettings
{
    [Tooltip("Enable this while playing with settings")]
    [GUIColor("$RealtimeUpdateGuiColor")]
    public bool realtimeUpdate;

    private Color RealtimeUpdateGuiColor => realtimeUpdate ? Color.red : Color.white;

    public float gravity = -25;

    public Vector3 constantForce = Vector3.zero;

    [Range(0, 1)] public float viscosity = 0.937f;

    [Tooltip("How strong the shoreline will slow down velocity/cause water collision, value of 1 will stop the water dead still where the shore is near")] [UnityEngine.Range(0, 1)]
    public float shoreCollision = 0.6f;

    [Tooltip("Introduces foam where the shore edge is, water will pick this up and flow it down stream")] [UnityEngine.Range(0, 1)]
    public float foamDensity = 1;

    //

    [Group("NormalMap Distort")]
    public bool useNormalMap = false;


    
    [Tooltip("Normal map to add swirly, follows the UV of the mesh")] //
    [ShowIf("useNormalMap")]
    [Group("NormalMap Distort")]
    [InlineButton(nameof(FindTextures), "Default")]
    public Texture2D surfaceNormals;

    [Group("NormalMap Distort")]
    [ShowIf("useNormalMap")] //
    public Vector2 normalTiling = Vector2.one;

    [Group("NormalMap Distort")]
    [Tooltip("if set to 1 will overpower/replace other effects like gravity")] //
    [ShowIf("useNormalMap")]
    [Range(0, 2)]
    public float normalStrength = 0.5f;
    
    [HideInInspector]
    public bool setup; // TODO remove

    //public enum eFluidMode
    //{
    //    oldSplat,
    //    new
    //}

    //
    
    /// <summary>
    /// Pull settings from existing fluxy components
    /// </summary>
    public FluidSimSettings(FluxyContainer fluxyContainer, FluxyTarget fluxyTarget)
    {
        if (fluxyContainer.surfaceNormals != null && fluxyContainer.normalScale > 0) useNormalMap = true;
        surfaceNormals = fluxyContainer.surfaceNormals;
        normalStrength = fluxyContainer.normalScale;
        normalTiling = fluxyContainer.normalTiling;

        gravity = fluxyContainer.gravity.y;

        constantForce = fluxyContainer.externalForce;
        viscosity = fluxyContainer.viscosity;

        shoreCollision = fluxyTarget.velocityWeight;
        foamDensity = fluxyTarget.densityWeight;

        FindTextures();
    }
    
    public void FindTextures()
    {
        Debug.Log("FluidSimSettings: FindTextures");
        const string normalName = "FluidSimDistortion";
        surfaceNormals = Resources.Load<Texture2D>(normalName);
    }

    public void ApplySettings(FluxyContainer fluxyContainer, FluxyTarget fluxyTarget)
    {
        if (useNormalMap)
        {
            fluxyContainer.surfaceNormals = surfaceNormals;
            fluxyContainer.normalScale = normalStrength;
            fluxyContainer.normalTiling = normalTiling;
        }
        else
        {
            fluxyContainer.normalScale = 0;
        }

        fluxyContainer.gravity = new Vector3(0, gravity, 0);
        fluxyContainer.externalForce = constantForce;
        fluxyContainer.viscosity = viscosity;

        fluxyTarget.velocityWeight = shoreCollision;
        fluxyTarget.densityWeight = foamDensity;

        //in editor we can update in realtime for testing purposes, but in build this should always be disabled
#if UNITY_EDITOR
        var enabled = Application.isPlaying ? false : realtimeUpdate;
        fluxyContainer.GetComponent<FluxySolver>().enabled = enabled;
        if (enabled)
        {
            Debug.LogWarning("FluidSim Realtime Solver Update Enabled. Performance may be affected in editor.");
        }
#else
        fluxyContainer.GetComponent<FluxySolver>().enabled = false;
#endif
    }
}


#endif