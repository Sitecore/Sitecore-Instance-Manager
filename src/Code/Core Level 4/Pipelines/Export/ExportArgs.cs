#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SIM.Base;
using SIM.Instances;
using SIM.Pipelines.Processors;

#endregion

namespace SIM.Pipelines.Export
{
  using SIM.Pipelines.Install;

  /// <summary>
  ///   The export args.
  /// </summary>
  public class ExportArgs : ProcessorArgs
  {
    #region Fields

    /// <summary>
    ///   The folder.
    /// </summary>
    public string Folder
    {
      get
      {
        return this.folder ?? (this.folder = FileSystem.Local.Directory.RegisterTempFolder(GetTempFolder()));
      }
    }

    private string GetTempFolder()
    {
      return FileSystem.Local.Directory.GetTempFolder(Settings.CoreExportTempFolderLocation.Value.EmptyToNull() ?? this.WebRootPath).Path;
    }

    public readonly Instance Instance;
    private readonly string _instanceName;
    public string WebRootPath;
    private readonly string _exportFilePath;
    public readonly ICollection<string> SelectedDatabases;
    public bool WipeSqlServerCredentials;
    public bool IncludeMongoDatabases;
    public readonly bool IncludeTempFolderContents;
    public readonly bool IncludeMediaCacheFolderContents;
    public readonly bool ExcludeUploadFolderContents;
    public readonly bool ExcludeLicenseFile;
    public readonly bool ExcludeDiagnosticsFolderContents;
    public readonly bool ExcludeLogsFolderContents;
    public readonly bool ExcludePackagesFolderContents;
    private string folder;

    #endregion

    #region Properties

    public string InstanceName
    {
      get { return _instanceName; }
    }

    public string ExportFile
    {
      get { return _exportFilePath; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SIM.Pipelines.Export.ExportArgs"/> class.
    /// </summary>
    public ExportArgs(Instance instance, bool wipeSqlServerCredentials, bool includeMongoDatabases, bool includeTempFolderContents, bool includeMediaCacheFolderContents, bool excludeUploadFolderContents, bool excludeLicenseFile, bool excludeDiagnosticsFolderContents, bool excludeLogsFolderContents, bool excludePackagesFolderContents, string exportFilePath = null, IEnumerable<string> selectedDatabases = null)
    {
      Assert.ArgumentNotNull(instance, "instance");

      Instance = instance;
      WebRootPath = instance.WebRootPath;
      _instanceName = Instance.Name;
      _exportFilePath = exportFilePath;
      SelectedDatabases = selectedDatabases.With(x => x.Select(y => y.ToLower()).ToArray());
      WipeSqlServerCredentials = wipeSqlServerCredentials;
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

    public override void Dispose()
    {
      FileSystem.Local.Directory.DeleteIfExists(this.Folder);
    }
  }
}
