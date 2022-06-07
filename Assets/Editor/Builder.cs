using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Diagnostics;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;

namespace Stroll.Assets.Editor.BuildTools
{
#if UNITY_EDITOR
    /**
     * It loads a player configuration from JSON file, which can be editted externally.
     * @see https://docs.unity3d.com/ja/current/ScriptReference/PlayerSettings.html
     */
    [InitializeOnLoad]
    public class Builder : MonoBehaviour
    {
        private static readonly string LocalPath = Application.dataPath + "/../Scripts/localhost.json";
        private static readonly string CiPath = Application.dataPath + "/../Scripts/github.json";
        private static readonly string lumin_sdk_root = "LuminSDKRoot";

        static Builder()
        {
            // Load either localhost.json or github.json
            string path = Builder.LocalPath;
            if (Environment.GetEnvironmentVariable("CI", EnvironmentVariableTarget.Machine) != null)
            {
                path = Builder.CiPath;
            }
            else if (! File.Exists(Builder.LocalPath))
            {
                // Provide file, which is otherwise being ignored by .gitignore
                FileUtil.CopyFileOrDirectory(Builder.CiPath, Builder.LocalPath);
            }

            // Append $GITHUB_RUN_NUMBER to the versionName
            string runNumber = ".0";
            if (Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER", EnvironmentVariableTarget.Machine) != null)
            {
                runNumber = "." + Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER", EnvironmentVariableTarget.Machine);
            }

            Builder.ParseConfiguration(path, runNumber);
        }

        // Architecture: 0 - None, 1 - ARM64, 2 - Universal.
        private static void ParseConfiguration(string path, string runNumber="")
        {
            const int architecture = 2;
            if (File.Exists(path))
            {
                BuildConfig buildConfig = JsonUtility.FromJson<BuildConfig>(File.ReadAllText(path));
                // Environment.SetEnvironmentVariable("MLSDK", buildConfig.GetMlSdkPath(), EnvironmentVariableTarget.User);
                EditorPrefs.SetString("LuminSDKRoot", buildConfig.GetMlSdkPath());

                PlayerSettings.companyName = buildConfig.GetCompanyName();
                PlayerSettings.productName = buildConfig.GetProductName();

                // Apply settings for Android OS
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.X86;

                // Apply settings for Lumin OS
                PlayerSettings.SetArchitecture(BuildTargetGroup.Lumin /* 28 */, architecture);
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Lumin /* 28 */, buildConfig.GetApplicationIdentifier());
                PlayerSettings.Lumin.isChannelApp = buildConfig.GetIsChannelApp();
                PlayerSettings.Lumin.signPackage = buildConfig.GetSignPackage();
                PlayerSettings.Lumin.certificatePath = buildConfig.GetCertificatePath();
                PlayerSettings.Lumin.versionName = buildConfig.GetVersionName() + runNumber;
                PlayerSettings.Lumin.versionCode = buildConfig.GetVersionCode();

                // Apply settings for Universal Windows Platform
                PlayerSettings.SetArchitecture(BuildTargetGroup.WSA /* 14 */, architecture);
                PlayerSettings.WSA.SetTargetDeviceFamily(PlayerSettings.WSATargetFamily.Holographic /* 3 */, true);
                PlayerSettings.WSA.SetCertificate(buildConfig.GetMetroCertificatePath(), null);
                PlayerSettings.WSA.packageVersion = new Version(buildConfig.GetVersionName() + runNumber + ".0");
            }
        }

        [MenuItem("Build/Settings")]
        public static void LogSettings()
        {
            // Debug.Log("MLSDK: " + Environment.GetEnvironmentVariable("MLSDK", EnvironmentVariableTarget.User));
            Debug.Log("LuminSDKRoot: " + EditorPrefs.GetString(Builder.lumin_sdk_root, "key not found"));
            Debug.Log("PlayerSettings.companyName: " + PlayerSettings.companyName);
            Debug.Log("PlayerSettings.productName: " + PlayerSettings.productName);

            Debug.Log("PlayerSettings.WSA.applicationIdentifier: " + PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.WSA));
            Debug.Log("PlayerSettings.WSA.certificatePath: " + PlayerSettings.WSA.certificatePath);
            Debug.Log("PlayerSettings.WSA.packageVersion: " + PlayerSettings.WSA.packageVersion.ToString());

