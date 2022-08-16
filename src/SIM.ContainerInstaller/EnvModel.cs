using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace SIM.ContainerInstaller
{
  public class EnvModel : IEnumerable<NameValuePair>
  {
    private List<NameValuePair> variables = new List<NameValuePair>();

    #region Env variables properties
    public string SitecoreRegistry
    {
      get
      {
        return this["SITECORE_DOCKER_REGISTRY"];
      }
      set
      {
        this["SITECORE_DOCKER_REGISTRY"] = value;
      }
    }
    public string ProjectName
    {
      get
      {
        return this["COMPOSE_PROJECT_NAME"];
      }
      set
      {
        this["COMPOSE_PROJECT_NAME"] = value;
      }
    }

    public string SitecoreVersion
    {
      get
      {
        return this["SITECORE_VERSION"];
      }
      set
      {
        this["SITECORE_VERSION"] = value;
      }
    }

    public string SitecoreAdminPassword
    {
      get
      {
        return this["SITECORE_ADMIN_PASSWORD"];
      }
      set
      {
        this["SITECORE_ADMIN_PASSWORD"] = value;
      }
    }

    public string SqlAdminPassword
    {
      get
      {
        return this["SQL_SA_PASSWORD"];
      }
      set
      {
        this["SQL_SA_PASSWORD"] = value;
      }
    }

    public string ReportingApiKey
    {
      get
      {
        return this["REPORTING_API_KEY"];
      }
      set
      {
        this["REPORTING_API_KEY"] = value;
      }
    }

    public string TelerikKey
    {
      get
      {
        return this["TELERIK_ENCRYPTION_KEY"];
      }
      set
      {
        this["TELERIK_ENCRYPTION_KEY"] = value;
      }
    }

    public string IdServerSecret
    {
      get
      {
        return this["SITECORE_IDSECRET"];
      }
      set
      {
        this["SITECORE_IDSECRET"] = value;
      }
    }

    public string IdServerCert
    {
      get
      {
        return this["SITECORE_ID_CERTIFICATE"];
      }
      set
      {
        this["SITECORE_ID_CERTIFICATE"] = value;
      }
    }

    public string IdServerCertPassword
    {
      get
      {
        return this["SITECORE_ID_CERTIFICATE_PASSWORD"];
      }
      set
      {
        this["SITECORE_ID_CERTIFICATE_PASSWORD"] = value;
      }
    } 

    public string SitecoreLicense
    {
      get
      {
        return this["SITECORE_LICENSE"];
      }
      set
      {
        this["SITECORE_LICENSE"] = value;
      }
    }

    public string CmHost
    {
      get
      {
        return this["CM_HOST"];
      }
      set
      {
        this["CM_HOST"] = value;
      }
    }

    public string CdHost
    {
      get
      {
        return this["CD_HOST"];
      }
      set
      {
        this["CD_HOST"] = value;
      }
    }

    public string IdHost
    {
      get
      {
        return this["ID_HOST"];
      }
      set
      {
        this["ID_HOST"] = value;
      }
    }

    public const string SitecoreToolsRegistryName = "SITECORE_TOOLS_REGISTRY";

    public string SitecoreToolsRegistry
    {
      get
      {
        return this[SitecoreToolsRegistryName];
      }
      set
      {
        this[SitecoreToolsRegistryName] = value;
      }
    }

    public const string SitecoreModuleRegistryName = "SITECORE_MODULE_REGISTRY";

    public string SitecoreModuleRegistry
    {
      get
      {
        return this[SitecoreModuleRegistryName];
      }
      set
      {
        this[SitecoreModuleRegistryName] = value;
      }
    }

    public const string ToolsVersionName = "TOOLS_VERSION";

    public string ToolsVersion
    {
      get
      {
        return this[ToolsVersionName];
      }
      set
      {
        this[ToolsVersionName] = value;
      }
    }

    public const string SpeVersionName = "SPE_VERSION";

    public string SpeVersion
    {
      get
      {
        return this[SpeVersionName];
      }
      set
      {
        this[SpeVersionName] = value;
      }
    }

    public const string SxaVersionName = "SXA_VERSION";

    public string SxaVersion
    {
      get
      {
        return this[SxaVersionName];
      }
      set
      {
        this[SxaVersionName] = value;
      }
    }

    public const string HorizonHostName = "HORIZON_HOST";

    public string HorizonHost
    {
      get
      {
        return this[HorizonHostName];
      }
      set
      {
        this[HorizonHostName] = value;
      }
    }

    public const string HorizonVersionName = "HORIZON_VERSION";

    public string HorizonVersion
    {
      get
      {
        return this[HorizonVersionName];
      }
      set
      {
        this[HorizonVersionName] = value;
      }
    }

    public const string HorizonAssetsVersionName = "HORIZON_ASSETS_VERSION";

    public string HorizonAssetsVersion
    {
      get
      {
        return this[HorizonAssetsVersionName];
      }
      set
      {
        this[HorizonAssetsVersionName] = value;
      }
    }
    #endregion

    public string this[string Name]
    {
      get
      {
        NameValuePair pair = this.GetPairOrNull(Name);
        if (pair == null)
        {
          return null;
        }

        return pair.Value;
      }
      set
      {
        NameValuePair pair = this.GetPairOrNull(Name);
        if (pair == null)
        {
          pair = new NameValuePair(Name, value);
          this.variables.Add(pair);
        }
        else
        {
          pair.Value = value;
        }
      }
    }

    public IEnumerable<string> GetNames()
    {
      return this.variables.Select(p => p.Name);
    }

    public void SaveToFile(string path)
    {
      using(StreamWriter writer=new StreamWriter(path))
      {
        foreach(NameValuePair pair in this)
        {
          writer.WriteLine(string.Format("{0}={1}", pair.Name, pair.Value));
        }
      }
    }

    public static EnvModel LoadFromFile(string path)
    {
      EnvModel model = new EnvModel();
      using (StreamReader reader = new StreamReader(path))
      {
        string line = reader.ReadLine();
        while (!string.IsNullOrEmpty(line))
        {
          int num = line.IndexOf('=');

          if (num > 0)
          {
            string key = line.Substring(0, num);

            string value = line.Substring(num + 1);

            model[key] = value;
          }

          line = reader.ReadLine();
        }
      }

      return model;
    }

    public bool KeyExists([NotNull]string key)
    {
      return this.GetNames().Contains(key);
    }

    public IEnumerator<NameValuePair> GetEnumerator()
    {
      return ((IEnumerable<NameValuePair>)variables).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<NameValuePair>)variables).GetEnumerator();
    }

    private NameValuePair GetPairOrNull(string name)
    {
      return this.variables.FirstOrDefault(v => v.Name == name);
    }

    public void Remove(string name)
    {
      NameValuePair pair = this.GetPairOrNull(name);
      if (pair != null)
      {
        this.variables.Remove(pair);
      }
    }
  }
}
