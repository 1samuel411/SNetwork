#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using SNetwork;
using SNetwork.Client;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class ClientSettings : EditorWindow
{

    [MenuItem("Tools/SNetworking/Client Settings")]
    public static void Init()
    {
        ClientSettingsScriptableObject asset = null;
        asset = AssetDatabase.LoadAssetAtPath<ClientSettingsScriptableObject>(ClientSettingsScriptableObject.resourcesLocation);

        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<ClientSettingsScriptableObject>();
            AssetDatabase.CreateAsset(asset, ClientSettingsScriptableObject.resourcesLocation);
            AssetDatabase.SaveAssets();
        }

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
#endif