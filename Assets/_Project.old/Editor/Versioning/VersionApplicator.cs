using System;
using UnityEditor;

namespace Blobby._Project.Editor.Versioning
{
  public class VersionApplicator
  {
    public static void SetVersion(string version)
    {
      if (version == "none") {
        return;
      }

      Apply(version);
    }

    public static void SetAndroidVersionCode(string androidVersionCode) {
      PlayerSettings.Android.bundleVersionCode = Int32.Parse(androidVersionCode);
    }

    static void Apply(string version)
    {
      PlayerSettings.bundleVersion = version;
      PlayerSettings.macOS.buildNumber = version;
    }
  }
}
