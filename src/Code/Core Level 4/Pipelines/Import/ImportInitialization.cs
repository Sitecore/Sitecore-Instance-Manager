using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SIM.Base;
using System.IO;
using SIM.Adapters.WebServer;
using SIM.Pipelines.Install;


namespace SIM.Pipelines.Import
{
    [UsedImplicitly]
    class ImportInitialization : ImportProcessor
    {
        #region Fields

        #endregion
        //
        protected override void Process(ImportArgs args)
        {
          //Assert.IsTrue(FileSystem.Instance.ZipContainsFile(args.PathToExportedInstance, ImportArgs.appPoolSettingsFileName), "Not valid package for import.");
          //Assert.IsTrue(FileSystem.Instance.ZipContainsFile(args.PathToExportedInstance, ImportArgs.websiteSettingsFileName), "Not valid package for import.");
            //            
            //args.temporaryPathToUnpack = Path.GetTempPath();
            //
            GetSettingsFromIISFiles(args);
            //CreateDirectoriesTree(args);
        }
        //
        void GetSettingsFromIISFiles(ImportArgs args)
        {
          using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("ImportSolution.Initialization"))
          {
            string appPoolFilePath = FileSystem.Local.Zip.ZipUnpackFile(args.PathToExportedInstance, args.temporaryPathToUnpack, ImportArgs.appPoolSettingsFileName);
            string websiteSettingsFilePath = FileSystem.Local.Zip.ZipUnpackFile(args.PathToExportedInstance, args.temporaryPathToUnpack, ImportArgs.websiteSettingsFileName);
            //
            XmlDocumentEx appPool = new XmlDocumentEx();
            appPool.Load(appPoolFilePath);

            XmlDocumentEx websiteSettings = new XmlDocumentEx();
            websiteSettings.Load(websiteSettingsFilePath);
            //
            args.oldSiteName = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site", "name");
            if (args.siteName == "")
              args.siteName = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site", "name");
            
            args.virtualDirectoryPath = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site/application/virtualDirectory", "path");

            if(args.virtualDirectoryPhysicalPath == "")
              args.virtualDirectoryPhysicalPath = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site/application/virtualDirectory", "physicalPath");

            args.appPoolName = appPool.GetElementAttributeValue("/appcmd/APPPOOL", "APPPOOL.NAME"); // need to set appPoolName in both files
            if (args.appPoolName != args.siteName) args.appPoolName = args.siteName;
            args.appPoolName = SetupWebsiteHelper.ChooseAppPoolName(args.appPoolName, context.ApplicationPools);
            args.siteID = long.Parse(websiteSettings.GetElementAttributeValue("/appcmd/SITE", "SITE.ID")); 
          }
        }
        //
        void CreateDirectoriesTree(ImportArgs args)
        {
          string rootPath = FileSystem.Local.Directory.GetParent(args.virtualDirectoryPhysicalPath).FullName;
          FileSystem.Local.Directory.CreateDirectory(rootPath);
          FileSystem.Local.Directory.CreateDirectory(rootPath.PathCombine("Data"));
          FileSystem.Local.Directory.CreateDirectory(rootPath.PathCombine("Databases"));
          FileSystem.Local.Directory.CreateDirectory(rootPath.PathCombine("Website"));
        }
        //
    }
}
