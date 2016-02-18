namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.IO;
  using System.Windows;
  using SIM.Instances;
  using SIM.Pipelines;
  using SIM.Pipelines.InstallModules;
  using SIM.Products;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core;

  [UsedImplicitly]
  public class OpenToolboxButton : IMainWindowButton
  {
    #region Fields

    private const string PackageName = "Support Toolbox.zip";
    private readonly bool bypassSecurity;

    #endregion

    #region Constructors

    public OpenToolboxButton()
    {
      this.bypassSecurity = false;
    }

    public OpenToolboxButton(string param)
    {
      this.bypassSecurity = param == "bypass";
    }

    #endregion

    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (!EnvironmentHelper.CheckSqlServer())
      {
        return;
      }

      if (instance == null)
      {
        return;
      }

      var path = Path.Combine(instance.WebRootPath, @"sitecore\admin\toolbox");
      if (!FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        var product = Product.GetFilePackageProduct(Path.Combine(ApplicationManager.DefaultPackages, PackageName)) ?? Product.GetFilePackageProduct(Path.Combine(ApplicationManager.FilePackagesFolder, PackageName));
        if (product == null)
        {
          WindowHelper.HandleError("The " + PackageName + " package cannot be found in either the .\\File Packages folder or %appdata%\\Sitecore\\Sitecore Instance Manager\\Custom Packages one", false, null);
          return;
        }

        var products = new[] { product };
        var args = new InstallModulesArgs(instance, products);
        PipelineManager.StartPipeline("installmodules", args, isAsync: false);
      }

      if (!FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        return;
      }

      if (this.bypassSecurity)
      {
        InstanceHelperEx.OpenInBrowserAsAdmin(instance, mainWindow, @"/sitecore/admin");
      }
      else
      {
        InstanceHelperEx.BrowseInstance(instance, mainWindow, @"/sitecore/admin", false);
      }
    }

    #endregion
  }
}