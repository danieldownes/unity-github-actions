using UnityEditor;
using System.Diagnostics;

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
}