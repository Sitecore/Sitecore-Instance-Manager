namespace SIM.Tool.Windows.Pipelines.Agreement
{
  using System.IO;
  using SIM.Pipelines.Processors;

  public class AcceptAgreement : Processor
  {
    #region Protected methods

    protected override void Process(ProcessorArgs args)
    {
      File.WriteAllText(Path.Combine(ApplicationManager.TempFolder, "agreement-accepted.txt"), @"agreement accepted");
    }

    #endregion
  }
}