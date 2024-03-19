using Newtonsoft.Json.Linq;
using SIM.Core;
using SIM.Instances;
using SIM.Pipelines.Install.Modules;
using SIM.Tool.Base.Wizards;
using Sitecore.Diagnostics.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using Newtonsoft.Json;
using System.Data.SqlTypes;
using System.Net.Http;
using System.Net.NetworkInformation;


namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  public class AddIndexesButton : InstanceOnlyButton
  {
    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        XmlDocument sitecoreWebResultConfig = instance.GetWebResultConfig();
        XmlNodeList solrIndexes = GetSolrIndexNodesFrom(sitecoreWebResultConfig);
        string solrUrl = GetUrlFrom(sitecoreWebResultConfig);
        string home = GetSolrHome(solrUrl);
        XmlNodeList existingSolrCores = GetExistingCoresFromSolr(solrUrl);

        foreach (XmlElement node in solrIndexes)
        {
          string coreName = GetCoreName(node);
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
            CreateSolrCore(solrUrl, coreName);
          }
        }
      }
    }

    private void CreateSolrCore(string solrUrl, string coreName)
    {
      string test = coreName;
    }

    private XmlNodeList GetExistingCoresFromSolr(string solrUrl)
    {
      var solrInfoUrl = $"{solrUrl}/admin/cores?action=STATUS&wt=json";
      //Stream response = RequestAndGetResponseStream(url);
      //string responseAsString = GetStringFromStream(response);
      //string xmlString = ReturnXml(responseAsString);
      //var doc = XmlDocumentEx.LoadXml(xmlString);
      var doc = GetXmlDocumenFromSolrResponse(solrInfoUrl);
      XmlNodeList solrIndexes = doc.SelectNodes("//status/*");
      return solrIndexes;
    }         

    private static string GetCoreName(XmlElement node)
    {
      var coreElement = node.SelectSingleNode("param[@desc='core']") as XmlElement;
      string id = node.Attributes["id"].InnerText;
      coreElement = Assert.IsNotNull(coreElement, "param[@desc='core'] not found in Solr configuration file");
      string coreName = coreElement.InnerText.Replace("$(id)", id);
      return coreName;
    }

    private static XmlNodeList GetSolrIndexNodesFrom(XmlDocument sitecoreConfig)
    {
      return sitecoreConfig.SelectNodes(
        "//sitecore/contentSearch/configuration/indexes/index[@type='Sitecore.ContentSearch.SolrProvider.SolrSearchIndex, Sitecore.ContentSearch.SolrProvider']");
    }

    private static string GetUrlFrom(XmlDocument sitecoreConfig)
    {
      string connectionStringName = "solr.search";
      XmlNode connectionStringNode = sitecoreConfig.SelectSingleNode("//connectionStrings/add[@name='" + connectionStringName + "']");
      Assert.IsNotNull(connectionStringNode, "ConnectionString with name '" + connectionStringName + "' not found.");      
      return connectionStringNode.Attributes["connectionString"].Value;
    }

    private string GetSolrHome(string url)
    {
      string solrInfoUrl = $"{url}/admin/info/system";
      var doc = GetXmlDocumenFromSolrResponse(solrInfoUrl);
      return ReturnSolrHome(doc);
    }

    private XmlDocumentEx GetXmlDocumenFromSolrResponse(string solrInfoUrl)
    {
     Stream response = RequestAndGetResponseStream(solrInfoUrl);
      string responseAsString = GetStringFromStream(response);
      string xmlString = ReturnXml(responseAsString);
      var doc = XmlDocumentEx.LoadXml(xmlString);
      return doc;
    }

    private string ReturnSolrHome(XmlDocumentEx xml)
    {
      XmlNode solrHomeNode = xml.SelectSingleNode("/response/solr_home");
      Assert.IsNotNull(solrHomeNode, "solr_home element should not be null");
     string solrHomeValue = solrHomeNode.InnerText;
     return solrHomeValue.Replace("\\\\", "\\");
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

    private string NormalizeXml(XmlDocumentEx xml)
    {
      string outerXml = xml.OuterXml;
      string formatted = XmlDocumentEx.Normalize(outerXml);
      Regex r = new Regex(@"^<\?.*\?>");
      string corrected = r.Replace(formatted, @"<?xml version=""1.0"" encoding=""UTF-8"" ?>");  //Solr requires UTF-8.
      return corrected;
    }

    private string ReturnXml(string jsonObject)
    {
      string xmlString = JsonConvert.DeserializeXmlNode(jsonObject, "response").OuterXml;
      return xmlString;     
    }
  }
}
