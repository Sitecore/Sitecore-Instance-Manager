using System;
using System.IO;
using SIM.Base;

namespace SIM.Pipelines.Import
{
  using SIM.Pipelines.Install;

  public class UpdateDataFolder : ImportProcessor
  {
    protected override void Process(ImportArgs args)
    {
      string websiteFolderPath = args.rootPath;
      SetupWebsiteHelper.SetDataFolder(websiteFolderPath);
    }
  }
}