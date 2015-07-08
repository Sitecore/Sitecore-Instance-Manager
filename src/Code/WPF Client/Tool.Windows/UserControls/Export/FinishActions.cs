using System.IO;
using SIM.Tool.Base;

namespace SIM.Tool.Windows.UserControls.Export
{
  public class FinishActions
	{
		public static void OpenExportFolder(ExportWizardArgs args)
		{
			WindowHelper.OpenFolder(Path.GetDirectoryName(args.ExportFilePath));
		}
	}
}
