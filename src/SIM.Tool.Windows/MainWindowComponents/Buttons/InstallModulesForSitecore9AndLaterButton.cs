using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Windows.UserControls.Helpers;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  [UsedImplicitly]
  public class InstallModulesForSitecore9AndLaterButton : InstanceOnlyButton
  {
    public override void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        if (instance.Type == Instance.InstanceType.Sitecore9AndLater)
        {
          int sitecoreVersion;
          int.TryParse(instance.Product.ShortVersion, out sitecoreVersion);
          if (sitecoreVersion >= 91)
          {
            string sitecoreIdServerUri = "https://" + instance.SitecoreEnvironment.Name + "Id.local";
            string authToken = null;
            var task = Task.Run(async () => {
              authToken = await
                SitecoreIdServerAuth.GetToken(sitecoreIdServerUri, "SitecorePassword", "SIF-Default", "password", "sitecore\\admin", "b");
            });
            task?.Wait();

            WebRequestHelper.AuthToken = authToken;
          }
        }

        var id = MainWindowHelper.GetListItemID(instance.ID);
        WizardPipelineManager.Start("installmodules", mainWindow, null, null, ignore => MainWindowHelper.MakeInstanceSelected(id), () => new InstallModulesWizardArgs(instance));

        WebRequestHelper.AuthToken = null;
      }
    }
  }
}
