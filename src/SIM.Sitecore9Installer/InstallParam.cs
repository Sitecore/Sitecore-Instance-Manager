using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SIM.Sitecore9Installer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Sitecore9Installer
{
  public enum InstallParamType { String, Bool, Int }

  public class InstallParam
  {
    string value;
    public InstallParam(string name, string value, bool isGlogal=false, string type = "string")
    {
      this.name = name;
      this.Value = value;
      this.IsGlobal = isGlogal;
      this.Type = ParseInstallParamType(type);
    }

    private InstallParamType ParseInstallParamType(string type)
    {
      switch (type?.ToLower())
      {
        case "bool":
        case "switch":
          {
            return InstallParamType.Bool;
          }
        case "int":
          {
            return InstallParamType.Int;
          }
        default:
          {
            return InstallParamType.String;
          }
      }
    }

    private string name;
    public string Name { get => this.name; }
    public bool IsGlobal { get; }
    public string Value
    {
      get
      {
        return this.value;
      }
      set
      {
        string oldValue = this.value;
        this.value = value;
        ParamValueUpdatedArgs args = new ParamValueUpdatedArgs(oldValue);
        EventManager.Instance.RaiseParamValueUpdated(this, args);
      }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public InstallParamType Type { get; set; }
    public string Description { get; set; }
    public virtual string GetParameterValue()
    {
      switch (this.Type)
      {
        case InstallParamType.Bool:
          {
            return GetBoolValue(Value);
          }       
        case InstallParamType.Int:
          {
            return GetIntValue(Value);
          }
        default:
          {
            return GetStringValue(Value);
          }
      }
    }

    private string GetIntValue(string value)
    {
      return value;
    }

    private string GetBoolValue(string value)
    {
      if (value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
      {
        return "$true";
      }
      else
      {
        return "$false";
      }
    }

    private string GetStringValue(string value)
    {
      if (!value.StartsWith("$"))
      {
        if (value.StartsWith("[") && value.EndsWith("]"))
        {
          value = value.Remove(0, 1);
          value = value.Remove(value.Length - 1, 1);
        }
        else
        {
          value = string.Format("\"{0}\"", value);
        }
      }

      return value;
    }
  }

}
