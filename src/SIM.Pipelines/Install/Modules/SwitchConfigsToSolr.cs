using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SIM.Instances;
using SIM.Pipelines.InstallModules;
using SIM.Products;

namespace SIM.Pipelines.Install.Modules
{
  using SIM.Extensions;

  public class SwitchConfigsToSolr:IPackageInstallActions
  {
    #region Properties

    private Instance Instance { get; set; }
    private IList<string> IncludeDirFiles { get; set; }

    #endregion

    #region Public methods

    /// <summary>
    /// Enables Solr config files, disables Lucene files, and updates core element to include instance name.
    /// </summary>
    /// <remarks>
    /// Solr IOC files are not activated, as they require IOC dlls.
    /// Lucene default files are not deactivated, as they are required if user wants to run both Lucene and Solr indexes.
    /// Facebook file is disabled, even though it does not have a Solr equivalent, because this is specified by comments in 
    /// file and is necessary to load site
    /// </remarks>
    public void Execute(Instance instance, Product module)
    {
      Instance = instance;
      IncludeDirFiles = GetIncludeDirFiles();
      
      EnableNonIocSolrFiles();

      IncludeDirFiles = GetIncludeDirFiles();

      List<string> luceneFilesToDisable = GetMatchingLuceneFiles(GetLuceneConfigsSkippingDefaults(), GetSolrConfigs());
      AddFacebookFile(luceneFilesToDisable);
      luceneFilesToDisable.ForEach(DisableFile);

      RenameCoreInConfigFile(GetSolrConfigs());
    }

    private void AddFacebookFile(List<string> luceneFilesToDisable)
    {
      string facebookFile =
        IncludeDirFiles.FirstOrDefault(file => file.EndsWith("Sitecore.Social.Lucene.Index.Analytics.Facebook.config"));
      if (facebookFile != null)
      {
        luceneFilesToDisable.Add(facebookFile);
      }
    }

    #endregion

    #region Private methods

    public IList<string> GetIncludeDirFiles()
    {
      string path = Instance.WebRootPath.EnsureEnd(@"\") + @"App_Config\Include";
      const string filter = "*";
      const SearchOption allDirectories = SearchOption.AllDirectories;
      return GetFiles(path, filter, allDirectories);
    }

    private void EnableNonIocSolrFiles()
    {
      List<string> disabledSolrFiles =
        IncludeDirFiles.Where(
          s => s.ToLower().Contains(".solr.") && 
               !s.ToLower().Contains(".ioc.") &&
               (s.EndsWith(".example") || s.EndsWith(".disabled"))).ToList();

      disabledSolrFiles.ForEach(f => RenameFile(f, RemoveEnding(f)));
    }

    private List<string> GetLuceneConfigsSkippingDefaults()
    {
      return IncludeDirFiles.Where(fileName =>
        fileName.ToLower().Contains(".lucene.") &&
        !fileName.ToLower().Contains("default") &&
        fileName.EndsWith(".config")).ToList();
    }

    private List<string> GetSolrConfigs()
    {
      return IncludeDirFiles.Where(fileName =>
        fileName.ToLower().Contains(".solr.") &&
        fileName.EndsWith(".config")).ToList();
    }

    private static string RemoveEnding(string fileName)
    {
      return Regex.Replace(fileName, @"\.(example|disabled)$", "");
    }

    private static List<string> GetMatchingLuceneFiles(List<string> luceneFiles, List<string> solrFiles)
    {
      return luceneFiles.Where(file => AnyMatchingSolrFile(solrFiles, file)).ToList();
    }

    private static bool AnyMatchingSolrFile(List<string> solrFiles, string luceneFileName)
    {
      return solrFiles.Any(fileName => fileName.ToLower().Replace(".solr.", ".lucene.") == luceneFileName.ToLower());
    }

    private void DisableFile(string configFileName)
    {
      RenameFile(configFileName, configFileName + ".disabled");
    }

    private void RenameCoreInConfigFile(IList<string> solrFiles)
    {
      string oldCore = @"<param desc=""core"">$(id)</param>";
      string newCore = $@"<param desc=""core"">{this.Instance.Name}_$(id)</param>";
      foreach (var file in solrFiles)
      {
        string oldContents = FileReadAllText(file);
        string newContents = oldContents.Replace(oldCore, newCore);
        FileWriteAllText(file, newContents);
      }
    }

    #endregion

    #region Virtual methods

    // System calls are virtual for unit testing.

    public virtual string[] GetFiles(string path, string filter, SearchOption allDirectories)
    {
      return FileSystem.FileSystem.Local.Directory.GetFiles(path, filter,allDirectories);
    }

    public virtual void RenameFile(string oldPath, string newPath)
    {
      FileSystem.FileSystem.Local.File.Move(oldPath, newPath);
    }

    public virtual void FileWriteAllText(string path, string text)
    {
      FileSystem.FileSystem.Local.File.WriteAllText(path, text);
    }

    public virtual string FileReadAllText(string path)
    {
      return FileSystem.FileSystem.Local.File.ReadAllText(path);
    }

    #endregion
  }
}