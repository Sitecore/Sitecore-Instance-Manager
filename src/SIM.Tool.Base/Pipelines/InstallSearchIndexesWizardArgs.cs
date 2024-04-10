using JetBrains.Annotations;
using SIM.Extensions;
using SIM.Instances;
using SIM.Pipelines.InstallModules;
using SIM.Pipelines.InstallSearchIndexes;
using SIM.Pipelines.Processors;
using SIM.Tool.Base.Wizards;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Tool.Base.Pipelines
{
  [UsedImplicitly]
  public class InstallSearchIndexesWizardArgs : WizardArgs
  {   
    [UsedImplicitly]
    public string InstanceName
    {
      get
      {
        return Instance != null ? Instance.Name : string.Empty;
      }
    }

    public List<string> _AvailableSearchIndexesList;

    public Dictionary<string, string> _AvailableSearchIndexesDictionary;

    public Instance Instance { get; }

    public string SolrUrl;

    public string SolrVersion;

    public string SolrFolder;

    public IDictionary<string, string> Headers { get; }

    public string AuthCookies { get; }

    public InstallSearchIndexesWizardArgs(Instance instance, Dictionary<string, string> availableSearchIndexesDictionary, string solrUrl, string solrVersion, string solrFolder, IDictionary<string, string> headers = null, string cookies = null)
    {
      SolrUrl = solrUrl;
      SolrVersion = solrVersion;
      SolrFolder = solrFolder;
      _AvailableSearchIndexesList = new List<string>();

      foreach (var availableIndex in availableSearchIndexesDictionary)
      {
        _AvailableSearchIndexesList.Add(availableIndex.Value);
      }
      _AvailableSearchIndexesDictionary = availableSearchIndexesDictionary;
      Instance = instance;
      AuthCookies = cookies;
      Headers = headers;
    }    

    public override ProcessorArgs ToProcessorArgs()
    {
      Dictionary<string, string> newAvailableSearchIndexesDictionary = new Dictionary<string, string>();

      foreach (var index in _AvailableSearchIndexesDictionary)
      {
        foreach (var availableSearchIndex in _AvailableSearchIndexesList) 
        {
          if (availableSearchIndex == index.Value)
          {
            newAvailableSearchIndexesDictionary.Add(index.Key, index.Value);
          }
        }
      }

      return new InstallSearchIndexesArgs(Instance, newAvailableSearchIndexesDictionary, SolrUrl, SolrVersion, SolrFolder, Headers, AuthCookies);
    }

  }
}
