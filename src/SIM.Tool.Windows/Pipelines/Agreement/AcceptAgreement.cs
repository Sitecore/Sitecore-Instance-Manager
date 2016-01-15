namespace SIM.Tool.Windows.Pipelines.Agreement
{
  using System.IO;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class AcceptAgreement : Processor
  {
    #region Protected methods

    protected override void Process([CanBeNull] ProcessorArgs args)
    {
      var tempFolder = ApplicationManager.TempFolder;
      if (!Directory.Exists(tempFolder))
      {
        Directory.CreateDirectory(tempFolder);
      }

      File.WriteAllText(Path.Combine(tempFolder, "agreement-accepted.txt"), @"agreement accepted");
    }

    #endregion
  }
}