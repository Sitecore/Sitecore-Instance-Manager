using SIM.Tool.Base.Wizards;
using System.IO;

namespace SIM.Tool.Windows
{
  public class SaveAgreement : IAfterLastWizardPipelineStep
  {
    public void Execute(WizardArgs args)
    {
      var tempFolder = ApplicationManager.TempFolder;
      if (!Directory.Exists(tempFolder))
      {
        Directory.CreateDirectory(tempFolder);
      }

      File.WriteAllText(Path.Combine(tempFolder, "agreement-accepted.txt"), @"agreement accepted");
    }
  }
}
