#region Usings

using System.IO;
using System.Linq;
using SIM.Adapters.WebServer;
using SIM.Base;
using SIM.Instances;

#endregion

namespace SIM.Pipelines.Install
{
  #region

  

  #endregion

  /// <summary>
  ///   The update hosts.
  /// </summary>
  [UsedImplicitly]
  public class GrantPermissions : InstallProcessor
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

      var tempRootFolder = FileSystem.Local.Zip.GetFirstRootFolder(args.PackagePath);
      Assert.IsNotNull(tempRootFolder, "The single root folder within {0} archive was not found".FormatWith(args.PackagePath));
      FileSystem.Local.Directory.Ensure(tempRootFolder);

      var websitePath = Path.Combine(tempRootFolder, "Website");
      Grant(websitePath);

      var dataPath = Path.Combine(tempRootFolder, "Data");
      Grant(dataPath);
    }

    private void Grant(string path)
    {
      FileSystem.Local.Directory.Ensure(path);
      FileSystem.Local.Security.EnsurePermissions(path, Settings.CoreInstallWebServerIdentity.Value);
    }

    #endregion
  }
}