namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.Diagnostics;
  using System.Linq;
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;

  [UsedImplicitly]
  public class KillAllButThisButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, nameof(mainWindow));

      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, nameof(mainWindow));

      var instances = InstanceManager.PartiallyCachedInstances ?? InstanceManager.Instances;
      Assert.IsNotNull(instances, nameof(instances));

      var otherInstances = instances.Where(x => x.ID != instance.ID);
      foreach (var otherInstance in otherInstances)
      {
        if (otherInstance == null)
        {
          continue;
        }

        try
        {
          var processIds = otherInstance.ProcessIds;
          foreach (var processId in processIds)
          {
            var process = Process.GetProcessById(processId);

            Log.Info(string.Format("Killing process {0}",  processId));
            process.Kill();
          }
        }
        catch (Exception ex)
        {
          Log.Warn(ex, string.Format("An error occurred"));
        }
      }
    }

    #endregion
  }
}