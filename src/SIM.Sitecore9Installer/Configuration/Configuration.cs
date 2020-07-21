using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace SIM.Sitecore9Installer.Configuration
{
  public class Configuration
  {
    private JObject settingsDoc;
    #region Singleton
    private static Configuration instance;
    private Configuration()
    {
      string globalSettings = string.Empty;
      using (StreamReader reader =
        new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "GlobalParamsConfig/GlobalSettings.json")))
      {
        globalSettings = reader.ReadToEnd();
      }

      this.settingsDoc = JObject.Parse(globalSettings);
      
    }
    public static Configuration Instance
    {
      get
      {
        if (instance == null)
        {
          instance = new Configuration();
        }

        return instance;
      }
    }
    #endregion

    public Dictionary<string, string> SolrMap
    {
      get
      {
        return this.settingsDoc["SolrVersionsMap"].ToObject<Dictionary<string, string>>();
      }
    }

    public Dictionary<string,string> GlobalFilesMap
    {
      get
      {
        return this.settingsDoc["GlobalFilesMap"].ToObject<Dictionary<string, string>>();
      }
    }

    public IEnumerable<string> ExcludedUninstallParams
    {
      get
      {
        return settingsDoc["ExcludeUninstallParams"].Values<string>();
      }
    }
  }
}
