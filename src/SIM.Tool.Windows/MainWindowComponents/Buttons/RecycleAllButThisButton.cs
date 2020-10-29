using System;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;
using Sitecore.Diagnostics.Base;
using Sitecore.Diagnostics.Logging;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class RecycleAllButThisButton : InstanceOnlyButton
  {
    #region Public methods

    public override void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, nameof(mainWindow));

      var instances = InstanceManager.Default.PartiallyCachedInstances?.Values;
      Assert.IsNotNull(instances, nameof(instances));

      var otherInstances = instances.Where(x => x.ID != instance.ID);
      foreach (var otherInstance in otherInstances)
      {
        try
        {
          if (otherInstance == null)
          {
            continue;
          }

          Log.Info($"Recycling instance {otherInstance}");
          otherInstance.Recycle();
        }
        catch (Exception ex)
        {
          Log.Warn(ex, "An error occurred");
        }
      }
    }

    #endregion
  }
}