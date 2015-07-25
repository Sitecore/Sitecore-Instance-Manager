namespace SIM.Pipelines.Export
{
  using SIM.Pipelines.Install;
  internal class ExportPostActions : ExportProcessor
  {
    #region Protected methods

    protected override void Process(ExportArgs args)
    {
      var zipName = args.ExportFile;
      FileSystem.FileSystem.Local.Zip.CreateZip(args.Folder, zipName, compressionLevel: Settings.CoreExportZipCompressionLevel.Value);
    }

    #endregion
  }
}