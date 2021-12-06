#if UNITY_EDITOR
using System;
using System.IO;

using UnityEditor;

using UnityEngine;

class ScriptedBuild
{
    // Invoked via command line only
    static void PerformHeadlessWindowsBuild()
    {
        var buildPath = "/Users/sshekhar/projects/vr/lab/Parallel/Builds/Parallel/Parallel.exe";// Path.Combine(Application.dataPath, "../Builds");

        BuildPipeline.BuildPlayer(new string[] { "Assets/Scenes/SampleScene.unity" }, buildPath, BuildTarget.StandaloneWindows64, BuildOptions.EnableHeadlessMode);
    }
}
#endif

