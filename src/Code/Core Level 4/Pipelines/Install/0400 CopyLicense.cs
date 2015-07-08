#region Usings

using System.IO;
using SIM.Adapters;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Install
{
  #region

  

  #endregion

  /// <summary>
  ///   The copy license.
  /// </summary>
  [UsedImplicitly]
  public class CopyLicense : InstallProcessor
  {
    #region Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      FileSystem.Local.File.Copy(args.LicenseFilePath, Path.Combine(args.DataFolderPath, "license.xml"));
    }

    #endregion
  }
}