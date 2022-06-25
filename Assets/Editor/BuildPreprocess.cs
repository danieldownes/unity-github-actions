using System;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class MyCustomBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("MyCustomBuildProcessor.OnPreprocessBuild for target "
            + report.summary.platform + " at path " + report.summary.outputPath);

        // Append $GITHUB_RUN_NUMBER to the versionName
        string runNumber = Application.version;
        if (Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER", EnvironmentVariableTarget.Machine) != null)
            runNumber += "." + Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER", EnvironmentVariableTarget.Machine);

        Build.UpdateVersion("CI 2 Build " + runNumber);
    }
}