            Debug.Log("PlayerSettings.Lumin.applicationIdentifier: " + PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Lumin));
            Debug.Log("PlayerSettings.Lumin.certificatePath: " + PlayerSettings.Lumin.certificatePath);

            Debug.Log("PlayerSettings.Lumin.isChannelApp: " + PlayerSettings.Lumin.isChannelApp);
            Debug.Log("PlayerSettings.Lumin.signPackage: " + PlayerSettings.Lumin.signPackage);
            Debug.Log("PlayerSettings.Lumin.versionCode: " + PlayerSettings.Lumin.versionCode);
            Debug.Log("PlayerSettings.Lumin.versionName: " + PlayerSettings.Lumin.versionName);
        }

        private static readonly DataReceivedEventHandler OnOutputDataReceived = new DataReceivedEventHandler((sender, e) =>
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Debug.Log(e.Data);
            }
        });

        private static readonly DataReceivedEventHandler OnOutputErrorReceived = new DataReceivedEventHandler((sender, e) =>
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Debug.LogError(e.Data);
            }
        });

        private static string GetArguments()
        {
            string build_options = "/p:Platform=ARM64 /p:Configuration=Master";
            string bundle_options = "/p:AppxBundlePlatforms=ARM64 /p:AppxBundle=Always /p:UapAppxPackageBuildMode=StoreUpload";
            string signing_options = "/p:PackageCertificateKeyFile=" + Environment.CurrentDirectory + "\\BuildCert\\WSATestCertificate.pfx";
            string log_options = "/flp:logfile=" + Environment.CurrentDirectory + "\\Logs\\MSBuild.log;verbosity=diagnostic";
            return build_options + " " + bundle_options + " " + signing_options + " " + log_options;
        }

        private static void MSBuild(string solution, string arguments) {
            Debug.Log("MSBuild arguments: " + arguments);
            ProcessStartInfo process = new ProcessStartInfo
            {
                FileName = "MSBuild.exe",
                Arguments = solution + " " + arguments,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                Process msbuild = Process.Start(process);
                msbuild.OutputDataReceived += OnOutputDataReceived;
                msbuild.ErrorDataReceived += OnOutputErrorReceived;
                msbuild.EnableRaisingEvents = true;
                msbuild.BeginOutputReadLine();
                msbuild.BeginErrorReadLine();
                msbuild.WaitForExit();

                if (msbuild.ExitCode.Equals(0))
                {
                    Debug.Log("MSBuild ExitCode 0 <color=green>SUCCESS</color>");
                }
                else {
                    Debug.Log("MSBuild ExitCode " + msbuild.ExitCode + " <color=red>ERROR</color>");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("MSBuild Exception " + e.Message);
            }
        }

        private static void Explorer(string path)
        {
            if (Directory.Exists(path))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Verb = "open",
                    Arguments = path
                };
                Process.Start(startInfo);
            };
        }

        private static string[] GetScenes()
        {
            List<string> temp = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    temp.Add(scene.path);
                }
            }
            return temp.ToArray();
        }

        // https://docs.microsoft.com/en-us/dotnet/api/microsoft.mixedreality.toolkit.build.editor.builddeploywindow?view=mixed-reality-toolkit-unity-2020-dotnet-2.7.0
        [MenuItem("Build/Build APPX Package")]
        public static void BuildWSAPlayer()
        {
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = GetScenes(),
                locationPathName = "Builds\\WSAPlayer",
                options = BuildOptions.Development | BuildOptions.AllowDebugging,
                target = BuildTarget.WSAPlayer
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            Debug.Log("outputPath " + report.summary.outputPath);

            BuildSummary summary = report.summary;
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }

            String buildDir = Environment.CurrentDirectory + "/Builds/WSAPlayer";
            MSBuild(buildDir + "/CueX.sln", "/t:Restore");
            MSBuild(buildDir + "/CueX.sln", GetArguments() + " /t:Build");

            Explorer(Environment.CurrentDirectory + "\\Builds\\WSAPlayer\\build\\bin\\ARM64\\Master");
        }

        [MenuItem("Build/Build MPK Package")]
        public static void BuildLumin()
        {
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = GetScenes(),
                locationPathName = "Builds\\Lumin.mpk",
                options = BuildOptions.Development | BuildOptions.AllowDebugging,
                target = BuildTarget.Lumin
            }; 
            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }
        }
    }
#endif
}