namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.Diagnostics;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class AttachDebuggerButton : IMainWindowButton
  {
    #region Fields

    private static readonly AdvancedProperty<string> WinDbgCommand = AdvancedSettings.Create("App/WinDbg/Command", "!symfix C:\\symbols;.loadby sos clr;.reload");

    #endregion

    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      var ids = instance.ProcessIds.ToArray();
      if (ids.Length == 0)
      {
        WindowHelper.HandleError("No running w3wp processes for this Sitecore instance", false);
        return;
      }

      var path = instance.Is32Bit ? @"Debugging Tools for Windows (x86)\windbg.exe" : @"Windows Kits\8.1\Debuggers\x64\windbg.exe";
      foreach (var id in ids)
      {
        var options = "-p " + id.ToString(CultureInfo.InvariantCulture) + " -c \"" + WinDbgCommand.Value + "\"";
        Process.Start(new ProcessStartInfo(Environment.ExpandEnvironmentVariables(Path.Combine("%programfiles(x86)%", path)), options));
      }
    }

    #endregion
  }
}