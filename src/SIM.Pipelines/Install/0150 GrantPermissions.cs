namespace SIM.Pipelines.Install
{
  using System.IO;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class GrantPermissions : InstallProcessor
  {
    #region Methods

    #region Protected methods

    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var tempRootFolder = FileSystem.FileSystem.Local.Zip.GetFirstRootFolder(args.PackagePath);
      Assert.IsNotNull(tempRootFolder, "The single root folder within {0} archive was not found".FormatWith(args.PackagePath));
      FileSystem.FileSystem.Local.Directory.Ensure(tempRootFolder);

      var websitePath = Path.Combine(tempRootFolder, "Website");
      this.Grant(websitePath);

      var dataPath = Path.Combine(tempRootFolder, "Data");
      this.Grant(dataPath);
    }

    #endregion

    #region Private methods

    private void Grant(string path)
    {
      FileSystem.FileSystem.Local.Directory.Ensure(path);
      FileSystem.FileSystem.Local.Security.EnsurePermissions(path, Settings.CoreInstallWebServerIdentity.Value);
    }

    #endregion

    #endregion
  }
}