using Sitecore.Diagnostics.Base.Annotations;

namespace SIM.FileSystem
{
  public class FileSystem
  {
    #region Fields

    [NotNull]
    public static readonly FileSystem Local = new FileSystem();

    [NotNull]
    public readonly DirectoryProvider Directory;

    [NotNull]
    public readonly FileProvider File;

    [NotNull]
    public readonly PathProvider Path;

    [NotNull]
    public readonly SecurityProvider Security;

    [NotNull]
    public readonly ZipProvider Zip;

    #endregion

    #region Constructors

    private FileSystem()
    {
      // Disabled for now as the problem is in IonicZip that doesn't work with them
      /*
      Directory = new UncDirectoryProvider(this);
      File = new UncFileProvider(this);
      Zip = new UncZipProvider(this);
      Path = new PathProvider(this);
      Security = new UncSecurityProvider(this);
       */
      this.Directory = new DirectoryProvider(this);
      this.File = new FileProvider(this);
      this.Zip = new ZipProvider(this);
      this.Path = new PathProvider(this);
      this.Security = new SecurityProvider(this);
    }

    #endregion
  }
}