using SIM.Instances;
using SIM.Tool.Base.Wizards;
using Sitecore.Diagnostics.Base;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml;
using Newtonsoft.Json;
using SIM.Tool.Windows.MainWindowComponents.Helpers;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  public class AddSolrCoresButton : InstanceOnlyButton
  {
    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {

        SolrStateResolver solrStateResolver = new SolrStateResolver();
        ButtonAuthenticationHelper buttonAuthenticationHelper = new ButtonAuthenticationHelper();
        XmlDocument sitecoreWebResultConfig = instance.GetWebResultConfig();
        string solrUrl = GetUrlFrom(sitecoreWebResultConfig);
        
        if (!solrStateResolver.IsSolrAvailable(solrUrl))
        {
          WindowHelper.ShowMessage($"Failed to access Solr at the following URL: {solrUrl}.\r\n Solr is not available.\r\nPlease make sure Solr is running and accessible.",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);

          return;
        }

        string solrAttribute = "solr_home";
        string solrFolder = solrStateResolver.GetSeparateSolrAttribute(solrUrl, solrAttribute);
        string solrVersion = solrStateResolver.GetVersion(solrUrl);
        XmlNodeList existingSolrCores = GetExistingCoresFromSolr(solrUrl);
        Dictionary<string, string> indexDataDictionary = GetindexDataListFromConfig(sitecoreWebResultConfig);
        Dictionary<string, string> availableSearchIndexesDictionary = new Dictionary<string, string>();

        Dictionary<string, string> headers = null;
        string authCookie = null;

        int.TryParse(instance.Product.ShortVersion, out int sitecoreVersion);

        if (sitecoreVersion >= 91)
        {
          headers = buttonAuthenticationHelper.GetIdServerAuthToken(mainWindow, instance);

          if (headers == null)
          {
            return;
          }
        }
        else if (sitecoreVersion == 90)
        {
          string instanceUri = instance.GetUrl();

          authCookie = buttonAuthenticationHelper.GetAuthCookie(mainWindow, instance);

          if (string.IsNullOrEmpty(authCookie))
          {
            return;
          }
        }

        foreach (var indexData in indexDataDictionary)
        {
          string coreName;
          string coreID;

          if (!string.IsNullOrEmpty(indexData.Value) && indexData.Value != "$(id)")
          {
            coreID = indexData.Key;
            coreName = indexData.Value;            
          }
          else
          {
            coreID = indexData.Key;
            coreName = indexData.Key;
          }
          
         bool itemMatched = false;

          foreach (XmlElement existingSolrnode in existingSolrCores)
          {
            string existingCoreName = existingSolrnode["name"].InnerText;
            if (coreName == existingCoreName)
            {
              itemMatched = true;
              break;
            }
          }
          
          if (!itemMatched)
          {
            availableSearchIndexesDictionary.Add(coreID, coreName);            
          }
        }

        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("installSearchIndexes", mainWindow, null, null, ignore => MainWindowHelper.MakeInstanceSelected(id), () => new InstallSearchIndexesWizardArgs(instance, availableSearchIndexesDictionary, solrUrl, solrVersion, solrFolder, headers, authCookie));
      }
    }

    private Dictionary<string, string> GetindexDataListFromConfig(XmlDocument sitecoreConfig)
    {
      Dictionary<string, string> indexData = new Dictionary<string, string>();

      foreach (XmlNode indexNode in sitecoreConfig.SelectNodes("//sitecore/contentSearch/configuration/indexes/index"))
      {
        string indexId = indexNode.Attributes["id"]?.Value;
        string coreName = indexNode.SelectSingleNode("param[@desc='core']")?.InnerText;

        if (!string.IsNullOrEmpty(indexId) && !string.IsNullOrEmpty(coreName))
        {
          indexData.Add(indexId, coreName);
        }
      }

      return indexData;
    }

    private XmlNodeList GetExistingCoresFromSolr(string solrUrl)
    {
      var solrInfoUrl = $"{solrUrl}/admin/cores?action=STATUS&wt=json";
      var doc = GetXmlDocumenFromSolrResponse(solrInfoUrl);
      XmlNodeList solrIndexes = doc.SelectNodes("//status/*");
      return solrIndexes;
    }         

    private static string GetUrlFrom(XmlDocument sitecoreConfig)
    {
      string connectionStringName = "solr.search";
      XmlNode connectionStringNode = sitecoreConfig.SelectSingleNode("//connectionStrings/add[@name='" + connectionStringName + "']");
      Assert.IsNotNull(connectionStringNode, "ConnectionString with name '" + connectionStringName + "' not found.");      
      return connectionStringNode.Attributes["connectionString"].Value;
    }

    private XmlDocumentEx GetXmlDocumenFromSolrResponse(string solrInfoUrl)
    {
     Stream response = RequestAndGetResponseStream(solrInfoUrl);
      string responseAsString = GetStringFromStream(response);
      string xmlString = ReturnXml(responseAsString);
      var doc = XmlDocumentEx.LoadXml(xmlString);
      return doc;
    }

    private Stream RequestAndGetResponseStream(string url)
    {
      return WebRequestHelper.RequestAndGetResponse(url).GetResponseStream();
    }

    private string GetStringFromStream(Stream stream)
    {
      using (var reader = new StreamReader(stream))
      {
        return reader.ReadToEnd();
      }
    }

    private string ReturnXml(string jsonObject)
    {
      string xmlString = JsonConvert.DeserializeXmlNode(jsonObject, "response").OuterXml;
      return xmlString;     
    }
  }
}
