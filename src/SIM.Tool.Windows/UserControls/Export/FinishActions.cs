namespace SIM.Tool.Windows.UserControls.Export
{
  using System.IO;
  using SIM.Core;
  using SIM.Tool.Base;

  public class FinishActions
  {
    #region Public methods

    public static void OpenExportFolder(ExportWizardArgs args)
    {
      CoreApp.OpenFolder(Path.GetDirectoryName(args.ExportFilePath));
    }

    #endregion
  }
}