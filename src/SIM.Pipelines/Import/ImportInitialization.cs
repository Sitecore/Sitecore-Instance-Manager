namespace SIM.Pipelines.Import
{
  using SIM.Adapters.WebServer;
  using JetBrains.Annotations;
  using SIM.Extensions;

  [UsedImplicitly]
  internal class ImportInitialization : ImportProcessor
  {

    #region Protected methods

    protected override void Process(ImportArgs args)
    {
      // Assert.IsTrue(FileSystem.Instance.ZipContainsFile(args.PathToExportedInstance, ImportArgs.appPoolSettingsFileName), "Not valid package for import.");
      // Assert.IsTrue(FileSystem.Instance.ZipContainsFile(args.PathToExportedInstance, ImportArgs.websiteSettingsFileName), "Not valid package for import.");
      // args.temporaryPathToUnpack = Path.GetTempPath();
      GetSettingsFromIisFiles(args);

      // CreateDirectoriesTree(args);
    }

    #endregion



    #region Private methods

    private void CreateDirectoriesTree(ImportArgs args)
    {
      var rootPath = FileSystem.FileSystem.Local.Directory.GetParent(args._VirtualDirectoryPhysicalPath).FullName;
      FileSystem.FileSystem.Local.Directory.CreateDirectory(rootPath);
      FileSystem.FileSystem.Local.Directory.CreateDirectory(rootPath.PathCombine("Data"));
      FileSystem.FileSystem.Local.Directory.CreateDirectory(rootPath.PathCombine("Databases"));
      FileSystem.FileSystem.Local.Directory.CreateDirectory(rootPath.PathCombine("Website"));
    }

    private void GetSettingsFromIisFiles(ImportArgs args)
    {
      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
      {
        var appPoolFilePath = FileSystem.FileSystem.Local.Zip.ZipUnpackFile(args.PathToExportedInstance, args._TemporaryPathToUnpack, ImportArgs.AppPoolSettingsFileName);
        var websiteSettingsFilePath = FileSystem.FileSystem.Local.Zip.ZipUnpackFile(args.PathToExportedInstance, args._TemporaryPathToUnpack, ImportArgs.WebsiteSettingsFileName);
        XmlDocumentEx appPool = new XmlDocumentEx();
        appPool.Load(appPoolFilePath);

        XmlDocumentEx websiteSettings = new XmlDocumentEx();
        websiteSettings.Load(websiteSettingsFilePath);
        args._OldSiteName = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site", "name");
        if (args._SiteName == string.Empty)
        {
          args._SiteName = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site", "name");
        }

        args._VirtualDirectoryPath = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site/application/virtualDirectory", "path");

        if (args._VirtualDirectoryPhysicalPath == string.Empty)
        {
          args._VirtualDirectoryPhysicalPath = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site/application/virtualDirectory", "physicalPath");
        }

        args._AppPoolName = appPool.GetElementAttributeValue("/appcmd/APPPOOL", "APPPOOL.NAME"); // need to set appPoolName in both files
        if (args._AppPoolName != args._SiteName)
        {
          args._AppPoolName = args._SiteName;
        }

        args._AppPoolName = SetupWebsiteHelper.ChooseAppPoolName(args._AppPoolName, context.ApplicationPools);
        args._SiteID = long.Parse(websiteSettings.GetElementAttributeValue("/appcmd/SITE", "SITE.ID"));
      }
    }

    #endregion

  }
}