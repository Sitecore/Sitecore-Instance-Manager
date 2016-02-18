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
      if (instance != null)
      {
        var paths = instance.GetVisualStudioSolutionFiles().ToArray();
        if (paths.Length == 1)
        {
          WindowHelper.OpenFile(paths.First());
        }
        else if (paths.Length == 0)
        {
          var version = instance.Product.Version.SubstringEx(0, 3);
          const string noThanks = "No, thanks";
          const string yesAspNetWebforms = "Yes, ASP.NET WebForms";
          const string yesAspNetMvc = "Yes, ASP.NET MVC";

          var packageName = "Visual Studio " + WinAppSettings.AppToolsVisualStudioVersion.Value + " Website Project.zip";

          var is66 = version == "6.6";
          var is70 = version == "7.0";
          var is7x = version[0] == '7';
          var is71plus = is7x && !is70;
          var is8 = version[0] == '8';
          if (is66 || is7x || is8)
          {
            var options = is71plus ? new[]
            {
              noThanks, yesAspNetMvc
            } : new[]
            {
              noThanks, yesAspNetWebforms, yesAspNetMvc
            };
            var result = WindowHelper.AskForSelection("Choose Visual Studio Project Type", null, 
              "There isn't any Visual Studio project in the instance's folder. \n\nWould you like us to create a new one?", 
              options, mainWindow);
            if (result == null || result == noThanks)
            {
              return;
            }

            if (result == yesAspNetMvc)
            {
              if (!this.EnsureMvcEnabled(mainWindow, instance, version))
              {
                return;
              }

              packageName = "Visual Studio " + WinAppSettings.AppToolsVisualStudioVersion.Value + " MVC" + (is71plus ? " 4" : string.Empty) + " Website Project.zip";
            }
          }

          Product product = GetProduct(packageName);

          if (product == null)
          {
            WindowHelper.HandleError("The " + packageName + " package cannot be found in either the .\\Packages folder", false, null);
            return;
          }

          PipelineManager.StartPipeline("installmodules", new InstallModulesArgs(instance, new[]
          {
            product
          }), isAsync: false);
          var path = instance.GetVisualStudioSolutionFiles().FirstOrDefault();
          Assert.IsTrue(!string.IsNullOrEmpty(path) && FileSystem.FileSystem.Local.File.Exists(path), "The Visual Studio files are missing");
          WindowHelper.OpenFile(path);
        }
        else
        {
          var rootPath = instance.RootPath.ToLowerInvariant();
          var virtualPaths = paths.Select(file => file.ToLowerInvariant().Replace(rootPath, string.Empty).TrimStart('\\'));
          var path = WindowHelper.AskForSelection("Open Visual Studio", null, "Select the project you need", virtualPaths, mainWindow, paths.First());
          if (string.IsNullOrEmpty(path))
          {
            return;
          }

          WindowHelper.OpenFile(Path.Combine(rootPath, path));
        }
      }
    }

    #endregion

    #region Private methods

    private static Product GetProduct(string packageName)
    {
      foreach (var folderPath in EnvironmentHelper.FilePackageFolders)
      {
        var packagePath = Path.Combine(folderPath, packageName);
        if (FileSystem.FileSystem.Local.File.Exists(packagePath))
        {
          var product = Product.GetFilePackageProduct(packagePath);
          if (product != null)
          {
            return product;
          }
        }
      }

      return null;
    }

    private bool EnsureFramework4(Window mainWindow, Instance instance)
    {
      // verify that .NET framework v4 is used
      if (!instance.IsNetFramework4)
      {
        // suggest to install them
        const string yesEnableSitecoreMvc = "Yes, switch to .NET 4.X";
        const string noCancel = "No, cancel";
        var options2 = new[]
        {
          noCancel, yesEnableSitecoreMvc
        };
        var result2 = WindowHelper.AskForSelection(".NET Framework 4.X is not set", null, 
          "You decided to enable Sitecore MVC, but it requiuires using .NET Framework 4.X in Sitecore instance's application pool for correct work. \n\nWould you like to change .NET Framework version?", 
          options2, mainWindow);

        if (result2 == noCancel)
        {
          return false;
        }

        instance.SetAppPoolMode(true);
      }

      return true;
    }

    private bool EnsureMvcConfigured(Instance instance, string version)
    {
      string enableMvcPackageName = Path.Combine(ApplicationManager.DefaultPackages, "Configuration - Enable MVC for Sitecore " + version + ".zip");
      Product enableMvcProduct = Product.GetFilePackageProduct(enableMvcPackageName);
      if (enableMvcProduct == null)
      {
        WindowHelper.HandleError(
          "The " + enableMvcPackageName +
          " package cannot be found in either the .\\File Packages folder or %appdata%\\Sitecore\\Sitecore Instance Manager\\Custom Packages one", 
          false, null);
        return false;
      }

      PipelineManager.StartPipeline("installmodules", new InstallModulesArgs(instance, new[]
      {
        enableMvcProduct
      }), isAsync: false);

      if (!this.EnsureMvcForDmsConfigured(instance, version))
      {
        return false;
      }

      return true;
    }

    private bool EnsureMvcEnabled(Window mainWindow, Instance instance, string version)
    {
      // verify if assemblies and configuration exists
      if ((!FileSystem.FileSystem.Local.File.Exists(Path.Combine(instance.WebRootPath, "bin\\Sitecore.Mvc.dll")) || !FileSystem.FileSystem.Local.File.Exists(Path.Combine(instance.WebRootPath, "App_Config\\Include\\Sitecore.Mvc.config"))) && (version == "6.6" || version == "7.0"))
      {
        // suggest to install them
        const string yesEnableSitecoreMvc = "Yes, enable Sitecore MVC";
        const string noCancel = "No, cancel";
        var options2 = new[]
        {
          noCancel, yesEnableSitecoreMvc
        };
        var result2 = WindowHelper.AskForSelection("Sitecore MVC is not enabled", null, 
          "It seems that Sitecore MVC features is not enabled in your Sitecore " +
          version + " instance. \n\nWould you like to enable it?", 
          options2, mainWindow);

        if (result2 == noCancel)
        {
          return false;
        }

        if (!this.EnsureFramework4(mainWindow, instance))
        {
          return false;
        }

        if (!this.EnsureMvcConfigured(instance, version))
        {
          return false;
        }

        return true;
      }

      if (!this.EnsureFramework4(mainWindow, instance))
      {
        return false;
      }

      return true;
    }

    private bool EnsureMvcForDmsConfigured(Instance instance, string version)
    {
      var dmsConfigPath = Path.Combine(instance.WebRootPath, "App_Config\\Include\\Sitecore.Analytics.config");
      var dmsMvcConfigPath = Path.Combine(instance.WebRootPath, "App_Config\\Include\\Sitecore.MvcAnalytics.config");
      if (FileSystem.FileSystem.Local.File.Exists(dmsConfigPath) && !FileSystem.FileSystem.Local.File.Exists(dmsMvcConfigPath))
      {
        string enableMvcForDmsPackageName = Path.Combine(ApplicationManager.DefaultPackages, "Configuration - Enable MVC-DMS for Sitecore " + version + ".zip");
        Product enableMvcForDmsProduct =
          Product.GetFilePackageProduct(enableMvcForDmsPackageName);
        if (enableMvcForDmsProduct == null)
        {
          WindowHelper.HandleError(
            "The " + enableMvcForDmsPackageName +
            " package cannot be found in either the .\\File Packages folder or %appdata%\\Sitecore\\Sitecore Instance Manager\\Custom Packages one", 
            false, null);
          return false;
        }

        PipelineManager.StartPipeline("installmodules", new InstallModulesArgs(instance, new[]
        {
          enableMvcForDmsProduct
        }), 
          isAsync: false);
      }

      return true;
    }

    #endregion
  }
}