using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Sitecore9Installer.Events
{
  public class ParamValueUpdatedArgs : EventArgs
  {
    public ParamValueUpdatedArgs(string oldValue)
    {
      this.OldValue = oldValue;
    }

    public string OldValue { get; }
  }
}
