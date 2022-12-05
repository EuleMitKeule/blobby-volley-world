using System;

namespace Blobby._Project.Editor.Versioning
{
  public class GitException : InvalidOperationException
  {
    public readonly int code;

    public GitException(int code, string errors) : base(errors)
    {
      this.code = code;
    }
  }
}
