using System;
using UnityEditor;
using System.Diagnostics;
using UnityEngine;

public class BuildServer : EditorWindow
{
    [MenuItem("Tools/SNetworking/Create Server")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BuildServer));
    }

    [SerializeField]
    string serverSceneLocation = "Assets/Scenes/Server.unity";
    [SerializeField]
    private string lastLocation;
    [SerializeField]
    private BuildTarget buildTarget = BuildTarget.StandaloneWindows;

    void OnGUI()
    {
        GUILayout.Space(10);
        buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build Target:", buildTarget);
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Server Scene Path");
        serverSceneLocation = EditorGUILayout.TextField(serverSceneLocation);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(20);
        if (GUILayout.Button("Create Server Executable"))
        {
            Build();
        }
    }

    public void Build()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Build", lastLocation, "");
        if (String.IsNullOrEmpty(path))
        {
            return;
        }
        lastLocation = path;
        string[] levels = new string[] { serverSceneLocation };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = levels;
        buildPlayerOptions.locationPathName = path + "/Server" + GetExtension(buildTarget);
        buildPlayerOptions.target = buildTarget;
        buildPlayerOptions.options = BuildOptions.None;
        // Build player.
        BuildPipeline.BuildPlayer(buildPlayerOptions);

        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = path + "/Server" + GetExtension(buildTarget);
        proc.Start();
    }

    private string GetExtension(BuildTarget target)
    {
        if (buildTarget == BuildTarget.StandaloneWindows)
        {
            return ".exe";
        }
        return ".unkown";
    }
}
