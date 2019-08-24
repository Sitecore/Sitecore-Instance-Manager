using Sitecore.Diagnostics.Base;
using System;
using System.Collections.Generic;
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
    
    public string Url {
      get
      {
        return url;
      }
      set
      {
        url = value;
      }
    }
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

    public string ValidateAndGetError()
    {
      if (string.IsNullOrWhiteSpace(this.Name) || string.IsNullOrWhiteSpace(this.Url) ||
          string.IsNullOrWhiteSpace(this.Root))
      {
        return "Name, Root and Url must not be empty.";
      }

      return string.Empty;
    }
  }
}
