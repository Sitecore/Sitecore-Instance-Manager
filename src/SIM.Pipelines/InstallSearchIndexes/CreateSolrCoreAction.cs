using JetBrains.Annotations;
using SIM.Extensions;
using Sitecore.Diagnostics.Base;
using System.IO;

namespace SIM.Pipelines.InstallSearchIndexes
{
  public class CreateSolrCoreAction : InstallSearchIndexesProcessor
  {
    protected override void Process([NotNull] InstallSearchIndexesArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      foreach (var index in args._AvailableSearchIndexesDictionary)
      {
        string newCorePath = args.SolrFolder.EnsureEnd(@"\") + index.Value;

        RequestAndGetResponseStream($"{args.SolrUrl}/admin/cores?action=CREATE&name={index.Value}&instanceDir={newCorePath}&config=solrconfig.xml&schema=schema.xml&dataDir=data");
        UpdateCorePropertiesFile(index.Value, newCorePath);
      }
    }

    private Stream RequestAndGetResponseStream(string url)
    {
      return WebRequestHelper.RequestAndGetResponse(url).GetResponseStream();
    }

    private void UpdateCorePropertiesFile(string coreName, string newCorePath)
    {
      string filePath = string.Format(newCorePath.EnsureEnd(@"\") + @"core.properties");
      string newText = @"update.autoCreateFields=false" + "\r\n" + "name=" + coreName;
      FileSystem.FileSystem.Local.File.Delete(filePath);
      FileSystem.FileSystem.Local.File.WriteAllText(filePath, newText);
    }
  }
}
