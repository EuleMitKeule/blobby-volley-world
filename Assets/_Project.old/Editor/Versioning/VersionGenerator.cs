namespace Blobby._Project.Editor.Versioning
{
  public static class VersionGenerator
  {
    public static string Generate()
    {
      return Git.GenerateSemanticCommitVersion();
    }
  }
}
