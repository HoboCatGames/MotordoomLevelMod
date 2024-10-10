using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ModLightingSettings : MonoBehaviour
{
    [Header("Skybox")]
    public bool changeSkybox = false;
    public Material skyBoxMaterial;

    [Header("Fog")]
    public bool fog = false;
    public Color fogColor;
    public FogMode fogMode;
    public float fogDensity = 0;

    public void GetCurrentLightingSettings()
    {
        skyBoxMaterial = RenderSettings.skybox;
        fog = RenderSettings.fog;
        fogColor = RenderSettings.fogColor;
        fogMode = RenderSettings.fogMode;
        fogDensity = RenderSettings.fogDensity;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ModLightingSettings))] // Replace "YourTargetScript" with the actual name of the script containing the function you want to call.
public class ModLightingSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw the default inspector for the target script.

        ModLightingSettings targetScript = (ModLightingSettings)target;

        // Add a button to the inspector for calling the function.
        if (GUILayout.Button("Get Current Lighting Settings"))
        {
            // Call the function in the target script when the button is pressed.
            targetScript.GetCurrentLightingSettings(); // Replace "YourFunction" with the actual name of the function you want to call.
            EditorUtility.SetDirty(targetScript);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
