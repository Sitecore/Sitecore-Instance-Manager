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

      Instance = instance;
      InstanceName = instance.Name;
      WebRootPath = instance.WebRootPath;
      ExportFile = exportFile;
      _SelectedDatabases = selectedDatabases.With(x => x.Select(y => y.ToLower()).ToArray());
      WipeSqlServerCredentials = wipeSqlServerCredentials;
      IncludeMongoDatabases = includeMongoDatabases;
      IncludeTempFolderContents = includeTempFolderContents;
      IncludeMediaCacheFolderContents = includeMediaCacheFolderContents;
      ExcludeUploadFolderContents = excludeUploadFolderContents;
      ExcludeLicenseFile = excludeLicenseFile;
      ExcludeDiagnosticsFolderContents = excludeDiagnosticsFolderContents;
      ExcludeLogsFolderContents = excludeLogsFolderContents;
      ExcludePackagesFolderContents = excludePackagesFolderContents;
    }

    public string InstanceName { get; }

    #endregion

    #region Public properties

    public string Folder
    {
      get
      {
        return _Folder ?? (_Folder = FileSystem.FileSystem.Local.Directory.RegisterTempFolder(GetTempFolder()));
      }
    }

    #endregion

    #region Public methods

    public override void Dispose()
    {
      FileSystem.FileSystem.Local.Directory.DeleteIfExists(Folder);
    }

    #endregion

    #region Private methods

    private string GetTempFolder()
    {
      var tempLocation = Settings.CoreExportTempFolderLocation.Value;
      var webRootPath = WebRootPath;

      return GetTempFolder(tempLocation, webRootPath);
    }

    public static string GetTempFolder(string tempLocation, string webRootPath)
    {
      if (!string.IsNullOrEmpty(tempLocation))
      {
        return FileSystem.FileSystem.Local.Directory.GetTempFolder(tempLocation, false).Path;
      }

      return FileSystem.FileSystem.Local.Directory.GetTempFolder(webRootPath, true).Path;
    }

    #endregion
  }
}