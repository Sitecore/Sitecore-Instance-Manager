#region Usings



#endregion

namespace SIM.Base
{
  /// <summary>
  ///   The file system.
  /// </summary>
  public class FileSystem
  {
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
      Directory = new DirectoryProvider(this);
      File = new FileProvider(this);
      Zip = new ZipProvider(this);
      Path = new PathProvider(this);
      Security = new SecurityProvider(this);
    }

    #region Static Fields

    [NotNull]
    public static readonly FileSystem Local = new FileSystem();

    #endregion

    #region Fields

    [NotNull]
    public readonly DirectoryProvider Directory;

    [NotNull]
    public readonly FileProvider File;

    [NotNull]
    public readonly ZipProvider Zip;

    [NotNull]
    public readonly PathProvider Path;

    [NotNull]
    public readonly SecurityProvider Security;

    #endregion
  }
}