using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SIM.Tool.Plugins.TrayPlugin.Resourcing
{
  public class MultisourceResourcesManager
  {
    private static ResourcesProvider actualProvider;

    public static ResourcesProvider ActualProvider
    {
      //Lazy to allow other principals to override it.
      get
      {
        if (actualProvider != null)
          return actualProvider;
        actualProvider = new ResourcesProvider();
        return actualProvider;
      }
      set { actualProvider = value; }
    }

    public static Icon GetIconResource(string iconResourceName, Icon defaultValue)
    {
      return ActualProvider.GetIconResource(iconResourceName, defaultValue);
    }

    public static string GetStringResource(string stringResourceName, string defaultValue)
    {
      return ActualProvider.GetStringResource(stringResourceName, defaultValue);
    }
  }
}
