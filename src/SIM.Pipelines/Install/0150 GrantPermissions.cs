using System.Data.SqlClient;
using SIM.Adapters.SqlServer;

namespace SIM.Pipelines.Install
{
  using System.IO;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Extensions;

  #region

  #endregion

  [UsedImplicitly]
  public class GrantPermissions : InstallProcessor
  {
    #region Methods

    #region Protected methods

    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var tempRootFolder = FileSystem.FileSystem.Local.Zip.GetFirstRootFolder(args.PackagePath);
      Assert.IsNotNull(tempRootFolder, "The single root folder within {0} archive was not found".FormatWith(args.PackagePath));
      FileSystem.FileSystem.Local.Directory.Ensure(tempRootFolder);

      var websitePath = Path.Combine(tempRootFolder, "Website");
      var sqlIdentity = SqlServerManager.Instance.GetSqlServerAccountName(args.ConnectionString);
      var iisIdentity = Settings.CoreInstallWebServerIdentity.Value;
      Grant(websitePath, sqlIdentity);
      Grant(websitePath, iisIdentity);

      var dataPath = Path.Combine(tempRootFolder, "Data");
      Grant(dataPath, sqlIdentity);
      Grant(dataPath, iisIdentity);
    }

    #endregion

    #region Private methods

    private void Grant(string path, string identity)
    {
      FileSystem.FileSystem.Local.Directory.Ensure(path);
      FileSystem.FileSystem.Local.Security.EnsurePermissions(path, identity);
    }

    #endregion

    #endregion
  }
}