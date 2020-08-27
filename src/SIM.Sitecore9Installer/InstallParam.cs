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
    private bool isGlobal;
    public InstallParam(string name, string value, bool isGlobal, InstallParamType type)
    {
      Initialize(name, value, isGlobal);
      this.Type = type;
    }

    private void Initialize(string name, string value, bool isGlobal)
    {
      this.name = name;
      this.value = value;
      this.isGlobal = isGlobal;
    }

    [JsonConstructor]
    public InstallParam(string name, string value, bool isGlogal, string type)
    {
      this.Initialize(name, value, isGlobal);
      this.Type = this.ParseInstallParamType(type);
    }

    private string name;
    public string Name { get => this.name; }
    public bool IsGlobal { get=>this.isGlobal; }
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

    private InstallParamType ParseInstallParamType(string type)
    {
      switch (type?.ToLower())
      {
        case "bool":
          {
            return InstallParamType.Bool;
          }
        case "string":
          {
            return InstallParamType.String;
          }
        case "int":
          {
            return InstallParamType.Int;
          }
        default:
          {
            throw new ArgumentOutOfRangeException($"Unknown parameter type '{type}'");
          }
      }
    }
  }

}
