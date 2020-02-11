using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SIM.Sitecore9Installer.Validation
{
  public class ValidatorDefinition
  {
    [JsonProperty]
    public string Name { get; private set; }
    [JsonProperty]
    public string Type { get; private set; }
    [JsonProperty]
    public Dictionary<string,string> Data { get; private set; }
  }
}
