using JetBrains.Annotations;
using SIM.Instances;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;
using System.Collections.Generic;

namespace SIM.Pipelines.InstallSearchIndexes
{
  public class InstallSearchIndexesArgs : ProcessorArgs
  {
    [UsedImplicitly]
    public string InstanceName
    {
      get
      {
        return Instance != null ? Instance.Name : string.Empty;
      }
    }

    public Dictionary<string, string> _AvailableSearchIndexesDictionary;

    public string SolrUrl;

    public string SolrVersion;

    public string SolrFolder;
    public Instance Instance { get; }

    public string AuthCookies { get; }

    public IDictionary<string, string> Headers { get; }

    public InstallSearchIndexesArgs([NotNull] Instance instance, [CanBeNull] Dictionary<string, string> availableSearchIndexesDictionary, [NotNull] string solrUrl,  [NotNull] string solrVersion, [NotNull] string solrFolder, [CanBeNull] IDictionary<string, string> headers = null, [CanBeNull] string cookies = null)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));
      Assert.ArgumentNotNull(solrUrl, nameof(solrUrl));
      Assert.ArgumentNotNull(solrVersion, nameof(solrVersion));
      Assert.ArgumentNotNull(solrFolder, nameof(solrFolder));
      
      SolrUrl = solrUrl;
      SolrVersion = solrVersion;
      SolrFolder = solrFolder;
      _AvailableSearchIndexesDictionary = availableSearchIndexesDictionary;
      Instance = instance;
      AuthCookies = cookies;
      Headers = headers;
    }
  }
}
