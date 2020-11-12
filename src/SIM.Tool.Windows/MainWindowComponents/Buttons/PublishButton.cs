using System;
using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Tool.Base;
using Sitecore.Diagnostics.Logging;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class PublishButton : InstanceOnlyButton
  {
    #region Constants

    private const string CancelOption = "Cancel, don't publish";
    private const string IncrementalOption = "Incremental, only recent changes";
    private const string RepublishOption = "Republish, entire site (slow)";
    private const string SmartOption = "Smart, entire site";

    #endregion

    #region Fields

    protected string _Mode;

    #endregion

    #region Constructors

    public PublishButton()
    {
      _Mode = null;
    }

    public PublishButton(string mode)
    {
      _Mode = mode;
    }

    #endregion

    #region Public methods

    public override void OnClick(Window mainWindow, Instance instance)
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

        var mode = ParseMode(modeText);
        MainWindowHelper.Publish(instance, mainWindow, mode);
      }
    }

    #endregion

    #region Private methods

    private string GetMode(Window mainWindow)
    {
      if (string.IsNullOrEmpty(_Mode))
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

      return _Mode;
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