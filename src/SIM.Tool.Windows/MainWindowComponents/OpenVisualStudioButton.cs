namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.IO;
  using System.Linq;
  using System.Windows;
  using SIM.Instances;
  using SIM.Pipelines;
  using SIM.Pipelines.InstallModules;
  using SIM.Products;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core;

  [UsedImplicitly]
  public class OpenVisualStudioButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (instance == null)
      {
        return;
      }

      var paths = instance.GetVisualStudioSolutionFiles().ToArray();

      if (paths.Length > 0)
      {
        CoreApp.OpenFile(paths.First());
        return;
      }

      const string noThanks = "No, thanks";
      const string yesAspNetMvc = "Yes, ASP.NET MVC";
      
      var options = new[] { noThanks, yesAspNetMvc };
      var result = WindowHelper.AskForSelection("Choose Visual Studio Project Type", null, "There isn't any Visual Studio project in the instance's folder. \n\nWould you like us to create a new one?", options, mainWindow);
      if (result == null || result == noThanks)
      {
        return;
      }

      var packageName = "Visual Studio 2015 Website Project.zip";
      var product = GetProduct(packageName);
      if (product == null)
      {
        WindowHelper.HandleError("The " + packageName + " package cannot be found in either the .\\Packages folder", false);
        return;
      }

      PipelineManager.StartPipeline("installmodules", new InstallModulesArgs(instance, new[] { product }), isAsync: false);
      var path = instance.GetVisualStudioSolutionFiles().FirstOrDefault();
      Assert.IsTrue(!string.IsNullOrEmpty(path) && FileSystem.FileSystem.Local.File.Exists(path), "The Visual Studio files are missing");

      CoreApp.OpenFile(path);
    }

    #endregion

    #region Private methods

    private static Product GetProduct(string packageName)
    {
      foreach (var folderPath in EnvironmentHelper.FilePackageFolders)
      {
        var packagePath = Path.Combine(folderPath, packageName);
        if (!FileSystem.FileSystem.Local.File.Exists(packagePath))
        {
          continue;
        }

        var product = Product.GetFilePackageProduct(packagePath);
        if (product != null)
        {
          return product;
        }
      }

      return null;
    }

    #endregion
  }
}