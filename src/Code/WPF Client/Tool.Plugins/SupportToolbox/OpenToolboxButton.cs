using System.IO;
using System.Windows;
using SIM.Base;
using SIM.Instances;
using SIM.Pipelines;
using SIM.Pipelines.InstallModules;
using SIM.Products;
using SIM.Tool.Base;
using SIM.Tool.Base.Plugins;

namespace SIM.Tool.Plugins.SupportToolbox
{
  public class OpenToolboxButton : IMainWindowButton
  {
    private readonly bool BypassSecurity;

    public OpenToolboxButton()
    {
      this.BypassSecurity = false;
    }

    public OpenToolboxButton(string param)
    {
      this.BypassSecurity = param == "bypass";
    }

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (EnvironmentHelper.CheckSqlServer())
      {
        if (instance != null)
        {
          string path = Path.Combine(instance.WebRootPath, @"sitecore\admin\toolbox");
          if (!FileSystem.Local.Directory.Exists(path))
          {
            const string packageName = "Support Toolbox\\Support Toolbox.zip";
            Product product = Product.GetFilePackageProduct(Path.Combine(ApplicationManager.StockPlugins, packageName)) ?? Product.GetFilePackageProduct(Path.Combine(ApplicationManager.FilePackagesFolder, packageName));
            if (product == null)
            {
              WindowHelper.HandleError("The " + packageName + " package cannot be found in either the .\\File Packages folder or %appdata%\\Sitecore\\Sitecore Instance Manager\\Custom Packages one", false, null, this);
              return;
            }

            PipelineManager.StartPipeline("installmodules", new InstallModulesArgs(instance, new[] { product }), isAsync: false);
          }

          if (FileSystem.Local.Directory.Exists(path))
          {
            if (this.BypassSecurity)
            {
              InstanceHelperEx.OpenInBrowserAsAdmin(instance, mainWindow, @"/sitecore/admin");
            }
            else
            {
              InstanceHelperEx.BrowseInstance(instance, mainWindow, @"/sitecore/admin", false);
            }
          }
        }
      }
    }
  }
}
