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

    public readonly Instance Instance;
    public bool WipeSqlServerCredentials;
    private readonly string _instanceName;

    #endregion

    #region Constructors

    public ExportWizardArgs(Instance instance, string exportFilePath = null)
    {
      this.Instance = instance;
      this._instanceName = instance.Name;
      this.ExportFilePath = exportFilePath ?? string.Empty;
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
        return this._instanceName;
      }
    }

    public IEnumerable<string> SelectedDatabases { get; set; }

    #endregion

    #region Public methods

    public override ProcessorArgs ToProcessorArgs()
    {
      return new ExportArgs(this.Instance, this.WipeSqlServerCredentials, this.IncludeMongoDatabases, this.IncludeTempFolderContents, this.IncludeMediaCacheFolderContents, this.ExcludeUploadFolderContents, this.ExcludeLicenseFile, this.ExcludeDiagnosticsFolderContents, this.ExcludeLogsFolderContents, this.ExcludePackagesFolderContents, this.ExportFilePath, this.SelectedDatabases);
    }

    #endregion
  }
}