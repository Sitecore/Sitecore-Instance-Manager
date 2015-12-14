namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Diagnostics;
  using System.Globalization;
  using System.Linq;
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class ManagedArgsTracerButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, "mainWindow");

      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, "mainWindow");

      if (instance == null)
      {
        Run(mainWindow, string.Empty);
        return;
      }

      var ids = (instance.ProcessIds ?? new int[0]).ToArray();
      if (ids.Length == 0)
      {
        WindowHelper.HandleError("No running w3wp processes for this Sitecore instance", false);
        return;
      }

      foreach (var id in ids)
      {
        var defaultValue = id.ToString(CultureInfo.InvariantCulture);
        if (Run(mainWindow, defaultValue))
        {
          return;
        }
      }
    }

    #endregion

    #region Private methods

    private static bool Run(Window mainWindow, string defaultValue)
    {
      var options = WindowHelper.Ask("Please specify params for Managed Args Tracer", defaultValue, mainWindow);
      if (string.IsNullOrEmpty(options))
      {
        return true;
      }

      Process.Start(new ProcessStartInfo("cmd.exe", "/K \"" + ApplicationManager.GetEmbeddedFile("ManagedArgsTracer.zip", "SIM.Tool.Windows", "ManagedArgsTracer.exe") + " " + options + "\""));
      return false;
    }

    #endregion
  }
}