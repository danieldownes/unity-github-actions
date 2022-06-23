using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Build 
{
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