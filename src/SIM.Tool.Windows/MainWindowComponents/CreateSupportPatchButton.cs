namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.IO;
  using System.Windows;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Core;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;

  [UsedImplicitly]
  public class CreateSupportPatchButton : IMainWindowButton
  {
    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (instance == null)
      {
        WindowHelper.ShowMessage("Choose an instance first");

        return;
      }

      var product = instance.Product;
      Assert.IsNotNull(product, $"The {instance.ProductFullName} distributive is not available in local repository. You need to get it first.");

      var version = product.Version + "." + product.Update;

      var args = new[]
      {
        version,
        instance.Name,
        instance.WebRootPath
      };

      var dir = Environment.ExpandEnvironmentVariables("%APPDATA%\\Sitecore\\PatchCreator");
      if (!Directory.Exists(dir))
      {
        Directory.CreateDirectory(dir);
      }

      File.WriteAllLines(Path.Combine(dir, "args.txt"), args);

      CoreApp.RunApp("iexplore", $"http://dl.sitecore.net/updater/pc/PatchCreator.application");
              
      NuGetHelper.UpdateSettings();

      NuGetHelper.GeneratePackages(new FileInfo(product.PackagePath));

      foreach (var module in instance.Modules)
      {
        NuGetHelper.GeneratePackages(new FileInfo(module.PackagePath));
      }
    }

    #endregion
  }
}
