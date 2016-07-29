namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.IO;
  using System.Windows;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
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
      CoreApp.RunApp("iexplore", $"http://dl.sitecore.net/updater/pc/CreatePatch.application?p1={version}&p2={instance.Name}&p3={instance.WebRootPath}");
              
      NuGetHelper.UpdateSettings();

      NuGetHelper.GeneratePackages(new FileInfo(product.PackagePath));      
    }

    #endregion
  }
}