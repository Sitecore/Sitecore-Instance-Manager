namespace SIM.Pipelines.Export
{
  using System.IO.Compression;
  using SIM.Pipelines.Install;
  internal class ExportPostActions : ExportProcessor
  {
    #region Protected methods

    protected override void Process(ExportArgs args)
    {
      var zipName = args.ExportFile;
      var compressionLevel = Settings.CoreExportZipCompressionLevel.Value;

      CompressionLevel zipCompressionLevel;
      if (compressionLevel < 3 && typeof(CompressionLevel).IsEnumDefined(compressionLevel))
      {
        zipCompressionLevel = (CompressionLevel)compressionLevel;
      }
      else
      {
        zipCompressionLevel = CompressionLevel.Optimal;
      }

      ZipFile.CreateFromDirectory(args.Folder, zipName, zipCompressionLevel, false);
    }

    #endregion
  }
}