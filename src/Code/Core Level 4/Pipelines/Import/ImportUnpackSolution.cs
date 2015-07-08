using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SIM.Base;
using System.IO;

namespace SIM.Pipelines.Import
{
  [UsedImplicitly]
  class ImportUnpackSolution : ImportProcessor
  {
    protected override void Process(ImportArgs args)
    {
      //Assert.IsTrue(FileSystem.Instance.ZipContainsSingleFile(args.PathToExportedInstance, ImportArgs.appPoolSettingsFileName), "Not valid package for import.");
      //Assert.IsTrue(FileSystem.Instance.ZipContainsSingleFile(args.PathToExportedInstance, ImportArgs.websiteSettingsFileName), "Not valid package for import.");
      //
      //args.temporaryPathToUnpack = Path.GetTempPath();
      //        
      string webRootName = args.virtualDirectoryPhysicalPath.Split('\\')[args.virtualDirectoryPhysicalPath.Split('\\').Length - 1];
      FileSystem.Local.Zip.ZipUnpackFolder(args.PathToExportedInstance, args.rootPath, "Data");
      FileSystem.Local.Zip.ZipUnpackFolder(args.PathToExportedInstance, args.rootPath, webRootName);
    }
  }
}
