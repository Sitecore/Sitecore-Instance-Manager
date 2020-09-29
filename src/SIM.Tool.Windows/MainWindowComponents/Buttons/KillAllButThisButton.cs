namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.Diagnostics;
  using System.Linq;
  using System.Windows;
  using SIM.Instances;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;

  [UsedImplicitly]
  public class KillAllButThisButton : InstanceOnlyButton
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

            Log.Info($"Killing process {processId}");
            process.Kill();
          }
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