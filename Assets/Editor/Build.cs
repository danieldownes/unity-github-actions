using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Build 
{
    [MenuItem("BUILD/Windows Build")]
    public static void Invoke()
    {
        string[] levels = new string[] {"Assets/Scenes/SampleScene.unity"};

        BuildPipeline.BuildPlayer(levels, "Build/App.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        // Copy a file from the project folder to the build folder, alongside the built game.
        //FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");
    }


    [MenuItem("BUILD/Update Version")]
    public static void UpdateVersion()
    {
        //EditorSceneManager.Open("SampleScene");

        GameObject versionObject = AssetDatabase.LoadAssetAtPath("Assets/Resources/VersionNumber.prefab",
            typeof(GameObject)) as GameObject;

        // Edit it
        versionObject.GetComponent<Text>().text = "123";

    }

    /*
     *  string runNumber = ".0";
            if (Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER", EnvironmentVariableTarget.Machine) != null)
                runNumber = "." + Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER", EnvironmentVariableTarget.Machine);
    */
}