using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Sitecore9Installer
{
  public class PackageMapping
  {
    private Dictionary<string, string> mapping = new Dictionary<string, string>();
    public PackageMapping(JToken map)
    {
      foreach (JProperty param in map.Children())
      {
        mapping.Add(param.Name.ToLowerInvariant(), param.Value.ToString().ToLowerInvariant());
      }
    }

    public string Map(string taskName, string filesRoot)
    {
      string pack;
      if(!this.mapping.TryGetValue(taskName.ToLowerInvariant(), out pack))
      {
        return string.Empty;
      }

      pack = Directory.GetFiles(filesRoot, pack).FirstOrDefault();
      if (string.IsNullOrEmpty(pack))
      {
        throw new FileNotFoundException($"Package file for {taskName} task cannot be found.");
      }

      return pack;
    }
  }
}
