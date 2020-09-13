using Sitecore.Diagnostics.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM
{
  [Serializable]
  public class SolrDefinition: ICloneable, IValidateable
  {
    string url;
    string root;
    string name;

    [RenderInDataGreed]
    public string Name {
      get
      {
        return this.name;
      }
      set
      {
        name = value;
      }
    }
    
    [RenderInDataGreed]
    public string Url {
      get
      {
        return url;
      }
      set
      {
        url = value.Trim().TrimEnd('/','#');
      }
    }
    [RenderInDataGreed]
    public string Root
    {
      get
      {
        return root;
      }
      set
      {
        root = value;
      }
    }
    [RenderInDataGreed]
    public string Service { get; set; }

    public object Clone()
    {
      return new SolrDefinition()
      {
        root = Root,
        name = Name,
        url = Url,
        Service = Service
      };
    }

    public bool HasAnyValuesInTheFields()
    {
      if (string.IsNullOrEmpty(this.Name) && string.IsNullOrEmpty(this.Root) && string.IsNullOrEmpty(this.Service) &&
          string.IsNullOrEmpty(this.Url))
      {
        return false;
      }

      return true;
    }

    public string ValidateAndGetError()
    {
      if (string.IsNullOrWhiteSpace(this.Name) || string.IsNullOrWhiteSpace(this.Url) ||
          string.IsNullOrWhiteSpace(this.Root))
      {
        return "Name, Root and Url must not be empty.";
      }

      Uri uri;

      try
      {
        uri = new Uri(this.Url, UriKind.Absolute);
      }
      catch (Exception)
      {
        return "Invalid solr URL.";
      }

      if (!uri.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase))
      {
        return "Solr must be HTTPS";
      }

      if (!Directory.Exists(this.Root))
      {
        return "Solr root path does not exist.";
      }

      return string.Empty;
    }
  }
}
