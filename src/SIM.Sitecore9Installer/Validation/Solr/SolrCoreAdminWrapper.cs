using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SIM.Sitecore9Installer.Validation.Solr
{
  public class SolrCoreAdminWrapper:IDisposable
  {
    private static readonly HttpClient client = new HttpClient();

    private string solrUrl;

    public SolrCoreAdminWrapper():this("http://localhost:8983/solr")
    {

    }

    public SolrCoreAdminWrapper(string url)
    {
      solrUrl = url;
    }

    public XmlDocument Status()
    {
    //  client.GetAsync();
      return new XmlDocument();
    }

    public bool Status(string names)
    {
      return true;
    }

    public int Unload(string name, string command)
    {
      return 1;
    }

    public List<string> GetCoreNamesFromResponse()
    {
      return new List<string>();
    }

    public List<string> GetCoreNamesFromResponse(XmlDocument response)
    {
      return new List<string>();
    }


    private void CloseConnection()
    {

    }

    public void Dispose()
    {
       this.CloseConnection();
    }
  }
}
