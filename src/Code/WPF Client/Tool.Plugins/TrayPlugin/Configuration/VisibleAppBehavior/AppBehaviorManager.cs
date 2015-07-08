using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Tool.Plugins.TrayPlugin.Configuration.VisibleAppBehavior
{
  public static class AppBehaviorManager
  {
    public static AppBehavior ActualBehavior { get; private set; }

    public static void Initialize()
    {
      ActualBehavior = new DefaultAppBehavior();
      ActualBehavior.Attach();
    }

    public static void ChangeBehavior(AppBehavior newBehavior)
    {
      if (ActualBehavior == newBehavior)
        return;
      if(ActualBehavior != null)
        ActualBehavior.Detach();
      ActualBehavior = newBehavior;
      ActualBehavior.Attach();
    }
  }
}
