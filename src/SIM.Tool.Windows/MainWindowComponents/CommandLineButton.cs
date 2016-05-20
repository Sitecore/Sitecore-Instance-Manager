namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Diagnostics;
  using System.Windows;
  using SIM.Tool.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class CommandLineButton : WindowOnlyButton
  {
    protected override void OnClick([CanBeNull] Window mainWindow)
    {
      var start = new ProcessStartInfo("cmd.exe")
      {
        Arguments = "/K echo %cd%^>sim & sim",
        UseShellExecute = true
      };

      WindowHelper.RunApp(start);
    }
  }
}
