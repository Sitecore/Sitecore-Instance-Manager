using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace SIM.Tool.Windows
{
  public class ButtonsConfiguration
  {
    private readonly JObject _settings;
    private static ButtonsConfiguration _instance;

    internal ButtonsConfiguration()
    {
      string path = Path.Combine(Directory.GetCurrentDirectory(),
        "ButtonsConfiguration/ButtonsConfiguration.json");
      if (File.Exists(path))
      {
        this._settings = JObject.Parse(File.ReadAllText(path));
      }
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

    private List<string> GetButtonsList(string jsonGroupName)
    {
      if (this._settings == null)
      {
        return new List<string>();
      }
      return this._settings[jsonGroupName].ToObject<List<string>>();
    }

    public IEnumerable<string> Sitecore8AndEarlierButtons => 
      this.GetButtonsList("Sitecore8AndEarlierButtons");

    public IEnumerable<string> Sitecore9AndLaterButtons =>
      this.GetButtonsList("Sitecore9AndLaterButtons");

    public IEnumerable<string> Sitecore9AndLaterMemberButtons =>
      this.GetButtonsList("Sitecore9AndLaterMemberButtons");

    public IEnumerable<string> SitecoreContainersButtons =>
      this.GetButtonsList("SitecoreContainersButtons");

    public IEnumerable<string> Sitecore8AndEarlierGroups =>
      this.GetButtonsList("Sitecore8AndEarlierGroups");

    public IEnumerable<string> Sitecore9AndLaterGroups =>
      this.GetButtonsList("Sitecore9AndLaterGroups");

    public IEnumerable<string> Sitecore9AndLaterMemberGroups =>
      this.GetButtonsList("Sitecore9AndLaterMemberGroups");

    public IEnumerable<string> SitecoreContainersGroups =>
      this.GetButtonsList("SitecoreContainersGroups");
  }
}