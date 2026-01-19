using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Windows.MainWindowComponents.Helpers;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class InstallModulesForSitecore9AndLaterButton : InstanceOnlyButton
  {
    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        Dictionary<string, string> headers = null;
        string authCookie = null;

        int.TryParse(instance.Product.ShortVersion, out int sitecoreVersion);
        ButtonAuthenticationHelper buttonAuthenticationHelper = new ButtonAuthenticationHelper();

        if (sitecoreVersion >= 91)
        {
          headers = buttonAuthenticationHelper.GetIdServerAuthToken(mainWindow, instance);

          if (headers == null)
          {
            return;
          }         
        }
        else if (sitecoreVersion == 90)
        {
          string instanceUri = instance.GetUrl();

          authCookie = buttonAuthenticationHelper.GetAuthCookie(mainWindow, instance);

          if (string.IsNullOrEmpty(authCookie))
          {
            return;
          }
        }

        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("installmodules", mainWindow, null, null, ignore => MainWindowHelper.MakeInstanceSelected(id), () => new InstallModulesWizardArgs(instance, authCookie, headers));
      }
    }    
  }
}