namespace SIM.Pipelines.Export
{
  using System.Collections.Generic;
  using System.Linq;
  using SIM.Instances;
  using SIM.Pipelines.Install;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;

  public class ExportArgs : ProcessorArgs
  {
    #region Fields

    public readonly bool ExcludeDiagnosticsFolderContents;
    public readonly bool ExcludeLicenseFile;
    public readonly bool ExcludeLogsFolderContents;
    public readonly bool ExcludePackagesFolderContents;
    public readonly bool ExcludeUploadFolderContents;
    public readonly bool IncludeMediaCacheFolderContents;
    public readonly bool IncludeTempFolderContents;
    public readonly Instance Instance;
    public readonly ICollection<string> SelectedDatabases;
    public bool IncludeMongoDatabases;
    public string WebRootPath;
    public bool WipeSqlServerCredentials;
    private readonly string _exportFilePath;
    private readonly string _instanceName;
    private string folder;

    #endregion

    #region Properties

    public string ExportFile
    {
      get
      {
        return this._exportFilePath;
      }
    }

    public string InstanceName
    {
      get
      {
        return this._instanceName;
      }
    }

    #endregion

    #region Constructors

    public ExportArgs(Instance instance, bool wipeSqlServerCredentials, bool includeMongoDatabases, bool includeTempFolderContents, bool includeMediaCacheFolderContents, bool excludeUploadFolderContents, bool excludeLicenseFile, bool excludeDiagnosticsFolderContents, bool excludeLogsFolderContents, bool excludePackagesFolderContents, string exportFilePath = null, IEnumerable<string> selectedDatabases = null)
    {
      Assert.ArgumentNotNull(instance, "instance");

      this.Instance = instance;
      this.WebRootPath = instance.WebRootPath;
      this._instanceName = this.Instance.Name;
      this._exportFilePath = exportFilePath;
      this.SelectedDatabases = selectedDatabases.With(x => x.Select(y => y.ToLower()).ToArray());
      this.WipeSqlServerCredentials = wipeSqlServerCredentials;
      this.IncludeMongoDatabases = includeMongoDatabases;
      this.IncludeTempFolderContents = includeTempFolderContents;
      this.IncludeMediaCacheFolderContents = includeMediaCacheFolderContents;
      this.ExcludeUploadFolderContents = excludeUploadFolderContents;
      this.ExcludeLicenseFile = excludeLicenseFile;
      this.ExcludeDiagnosticsFolderContents = excludeDiagnosticsFolderContents;
      this.ExcludeLogsFolderContents = excludeLogsFolderContents;
      this.ExcludePackagesFolderContents = excludePackagesFolderContents;
    }

    #endregion

    #region Public properties

    public string Folder
    {
      get
      {
        return this.folder ?? (this.folder = FileSystem.FileSystem.Local.Directory.RegisterTempFolder(this.GetTempFolder()));
      }
    }

    #endregion

    #region Public methods

    public override void Dispose()
    {
      FileSystem.FileSystem.Local.Directory.DeleteIfExists(this.Folder);
    }

    #endregion

    #region Private methods

    private string GetTempFolder()
    {
      return FileSystem.FileSystem.Local.Directory.GetTempFolder(Settings.CoreExportTempFolderLocation.Value.EmptyToNull() ?? this.WebRootPath).Path;
    }

    #endregion
  }
}