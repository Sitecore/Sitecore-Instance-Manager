namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Xml;
  using SIM.Instances;
  using SIM.Products;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;
  using Sitecore.NuGet.Core;

  [UsedImplicitly]
  public class GenerateNuGetPackagesButton : IMainWindowButton
  {
    private const string Link = "http://bitbucket.org/sitecoresupport/sitecore-nuget-packages-generator";

    public bool IsEnabled([CanBeNull] Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, "mainWindow");

      var nugetFolderPath = Environment.ExpandEnvironmentVariables(WindowsSettings.AppNuGetDirectory.Value);

      Action longRunningTask = delegate
      {
        if (!Directory.Exists(nugetFolderPath))
        {
          Directory.CreateDirectory(nugetFolderPath);
        }

        UpdateSettings(nugetFolderPath);
        GeneratePackages(nugetFolderPath);
      };

      var content = string.Format("The SC.* NuGet packages are now being generated in the {0} directory for all Sitecore versions that exist in the local repository. Read more: {1}", nugetFolderPath, Link);

      WindowHelper.LongRunningTask(longRunningTask, "Generating NuGet Packages", mainWindow, null, content, true);
    }

    private static void GeneratePackages([NotNull] string nugetFolderPath)
    {
      Assert.ArgumentNotNull(nugetFolderPath, "nugetFolderPath");

      var generator = new PackageGenerator();
      foreach (var product in ProductManager.StandaloneProducts)
      {
        generator.Generate(product.PackagePath, nugetFolderPath);
      }
    }

    private static void UpdateSettings([NotNull] string nugetFolderPath)
    {
      Assert.ArgumentNotNull(nugetFolderPath, "nugetFolderPath");

      var nugetConfigPath = Environment.ExpandEnvironmentVariables(@"%appdata%\NuGet\nuget.config");
      var bakFilePath = nugetConfigPath + ".bak";
      if (!File.Exists(bakFilePath))
      {
        File.Copy(nugetConfigPath, bakFilePath);
      }

      var nugetConfig = XmlDocumentEx.LoadFile(nugetConfigPath);
      Assert.IsNotNull(nugetConfig, "nugetConfig");

      var keyName = "Sitecore NuGet";
      var packageSources = nugetConfig.SelectSingleElement("/configuration/packageSources") ?? nugetConfig.AddElement("packageSources");
      Assert.IsNotNull(packageSources, "packageSources");

      var addElements = packageSources.ChildNodes.OfType<XmlElement>();
      var add = addElements.FirstOrDefault(x => x.GetAttribute("key").Equals(keyName, StringComparison.OrdinalIgnoreCase)) ?? packageSources.AddElement("add");
      add.SetAttribute("key", keyName);
      add.SetAttribute("value", nugetFolderPath);
      nugetConfig.Save();
    }
  }
}