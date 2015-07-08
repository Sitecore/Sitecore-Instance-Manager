using System.Windows;
using SIM.Instances;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Plugins;

namespace SIM.Tool.Windows.MainWindowComponents
{
  public class LoginAdminButton : IMainWindowButton
  {
    protected readonly string VirtualPath;
    protected readonly string Browser;

    public LoginAdminButton()
    {
      this.VirtualPath = string.Empty;
      this.Browser = string.Empty;
    }

    public LoginAdminButton(string param)
    {
      var arr = (param + ":").Split(':');
      this.VirtualPath = arr[0];
      this.Browser = arr[1];
    }

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      InstanceHelperEx.OpenInBrowserAsAdmin(instance, mainWindow, this.VirtualPath, this.Browser);
    }

    public static void FinishAction(InstallWizardArgs args)
    {
      InstanceHelperEx.OpenInBrowserAsAdmin(args.Instance, MainWindow.Instance);
    }

    public static void FinishAction(InstallModulesWizardArgs args)
    {
      InstanceHelperEx.OpenInBrowserAsAdmin(args.Instance, MainWindow.Instance);
    }    
  }
}