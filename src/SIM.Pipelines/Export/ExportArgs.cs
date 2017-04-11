namespace SIM.Pipelines.Export
{
  using System.Collections.Generic;
  using System.Linq;
  using SIM.Instances;
  using SIM.Pipelines.Install;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using SIM.Extensions;

  public class ExportArgs : ProcessorArgs
  {
    #region Fields

    public bool ExcludeDiagnosticsFolderContents { get; }
    public bool ExcludeLicenseFile { get; }
    public bool ExcludeLogsFolderContents { get; }
    public bool ExcludePackagesFolderContents { get; }
    public bool ExcludeUploadFolderContents { get; }
    public bool IncludeMediaCacheFolderContents { get; }
    public bool IncludeTempFolderContents { get; }
    public Instance Instance { get; }
    public readonly ICollection<string> _SelectedDatabases;
    public bool IncludeMongoDatabases { get; }
    public string WebRootPath { get; }
    public bool WipeSqlServerCredentials { get; }
    public string ExportFile { get; }

    private string _Folder;

    #endregion

    #region Properties               

    #endregion

    #region Constructors

    public ExportArgs(Instance instance, bool wipeSqlServerCredentials, bool includeMongoDatabases, bool includeTempFolderContents, bool includeMediaCacheFolderContents, bool excludeUploadFolderContents, bool excludeLicenseFile, bool excludeDiagnosticsFolderContents, bool excludeLogsFolderContents, bool excludePackagesFolderContents, string exportFile = null, IEnumerable<string> selectedDatabases = null)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));

      this.Instance = instance;
      InstanceName = instance.Name;
      this.WebRootPath = instance.WebRootPath;
      this.ExportFile = exportFile;
      this._SelectedDatabases = selectedDatabases.With(x => x.Select(y => y.ToLower()).ToArray());
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

    public string InstanceName { get; }

    #endregion

    #region Public properties

    public string Folder
    {
      get
      {
        return this._Folder ?? (this._Folder = FileSystem.FileSystem.Local.Directory.RegisterTempFolder(this.GetTempFolder()));
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