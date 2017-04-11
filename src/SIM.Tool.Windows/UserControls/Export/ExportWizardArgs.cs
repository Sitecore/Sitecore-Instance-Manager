namespace SIM.Tool.Windows.UserControls.Export
{
  using System.Collections.Generic;
  using SIM.Instances;
  using SIM.Pipelines.Export;
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base.Wizards;

  public class ExportWizardArgs : WizardArgs
  {
    #region Fields

    public Instance Instance { get; }
    public bool _WipeSqlServerCredentials;
    private string _instanceName { get; }

    #endregion

    #region Constructors

    public ExportWizardArgs(Instance instance, string exportFilePath = null)
    {
      Instance = instance;
      _instanceName = instance.Name;
      ExportFilePath = exportFilePath ?? string.Empty;
    }

    #endregion

    #region Public properties

    public bool ExcludeDiagnosticsFolderContents { get; set; }
    public bool ExcludeLicenseFile { get; set; }
    public bool ExcludeLogsFolderContents { get; set; }
    public bool ExcludePackagesFolderContents { get; set; }
    public bool ExcludeUploadFolderContents { get; set; }
    public string ExportFilePath { get; set; }
    public bool IncludeMediaCacheFolderContents { get; set; }
    public bool IncludeMongoDatabases { get; set; }
    public bool IncludeTempFolderContents { get; set; }

    public string InstanceName
    {
      get
      {
        return _instanceName;
      }
    }

    public IEnumerable<string> SelectedDatabases { get; set; }

    #endregion

    #region Public methods

    public override ProcessorArgs ToProcessorArgs()
    {
      return new ExportArgs(Instance, _WipeSqlServerCredentials, IncludeMongoDatabases, IncludeTempFolderContents, IncludeMediaCacheFolderContents, ExcludeUploadFolderContents, ExcludeLicenseFile, ExcludeDiagnosticsFolderContents, ExcludeLogsFolderContents, ExcludePackagesFolderContents, ExportFilePath, SelectedDatabases);
    }

    #endregion
  }
}