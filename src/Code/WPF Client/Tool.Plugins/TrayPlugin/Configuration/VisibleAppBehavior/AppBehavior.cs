using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Tool.Plugins.TrayPlugin.Configuration.VisibleAppBehavior
{
  public abstract class AppBehavior
  {
    public abstract void Attach();
    public abstract void Detach();
  }
}
