namespace SIM.Pipelines.Export
{
  internal class ExportPostActions : ExportProcessor
  {
    #region Protected methods

    protected override void Process(ExportArgs args)
    {
      var zipName = args.ExportFile;
      FileSystem.FileSystem.Local.Zip.CreateZip(args.Folder, zipName);
    }

    #endregion
  }
}