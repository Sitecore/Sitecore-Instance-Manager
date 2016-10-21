namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.IO;
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Tool.Base.Profiles;

  [UsedImplicitly]
  public class GenerateNuGetPackagesButton : IMainWindowButton
  {
    private readonly bool InstanceMode;

    public GenerateNuGetPackagesButton()
    {
    }

    public GenerateNuGetPackagesButton(string mode)
    {
      InstanceMode = mode == "instance";
    }

    private const string Link = "http://bitbucket.org/sitecoresupport/sitecore-nuget-packages-generator";

    public bool IsEnabled([CanBeNull] Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, nameof(mainWindow));

      var nugetFolderPath = NuGetHelper.NuGetFolderPath;

      Action longRunningTask = delegate
      {
        if (!Directory.Exists(nugetFolderPath))
        {
          Directory.CreateDirectory(nugetFolderPath);
        }

        NuGetHelper.UpdateSettings(nugetFolderPath);

        if (InstanceMode)
        {   
          var product = instance.Product;
          Assert.IsNotNull(product, $"The {instance.ProductFullName} distributive is not available in local repository. You need to get it first.");
                                           
          NuGetHelper.GeneratePackages(new FileInfo(product.PackagePath));
        }
        else
        {
          NuGetHelper.GeneratePackages(ProfileManager.Profile.LocalRepository, nugetFolderPath);
        }
      };

      var content = $"The SC.* NuGet packages are now being generated in the {nugetFolderPath} directory for all Sitecore versions that exist in the local repository. Read more: {Link}";

      WindowHelper.LongRunningTask(longRunningTask, "Generating NuGet Packages", mainWindow, null, content, true);
    }
  }
}