using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerInstaller
{
  public class EnvModel : IEnumerable<KeyValuePair<string, string>>
  {
    private Dictionary<string, string> variables = new Dictionary<string, string>();

    #region Env variables properties
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
    public string this[string Name]
    {
      get
      {
        return this.variables[Name];
      }
      set
      {
        this.variables[Name] = value;
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
    #endregion
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
      return ((IEnumerable<KeyValuePair<string, string>>)variables).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<KeyValuePair<string, string>>)variables).GetEnumerator();
    }

    public void SaveToFile(string path)
    {
      using(StreamWriter writer=new StreamWriter(path))
      {
        foreach(KeyValuePair<string,string> pair in this)
        {
          writer.WriteLine(string.Format("{0}={1}", pair.Key, pair.Value));
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
          string key = line.Substring(0, num);
          string value = line.Substring(num + 1);
          model[key] = value;
          line = reader.ReadLine();
        }
      }

      return model;
    }
  }
}
