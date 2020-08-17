using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Sitecore9Installer.Events
{
  public class EventManager
  {
    private static EventManager _instance=new EventManager();
    public static EventManager Instance
    {
      get => _instance;
    }

    public event EventHandler<ParamValueUpdatedArgs> ParamValueUpdated;
    public void RaiseParamValueUpdated(InstallParam sender, ParamValueUpdatedArgs args)
    {
      if (this.ParamValueUpdated != null)
      {
        this.ParamValueUpdated(sender, args);
      }
    }
  }
}
