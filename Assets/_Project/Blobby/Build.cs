using System.IO;
using UnityEditor;

namespace Blobby
{
    public static class Build
    {
        public static void BuildClient()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Builds");
            var levels = new[] {"Assets/Scenes/ClientScene.unity"};

            BuildPipeline.BuildPlayer(levels, path + "/client.exe", BuildTarget.StandaloneWindows,
                BuildOptions.None);
        }

        public static void BuildServerLinux()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Builds");
            var levels = new[] {"Assets/Scenes/ServerScene.unity"};

            BuildPipeline.BuildPlayer(levels, path + "/server.exe", BuildTarget.StandaloneLinux64,
                BuildOptions.EnableHeadlessMode);
        }
    }
}