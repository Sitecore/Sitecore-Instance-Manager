using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SIM.Sitecore9Installer.Configuration;

namespace SIM.Tool.Windows
{
  public class ButtonsConfiguration
  {
    private JObject settings;
    private static ButtonsConfiguration _instance;

    internal ButtonsConfiguration()
    {
      this.settings = JObject.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(),
        "ButtonsConfiguration/ButtonsConfiguration.json")));
    }

    public static ButtonsConfiguration Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = new ButtonsConfiguration();
        }

        return _instance;
      }
    }

    public IEnumerable<string> Sitecore8AndEarlierButtons
    {
      get
      {
        return this.settings["Sitecore8AndEarlierButtons"].ToObject<List<string>>(); ;
      }
    }

    public IEnumerable<string> Sitecore9AndLaterButtons
    {
      get
      {
        return this.settings["Sitecore9AndLaterButtons"].Values<string>();
      }
    }

    public IEnumerable<string> Sitecore9AndLaterMemberButtons
    {
      get
      {
        return settings["Sitecore9AndLaterMemberButtons"].Values<string>();
      }
    }
  }
}
