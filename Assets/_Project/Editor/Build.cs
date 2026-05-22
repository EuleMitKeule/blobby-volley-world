using System.IO;
using UnityEditor;

namespace Editor
{
    public static class Build
    {
        [MenuItem("Build/Build Client")]
        public static void BuildClient()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Builds");
            var levels = new[] { "Assets/_Project/Scenes/ClientScene.unity" };

            BuildPipeline.BuildPlayer(levels, path + "/client.exe", BuildTarget.StandaloneWindows,
                BuildOptions.None);
        }

        [MenuItem("Build/Build Linux Server")]
        public static void BuildServerLinux()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Builds");
            var levels = new[] { "Assets/_Project/Scenes/ServerScene.unity" };

            var options = new BuildPlayerOptions
            {
                scenes = levels,
                locationPathName = path + "/server.exe",
                target = BuildTarget.StandaloneLinux64,
                subtarget = (int)StandaloneBuildSubtarget.Server
            };

            BuildPipeline.BuildPlayer(options);
        }
    }
}
