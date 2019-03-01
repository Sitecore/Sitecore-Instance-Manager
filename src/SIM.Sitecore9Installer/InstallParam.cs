using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Sitecore9Installer
{
  public class InstallParam
  {
    public event EventHandler<ParamValueUpdatedArgs> ParamValueUpdated;
    string value;
    public InstallParam(string name, string value)
    {
      this.name = name;
      this.Value = value;
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
    public string Description { get; set; }
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
