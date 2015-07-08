namespace SIM.Base
{
  using System.IO;
  using System.Security.AccessControl;
  using System.Security.Principal;

  public class UncSecurityProvider : SecurityProvider
  {
    public UncSecurityProvider([NotNull] FileSystem fileSystem)
      : base(fileSystem)
    {
      Assert.ArgumentNotNull(fileSystem, "fileSystem");
    }

    /// <summary>
    /// The ensure permissions.
    /// </summary>
    /// <param name="path">
    /// The path. 
    /// </param>
    /// <param name="identity">
    /// The identity. 
    /// </param>
    protected override void EnsureDirectoryPermissions(string path, IdentityReference identity)
    {
      try
      {
        base.EnsureDirectoryPermissions(path, identity);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        base.EnsureDirectoryPermissions(path, identity);
      }
    }

    /// <summary>
    /// The ensure permissions.
    /// </summary>
    /// <param name="path">
    /// The path. 
    /// </param>
    /// <param name="identity">
    /// The identity. 
    /// </param>
    protected override void EnsureFilePermissions(string path, IdentityReference identity)
    {
      try
      {
        base.EnsureFilePermissions(path, identity);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        base.EnsureFilePermissions(path, identity);
      }
    }

    public override void EnsurePermissions(string path, string identity)
    {
      try
      {
        base.EnsurePermissions(path, identity);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        base.EnsurePermissions(path, identity);
      }
    }

    protected override bool HasDirectoryPermissions(string path, IdentityReference identity, FileSystemRights permissions)
    {
      try
      {
        return base.HasDirectoryPermissions(path, identity, permissions);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.HasDirectoryPermissions(path, identity, permissions);
      }
    }

    protected override bool HasFilePermissions(string path, IdentityReference identity, FileSystemRights permissions)
    {
      try
      {
        return base.HasFilePermissions(path, identity, permissions);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.HasFilePermissions(path, identity, permissions);
      }
    }

    public override bool HasPermissions(string path, string identity, FileSystemRights permissions)
    {
      try
      {
        return base.HasPermissions(path, identity, permissions);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.HasPermissions(path, identity, permissions);
      }
    }
  }
}