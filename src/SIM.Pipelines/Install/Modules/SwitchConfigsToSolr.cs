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
    public Instance Instance { get; set; }

    public void Execute(Instance instance, Product module)
    {
      Instance = instance;

      EnableSolrFiles();

      DisableLuceneFiles();

      RenameCores();

    }

    private void EnableSolrFiles()
    {
      var configFiles = GetConfigFiles();

      var disabledSolrFiles =
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
      var configFiles = GetConfigFiles(); 

      var luceneFiles = configFiles.Where(s => s.ToLower().Contains(".lucene.") && s.EndsWith(".config")).ToList();

      var solrFiles = configFiles.Where(s => s.ToLower().Contains(".solr.") && s.EndsWith(".config")).ToList();

      var filesToRename = luceneFiles.Where(file => AnyMatchingSolrFile(solrFiles, file));

      filesToRename
        .ToList().ForEach(s => RenameFile(s, s + ".disabled"));
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

    #region System Calls marked Virtual for unit testing

    public virtual IEnumerable<string> GetConfigFiles()
    {
      return FileSystem.FileSystem.Local.Directory.GetFiles(Instance.WebRootPath.EnsureEnd(@"\") + "App_Config", "*",
        SearchOption.AllDirectories);
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