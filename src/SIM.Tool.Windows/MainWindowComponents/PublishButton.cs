namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  [UsedImplicitly]
  public class PublishButton : IMainWindowButton
  {
    #region Constants

    private const string CancelOption = "Cancel, don't publish";
    private const string IncrementalOption = "Incremental, only recent changes";
    private const string RepublishOption = "Republish, entire site (slow)";
    private const string SmartOption = "Smart, entire site";

    #endregion

    #region Fields

    protected string Mode;

    #endregion

    #region Constructors

    public PublishButton()
    {
      this.Mode = null;
    }

    public PublishButton(string mode)
    {
      this.Mode = mode;
    }

    #endregion

    #region Public methods

    public static void PublishSite(InstallWizardArgs args)
    {
      MainWindowHelper.RefreshInstances();
      var instance = InstanceManager.GetInstance(args.InstanceName);
      new PublishButton().OnClick(MainWindow.Instance, instance);
    }

    public static void PublishSite(InstallModulesWizardArgs args)
    {
      new PublishButton().OnClick(MainWindow.Instance, args.Instance);
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

        var modeText = this.GetMode(mainWindow);

        if (modeText == null || modeText == CancelOption)
        {
          return;
        }

        var mode = this.ParseMode(modeText);
        MainWindowHelper.Publish(instance, mainWindow, mode);
      }
    }

    #endregion

    #region Private methods

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

    #endregion
  }
}