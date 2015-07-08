using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Instances.RuntimeSettings
{
  public class RuntimeSettingsManager
  {
    public static RuntimeSettingsAccessor GetRealtimeSettingsAccessor(Instance instance)
    {
      return new RuntimeSettingsAccessor(instance);
    }

    public static RuntimeSettingsAccessor GetCachingSettingsAccessor(Instance instance)
    {
      return new RuntimeSettingsAccessor(instance);
    }

  }
}
