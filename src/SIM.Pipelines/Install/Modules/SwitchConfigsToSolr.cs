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

    public Instance Instance { get; set; }

    #endregion

    #region Public methods

    public void Execute(Instance instance, Product module)
    {
      Instance = instance;
      EnableSolrFiles(GetConfigFiles());

      IList<string> configFiles = GetConfigFiles();
      List<string> solrFiles = GetSolrFrom(configFiles);
      DisabledMatchedFiles(GetLuceneSkippingDefaultsFrom(configFiles), solrFiles);
      DisabledSpecificFile(configFiles, "Sitecore.Social.Lucene.Index.Analytics.Facebook.config");
      RenameCores(solrFiles);
    }


    private static List<string> GetSolrFrom(IEnumerable<string> configFiles)
    {
      return configFiles.Where(fileName =>
        fileName.ToLower().Contains(".solr.") && 
        fileName.EndsWith(".config")).ToList();
    }

    private static List<string> GetLuceneSkippingDefaultsFrom(IEnumerable<string> configFiles)
    {
      return configFiles.Where(fileName => 
        fileName.ToLower().Contains(".lucene.") &&
        !fileName.ToLower().Contains("default") &&
        fileName.EndsWith(".config")).ToList();
    }

    #endregion

    #region Private methods

    private void EnableSolrFiles(IEnumerable<string> configFiles)
    {
      List<string> disabledSolrFiles =
        configFiles.Where(
          s => s.ToLower().Contains(".solr.") && 
          !s.ToLower().Contains(".ioc.") &&
        (s.EndsWith(".example") || s.EndsWith(".disabled"))).ToList();

      disabledSolrFiles.ForEach(f => RenameFile(f, RemoveEnding(f)));
    }

    private static string RemoveEnding(string fileName)
    {
      return Regex.Replace(fileName, @"\.(example|disabled)$", "");
    }

    private void DisabledMatchedFiles(List<string> filesToDisabledIfMatched, List<string> matchingFiles)
    {

      List<string> filesToDisable =
        filesToDisabledIfMatched.Where(file => AnyMatchingSolrFile(matchingFiles, file)).ToList();

      filesToDisable.ForEach(DisableFile);
    }

    private void DisabledSpecificFile(IEnumerable<string> fileList, string specificFile)
    {

      List<string> filesToDisable =
        fileList.Where(file => file.EndsWith(specificFile)).ToList(); 

      filesToDisable.ForEach(DisableFile);
    }

    private void DisableFile(string configFileName)
    {
      RenameFile(configFileName, configFileName + ".disabled");
    }

    private void RenameCores(IList<string> solrFiles)
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

    //TODO Rename
    private static bool AnyMatchingSolrFile(List<string> solrFiles, string luceneFileName)
    {
      return solrFiles.Any(solrFile => solrFile.ToLower().Replace(".solr.", ".lucene.") == luceneFileName.ToLower());
    }

    public IList<string> GetConfigFiles()
    {
      string path = Instance.WebRootPath.EnsureEnd(@"\") + @"App_Config\Include";
      const string filter = "*";
      const SearchOption allDirectories = SearchOption.AllDirectories;
      return GetFiles(path, filter, allDirectories);
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