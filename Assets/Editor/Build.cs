using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Build 
{
    [MenuItem("BUILD/Windows Build")]
    public static void Invoke()
    {
        string[] levels = new string[] { "Assets/Scenes/SampleScene.unity" };

        BuildPipeline.BuildPlayer(levels, "Build/App.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        // Copy a file from the project folder to the build folder, alongside the built game.
        //FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");
    }

    [MenuItem("BUILD/Update Version")]
    public static void UpdateVersion(string version)
    {
        //EditorSceneManager.Open("SampleScene");

        GameObject versionObject = AssetDatabase.LoadAssetAtPath("Assets/Resources/VersionNumber.prefab",
            typeof(GameObject)) as GameObject;

        // Edit it
        versionObject.GetComponent<Text>().text = version;
    }

}