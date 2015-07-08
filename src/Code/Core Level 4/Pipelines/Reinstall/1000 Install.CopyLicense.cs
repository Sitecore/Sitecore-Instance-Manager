#region Usings

using System.IO;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Reinstall
{
  #region

  

  #endregion

  /// <summary>
  ///   The copy license.
  /// </summary>
  [UsedImplicitly]
  public class CopyLicense : ReinstallProcessor
  {
    #region Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      FileSystem.Local.File.Copy(args.LicenseFilePath, Path.Combine(args.DataFolderPath, "license.xml"));
    }

    #endregion
  }
}