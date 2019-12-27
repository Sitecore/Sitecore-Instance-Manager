using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Sitecore9Installer
{
  public enum InstallParamType { String, Bool }

  public class InstallParam
  {
    public event EventHandler<ParamValueUpdatedArgs> ParamValueUpdated;
    string value;
    public InstallParam(string name, string value, string type = "string")
    {
      this.name = name;
      this.Value = value;
      this.Type = ParseInstallParamType(type);
    }

    private InstallParamType ParseInstallParamType(string type)
    {
      switch (type?.ToLower())
      {
        case "bool":
          {
            return InstallParamType.Bool;
          }
        default:
          {
            return InstallParamType.String;
          }
      }
    }

    private string name;
    public string Name { get => this.name; }
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
        if (this.ParamValueUpdated != null)
        {
          ParamValueUpdatedArgs args = new ParamValueUpdatedArgs(oldValue);

          this.ParamValueUpdated(this, args);
        }
      }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public InstallParamType Type { get; set; }
    public string Description { get; set; }
    public virtual string GetParameterValue()
    {
      if (Type == InstallParamType.Bool)
      {
        return GetBoolValue(Value);
      }
      else
      {
        return GetStringValue(Value);
      }     
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

  public class ParamValueUpdatedArgs : EventArgs
  {
    public ParamValueUpdatedArgs(string oldValue)
    {
      this.OldValue = oldValue;
    }

    public string OldValue { get; }
  }
}
