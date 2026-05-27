using System.IO;
using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace UlarTangga.EditorSetup
{
    public static class WebGLBuildHelper
    {
        private const string OutputPath = "docs";

        [MenuItem("Ular Tangga/Build WebGL To Docs")]
        public static void BuildWebGLToDocs()
        {
            Directory.CreateDirectory(OutputPath);

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/MainMenuScene.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/GameScene.unity", true)
            };

            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            PlayerSettings.WebGL.decompressionFallback = false;
            PlayerSettings.WebGL.dataCaching = false;

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/MainMenuScene.unity", "Assets/Scenes/GameScene.unity" },
                locationPathName = OutputPath,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            Debug.Log($"[WebGLBuild] Result: {summary.result}, Size: {summary.totalSize} bytes, Output: {Path.GetFullPath(OutputPath)}");

            if (summary.result != BuildResult.Succeeded)
            {
                throw new Exception($"WebGL build failed with result: {summary.result}");
            }
        }
    }
}
