using SIM.Core;
using SIM.Instances;
using SIM.Tool.Base.Wizards;
using Sitecore.Diagnostics.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

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

        foreach (XmlElement node in solrIndexes)
        {
          string coreName = GetCoreName(node);
        }

        string solrUrl = GetUrlFrom(sitecoreWebResultConfig);
      }
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
  }
}
