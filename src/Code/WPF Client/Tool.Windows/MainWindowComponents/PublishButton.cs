using System;
using System.Windows;
using SIM.Base;
using SIM.Instances;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Plugins;

namespace SIM.Tool.Windows.MainWindowComponents
{
  public class PublishButton : IMainWindowButton
  {
    const string CancelOption = "Cancel, don't publish";
    const string IncrementalOption = "Incremental, only recent changes";
    const string SmartOption = "Smart, entire site";
    const string RepublishOption = "Republish, entire site (slow)";

    protected string Mode;
    
    public PublishButton()
    {
      this.Mode = null;
    }

    public PublishButton(string mode)
    {
      this.Mode = mode;
    }

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      using (new ProfileSection("Publish", this))
      {
        ProfileSection.Argument("mainWindow", mainWindow);
        ProfileSection.Argument("instance", instance);

        var modeText = GetMode(mainWindow);

        if (modeText == null || modeText == CancelOption)
        {
          return;
        }

        var mode = ParseMode(modeText);
        MainWindowHelper.Publish(instance, mainWindow, mode);
      }
    }

    private string GetMode(Window mainWindow)
    {
      if (string.IsNullOrEmpty(this.Mode))
      {
        var options = new[]
        {
          CancelOption,
          IncrementalOption,
          SmartOption,
          RepublishOption
        };

        return WindowHelper.AskForSelection("Publish", "Publish", "Choose publish mode", options, mainWindow, IncrementalOption);
      }

      return this.Mode;
    }

    private PublishMode ParseMode(string result)
    {
      if (result.StartsWith("Incremental", StringComparison.OrdinalIgnoreCase))
      {
        return PublishMode.Incremental;
      }

      if (result.StartsWith("Smart", StringComparison.OrdinalIgnoreCase))
      {
        return PublishMode.Smart;
      }

      if (result.StartsWith("Republish", StringComparison.OrdinalIgnoreCase))
      {
        return PublishMode.Republish;
      }

      throw new NotSupportedException(result + " is not supported publish mode");
    }

    public static void PublishSite(InstallWizardArgs args)
    {
      MainWindowHelper.RefreshInstances();
      var instance = InstanceManager.GetInstance(args.InstanceName);
      new PublishButton().OnClick(MainWindow.Instance, instance);
    }

    /// <summary>
    /// Opens solution.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    public static void PublishSite(InstallModulesWizardArgs args)
    {
      new PublishButton().OnClick(MainWindow.Instance, args.Instance);
    }
  }
}
