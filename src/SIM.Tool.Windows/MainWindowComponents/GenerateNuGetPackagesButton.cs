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
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.InformationService.Client;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.NuGet.Core;
  using SIM.Tool.Base.Profiles;

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

      Log.Info("Generating NuGet packages from {0} to {1}", ProfileManager.Profile.LocalRepository, nugetFolderPath);
      ProcessFolder(ProfileManager.Profile.LocalRepository, nugetFolderPath);
    }

    private static void ProcessFolder(string directory, string outputFolderPath)
    {
      var client = new ServiceClient();
      foreach (var productName in client.GetProductNames())
      {
        var filePrefix = PackageGenerator.GetFilePrefix(productName);
        var prefix = PackageGenerator.GetAbbr(productName);
        if (string.IsNullOrEmpty(prefix))
        {
          continue;
        }

        foreach (var version in client.GetVersions(productName))
        {
          var majorMinor = PackageGenerator.GetMajorMinor(version);
          foreach (var release in version.Releases)
          {
            var revision = release.Revision;
            var pattern = filePrefix + " " + majorMinor + "* rev. " + revision + ".zip";
            var zipfiles = Directory.GetFiles(directory, pattern, SearchOption.AllDirectories);
            if (zipfiles.Length <= 0)
            {
              continue;
            }

            var releaseVersion = PackageGenerator.GetReleaseVersion(majorMinor, revision);
            var file = zipfiles.First();
            try
            {
              // Create nupkg file
              new PackageGenerator().Generate(prefix, productName, file, releaseVersion, outputFolderPath);
            }
            catch (Exception ex)
            {
              Log.Error("Error processing file " + file + ". " + ex.Message + Environment.NewLine + "Stack trace:" + Environment.NewLine + ex.StackTrace);
            }
          }
        }
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

      var config = nugetConfig.SelectSingleNode("/configuration") ?? nugetConfig.DocumentElement.AddElement("configuration");
      Assert.IsNotNull(config, "config");

      var keyName = "Sitecore NuGet";
      var packageSources = nugetConfig.SelectSingleElement("/configuration/packageSources") ?? config.AddElement("packageSources");
      Assert.IsNotNull(packageSources, "packageSources");

      var addElements = packageSources.ChildNodes.OfType<XmlElement>();
      var add = addElements.FirstOrDefault(x => x.GetAttribute("key").Equals(keyName, StringComparison.OrdinalIgnoreCase)) ?? packageSources.AddElement("add");
      Assert.IsNotNull(add, "add");

      add.SetAttribute("key", keyName);
      add.SetAttribute("value", nugetFolderPath);
      nugetConfig.Save();
    }
  }
}