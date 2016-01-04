namespace SIM.Pipelines.Import
{
  using SIM.Adapters.WebServer;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  internal class ImportInitialization : ImportProcessor
  {

    #region Protected methods

    protected override void Process(ImportArgs args)
    {
      // Assert.IsTrue(FileSystem.Instance.ZipContainsFile(args.PathToExportedInstance, ImportArgs.appPoolSettingsFileName), "Not valid package for import.");
      // Assert.IsTrue(FileSystem.Instance.ZipContainsFile(args.PathToExportedInstance, ImportArgs.websiteSettingsFileName), "Not valid package for import.");
      // args.temporaryPathToUnpack = Path.GetTempPath();
      this.GetSettingsFromIISFiles(args);

      // CreateDirectoriesTree(args);
    }

    #endregion



    #region Private methods

    private void CreateDirectoriesTree(ImportArgs args)
    {
      string rootPath = FileSystem.FileSystem.Local.Directory.GetParent(args.virtualDirectoryPhysicalPath).FullName;
      FileSystem.FileSystem.Local.Directory.CreateDirectory(rootPath);
      FileSystem.FileSystem.Local.Directory.CreateDirectory(rootPath.PathCombine("Data"));
      FileSystem.FileSystem.Local.Directory.CreateDirectory(rootPath.PathCombine("Databases"));
      FileSystem.FileSystem.Local.Directory.CreateDirectory(rootPath.PathCombine("Website"));
    }

    private void GetSettingsFromIISFiles(ImportArgs args)
    {
      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("ImportSolution.Initialization"))
      {
        string appPoolFilePath = FileSystem.FileSystem.Local.Zip.ZipUnpackFile(args.PathToExportedInstance, args.temporaryPathToUnpack, ImportArgs.appPoolSettingsFileName);
        string websiteSettingsFilePath = FileSystem.FileSystem.Local.Zip.ZipUnpackFile(args.PathToExportedInstance, args.temporaryPathToUnpack, ImportArgs.websiteSettingsFileName);
        XmlDocumentEx appPool = new XmlDocumentEx();
        appPool.Load(appPoolFilePath);

        XmlDocumentEx websiteSettings = new XmlDocumentEx();
        websiteSettings.Load(websiteSettingsFilePath);
        args.oldSiteName = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site", "name");
        if (args.siteName == string.Empty)
        {
          args.siteName = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site", "name");
        }

        args.virtualDirectoryPath = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site/application/virtualDirectory", "path");

        if (args.virtualDirectoryPhysicalPath == string.Empty)
        {
          args.virtualDirectoryPhysicalPath = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site/application/virtualDirectory", "physicalPath");
        }

        args.appPoolName = appPool.GetElementAttributeValue("/appcmd/APPPOOL", "APPPOOL.NAME"); // need to set appPoolName in both files
        if (args.appPoolName != args.siteName)
        {
          args.appPoolName = args.siteName;
        }

        args.appPoolName = SetupWebsiteHelper.ChooseAppPoolName(args.appPoolName, context.ApplicationPools);
        args.siteID = long.Parse(websiteSettings.GetElementAttributeValue("/appcmd/SITE", "SITE.ID"));
      }
    }

    #endregion

  }
}