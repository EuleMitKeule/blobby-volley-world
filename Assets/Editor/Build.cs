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
            var levels = new[] {"Assets/_Project/Scenes/ClientScene.unity"};

            BuildPipeline.BuildPlayer(levels, path + "/client.exe", BuildTarget.StandaloneWindows,
                BuildOptions.None);
        }

        [MenuItem("Build/Build Linux Server")]
        public static void BuildServerLinux()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Builds");
            var levels = new[] {"Assets/_Project/Scenes/ServerScene.unity"};

            BuildPipeline.BuildPlayer(levels, path + "/server.exe", BuildTarget.StandaloneLinux64,
                BuildOptions.EnableHeadlessMode);
        }
    }
}