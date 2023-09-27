using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using Newtonsoft.Json;

namespace SIM
{
  public class SolrStateResolver
  {
    public virtual SolrState.CurrentState GetServiceState(ServiceControllerWrapper service)
    {
      if (service != null)
      {
        if (service.Status == ServiceControllerStatus.Running)
        {
          return SolrState.CurrentState.Running;
        }
      }

      return SolrState.CurrentState.Stopped;
    }

    public virtual ServiceControllerWrapper GetService(string solrServiceName)
    {
      // Windows services' names are case insensitive
      ServiceController serviceController = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName.ToLowerInvariant() == solrServiceName.ToLowerInvariant());
      if (serviceController == null)
      {
        return null;
      }

      return new ServiceControllerWrapper(serviceController);
    }

    public virtual SolrState.CurrentState GetUrlState(string solrUrl)
    {
      HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(solrUrl);
      HttpWebResponse httpWebResponse;
      try
      {
        httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
      }
      catch
      {
        return SolrState.CurrentState.Stopped;
      }

      if (httpWebResponse.StatusCode != HttpStatusCode.OK)
      {
        return SolrState.CurrentState.Stopped;
      }

      return SolrState.CurrentState.Running;
    }

    public virtual string GetVersion(string solrUrl)
    {
      HttpClient client = new HttpClient();

      using (Stream stream = client.GetStreamAsync($"{solrUrl}/admin/info/system?wt=json").Result)
      using (StreamReader streamReader = new StreamReader(stream))
      using (JsonReader reader = new JsonTextReader(streamReader))
      {
        while (reader.Read())
        {
          if (string.Equals(reader.Path, "lucene.solr-spec-version", StringComparison.OrdinalIgnoreCase)
              && !string.Equals((string)reader.Value, "solr-spec-version", StringComparison.OrdinalIgnoreCase))
          {
            return (string)reader.Value;
          }
        }
      }

      return string.Empty;
    }
  }

  public class ServiceControllerWrapper
  {
    ServiceController _service;

    public ServiceControllerWrapper(ServiceController service)
    {
      this._service = service;
    }

    public virtual ServiceControllerStatus Status => this._service.Status;

    public virtual string ServiceName => this._service.ServiceName;
  }
}
