#region Usings

using SIM.Adapters;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Restore
{
  public class DeleteFiles : RestoreProcessor
  {
    /// <summary>
    /// The is require processing.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The is require processing. 
    /// </returns>
    protected override bool IsRequireProcessing(RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return args.Backup.BackupWebsiteFiles;
    }

    protected override long EvaluateStepsCount(RestoreArgs args)
    {
      return args.Backup.BackupDataFiles ? 2 : 1;
    }

    protected override void Process(RestoreArgs args)
    {
      var webRootPath = args.WebRootPath;
      if (args.Backup.BackupDataFiles)
      {
        FileSystem.Local.Directory.DeleteIfExists(args.DataFolder);
        this.Controller.IncrementProgress();
      }
      if(args.Backup.BackupWebsiteFiles)
      {
        FileSystem.Local.Directory.DeleteIfExists(webRootPath, "sitecore");
      }
    }
  }
}
