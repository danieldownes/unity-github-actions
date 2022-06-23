using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

namespace Assets.Editor.BuildTools
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
        private static readonly string lumin_sdk_root = "LuminSDKRoot";

        static Builder()
        {
            // Load either localhost.json or github.json
            string path = Builder.LocalPath;
            //if (Environment.GetEnvironmentVariable("CI", EnvironmentVariableTarget.Machine) != null)
            //{
                //path = Builder.CiPath;
            //}
            //else if (! File.Exists(Builder.LocalPath))
            //{
                // Provide file, which is otherwise being ignored by .gitignore
            //    FileUtil.CopyFileOrDirectory(Builder.CiPath, Builder.LocalPath);
            //}

            // Append $GITHUB_RUN_NUMBER to the versionName
            string runNumber = ".0";
            if (Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER", EnvironmentVariableTarget.Machine) != null)
                runNumber = "." + Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER", EnvironmentVariableTarget.Machine);

            Builder.ParseConfiguration(path, runNumber);
        }

        // Architecture: 0 - None, 1 - ARM64, 2 - Universal.
        private static void ParseConfiguration(string path, string runNumber="")
        {
            const int architecture = 2;
            if (File.Exists(path))
            {
                //BuildConfig buildConfig = JsonUtility.FromJson<BuildConfig>(File.ReadAllText(path));
                
                //PlayerSettings.companyName = buildConfig.GetCompanyName();
                //PlayerSettings.productName = buildConfig.GetProductName();

                //PlayerSettings.WSA.packageVersion = new Version(buildConfig.GetVersionName() + runNumber + ".0");
            }
        }

        [MenuItem("Build/Settings")]
        public static void LogSettings()
        {
            Debug.Log("PlayerSettings.companyName: " + PlayerSettings.companyName);
            Debug.Log("PlayerSettings.productName: " + PlayerSettings.productName);
        }

        private static readonly DataReceivedEventHandler OnOutputDataReceived = new DataReceivedEventHandler((sender, e) =>
        {
            if (String.IsNullOrEmpty(e.Data) == false)
                Debug.Log(e.Data);
        });

        private static readonly DataReceivedEventHandler OnOutputErrorReceived = new DataReceivedEventHandler((sender, e) =>
        {
            if (String.IsNullOrEmpty(e.Data) == false)
                Debug.LogError(e.Data);
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
    }
#endif
}