using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SIM.Instances;
using SIM.Pipelines.InstallModules;
using SIM.Products;

namespace SIM.Pipelines.Install.Modules
{
  public class SwitchConfigsToSolr:IPackageInstallActions
  {
    #region Properties

    public Instance Instance { get; set; }

    #endregion

    #region Public methods

    public void Execute(Instance instance, Product module)
    {
      Instance = instance;
      EnableSolrFiles();
      DisableLuceneFiles();
      RenameCores();
    }
    #endregion

    #region Private methods

    private void EnableSolrFiles()
    {
      IEnumerable<string> configFiles = GetConfigFiles();

      List<string> disabledSolrFiles =
        configFiles.Where(
          s => s.ToLower().Contains(".solr.") && 
        (s.EndsWith(".example") || s.EndsWith(".disabled"))).ToList();

      disabledSolrFiles.ForEach(f => RenameFile(f, RemoveEnding(f)));
    }

    private static string RemoveEnding(string fileName)
    {
      return Regex.Replace(fileName, @"\.(example|disabled)$", "");
    }

    private void DisableLuceneFiles()
    {
      IEnumerable<string> configFiles = GetConfigFiles();

      List<string> luceneFiles = 
        configFiles.Where(s => s.ToLower().Contains(".lucene.") && s.EndsWith(".config")).ToList();

      List<string> solrFiles = 
        configFiles.Where(s => s.ToLower().Contains(".solr.") && s.EndsWith(".config")).ToList();

      List<string> filesToRename =
        luceneFiles.Where(file => AnyMatchingSolrFile(solrFiles, file)).ToList();

      filesToRename.ForEach(s => RenameFile(s, s + ".disabled"));
    }

    private void RenameCores()
    {
      string oldCore = @"<param desc=""core"">$(id)</param>";
      string newCore = string.Format(@"<param desc=""core"">{0}_$(id)</param>", this.Instance.Name);
      var solrFiles = GetConfigFiles().Where(s => s.ToLower().Contains(".solr.")).ToList();
      foreach (var file in solrFiles)
      {
        string oldContents = FileReadAllText(file);
        string newContents = oldContents.Replace(oldCore, newCore);
        FileWriteAllText(file, newContents);
      }
    }

    private static bool AnyMatchingSolrFile(List<string> solrFiles, string luceneFileName)
    {
      return solrFiles.Any(solrFile => solrFile.ToLower().Replace(".solr.", ".lucene.") == luceneFileName.ToLower());
    }

    public IEnumerable<string> GetConfigFiles()
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