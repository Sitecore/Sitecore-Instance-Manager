using JetBrains.Annotations;

namespace SIM.FileSystem
{
  public class FileSystem
  {
    #region Fields

    [NotNull]
    public static FileSystem Local { get; } = new FileSystem();

    [NotNull]
    public DirectoryProvider Directory { get; }

    [NotNull]
    public FileProvider File { get; }

    [NotNull]
    public SecurityProvider Security { get; }

    [NotNull]
    public ZipProvider Zip { get; }

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
      Directory = new DirectoryProvider(this);
      File = new FileProvider(this);
      Zip = new ZipProvider(this);
      Security = new SecurityProvider(this);
    }

    #endregion
  }
}