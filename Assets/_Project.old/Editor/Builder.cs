using System;
using Blobby._Project.Editor.Input;
using Blobby._Project.Editor.Reporting;
using Blobby._Project.Editor.Versioning;
using UnityEditor;

namespace Blobby._Project.Editor
{
  static class Builder
  {
    public static void BuildClient()
    {
      var options = ArgumentsParser.GetValidatedOptions();
      var scenes = new[] { "Assets/_Project/Scenes/ClientScene.unity" };
      
      var buildOptions = BuildOptions.None;
      foreach (string buildOptionString in Enum.GetNames(typeof(BuildOptions))) 
      {
        if (options.ContainsKey(buildOptionString)) 
        {
          var buildOptionEnum = (BuildOptions)Enum.Parse(typeof(BuildOptions), buildOptionString);
          buildOptions |= buildOptionEnum;
        }
      }

      var buildPlayerOptions = new BuildPlayerOptions 
      {
        scenes = scenes,
        locationPathName = options["customBuildPath"],
        target = (BuildTarget)Enum.Parse(typeof(BuildTarget), options["buildTarget"]),
        options = buildOptions
      };

      VersionApplicator.SetVersion(options["buildVersion"]);
      VersionApplicator.SetAndroidVersionCode(options["androidVersionCode"]);
      
      if (buildPlayerOptions.target == BuildTarget.Android)
        AndroidSettings.Apply(options);

      var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

      var summary = buildReport.summary;
      StdOutReporter.ReportSummary(summary);

      var result = summary.result;
      StdOutReporter.ExitWithResult(result);
    }

    public static void BuildServerLinux()
    {
      var options = ArgumentsParser.GetValidatedOptions();
      var scenes = new[] { "Assets/_Project/Scenes/ServerScene.unity" };
      
      var buildOptions = BuildOptions.None;
      foreach (string buildOptionString in Enum.GetNames(typeof(BuildOptions))) 
      {
        if (options.ContainsKey(buildOptionString)) 
        {
          var buildOptionEnum = (BuildOptions)Enum.Parse(typeof(BuildOptions), buildOptionString);
          buildOptions |= buildOptionEnum;
        }
      }

      buildOptions |= BuildOptions.EnableHeadlessMode;

      var buildPlayerOptions = new BuildPlayerOptions 
      {
        scenes = scenes,
        locationPathName = options["customBuildPath"],
        target = (BuildTarget)Enum.Parse(typeof(BuildTarget), options["buildTarget"]),
        options = buildOptions
      };

      VersionApplicator.SetVersion(options["buildVersion"]);
      VersionApplicator.SetAndroidVersionCode(options["androidVersionCode"]);
      
      if (buildPlayerOptions.target == BuildTarget.Android)
        AndroidSettings.Apply(options);

      var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

      var summary = buildReport.summary;
      StdOutReporter.ReportSummary(summary);

      var result = summary.result;
      StdOutReporter.ExitWithResult(result); 
    }
  }
}
