using JetBrains.Annotations;
using SIM.Extensions;
using Sitecore.Diagnostics.Base;

namespace SIM.Pipelines.InstallSearchIndexes
{
  public class CreateIndexDirectoryAction : InstallSearchIndexesProcessor
  {
    protected override void Process([NotNull] InstallSearchIndexesArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      foreach (var index in args._AvailableSearchIndexesDictionary)
      {
        string newCorePath = args.SolrFolder.EnsureEnd(@"\") + index.Value;
        CreateIndexDirectory(args.SolrVersion, args.SolrFolder, newCorePath);
      }      
    }

    private void CreateIndexDirectory(string solrVersion, string solrFolder, string newCorePath)
    {
      string sourcePath = GetSourceConfPath(solrVersion, solrFolder);
      FileSystem.FileSystem.Local.Directory.Copy(sourcePath, newCorePath, recursive: true);
    }

    private string GetSourceConfPath(string solrVersion, string solrFolder)
    {
      string confPath = string.Empty;

      if (!string.IsNullOrEmpty(solrVersion) && char.IsDigit(solrVersion[0]))
      {
        int firstDigit = int.Parse(solrVersion[0].ToString());

        if (firstDigit >= 7)
        {
          confPath = solrFolder.EnsureEnd(@"\") + @"\configsets\_default";
        }
        else
        {
          confPath = solrFolder.EnsureEnd(@"\") + @"\configsets\data_driven_schema_configs";
        }
      }

      return confPath;
    }
  }
}
