using System.Collections.Generic;
using SIM.Instances;
using SIM.Pipelines.Export;
using SIM.Pipelines.Processors;
using SIM.Tool.Base.Wizards;

namespace SIM.Tool.Windows.UserControls.Export
{
	public class ExportWizardArgs : WizardArgs
	{
		public readonly Instance Instance;
		private readonly string _instanceName;
		public bool WipeSqlServerCredentials;

		public ExportWizardArgs(Instance instance, string exportFilePath = null)
		{
			Instance = instance;
			_instanceName = instance.Name;
			ExportFilePath = exportFilePath ?? string.Empty;
		}

		public IEnumerable<string> SelectedDatabases { get; set; } 

		public string ExportFilePath { get; set; }

		public string InstanceName
		{
			get { return _instanceName; }
		}

    public bool IncludeTempFolderContents { get; set; }
    public bool IncludeMongoDatabases { get; set; }

    public bool IncludeMediaCacheFolderContents { get; set; }
    public bool ExcludeLicenseFile { get; set; }
	  public bool ExcludeDiagnosticsFolderContents { get; set; }
	  public bool ExcludeLogsFolderContents { get; set; }
	  public bool ExcludePackagesFolderContents { get; set; }
	  public bool ExcludeUploadFolderContents { get; set; }

	  public override ProcessorArgs ToProcessorArgs()
		{
      return new ExportArgs(Instance, WipeSqlServerCredentials, IncludeMongoDatabases, IncludeTempFolderContents, IncludeMediaCacheFolderContents, ExcludeUploadFolderContents, ExcludeLicenseFile, ExcludeDiagnosticsFolderContents, ExcludeLogsFolderContents, ExcludePackagesFolderContents, ExportFilePath, SelectedDatabases);
		}
	}
}
