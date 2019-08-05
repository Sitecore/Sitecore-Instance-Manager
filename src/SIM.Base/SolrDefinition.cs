using Sitecore.Diagnostics.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM
{
  [Serializable]
  public class SolrDefinition: ICloneable
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
        Assert.ArgumentNotNullOrEmpty(value, nameof(this.Name));
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
        Assert.ArgumentNotNullOrEmpty(value, nameof(this.Url));
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
        Assert.ArgumentNotNullOrEmpty(value, nameof(this.Root));
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
  }
}
