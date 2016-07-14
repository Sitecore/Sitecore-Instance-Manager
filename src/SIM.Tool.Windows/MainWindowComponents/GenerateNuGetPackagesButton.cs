namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Xml;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
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

      var content = $"The SC.* NuGet packages are now being generated in the {nugetFolderPath} directory for all Sitecore versions that exist in the local repository. Read more: {Link}";

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
      var pattern = "*rev*.zip";
      var zipfiles = Directory.GetFiles(directory, pattern, SearchOption.AllDirectories);
      foreach (var file in zipfiles)
      {
        try
        {
          Log.Info("Generating NuGet packages from: " + file);

          // Create nupkg file
          new PackageGenerator().Generate(file, outputFolderPath);
        }
        catch (Exception ex)
        {
          Log.Error("Error processing file " + file + ". " + ex.Message + Environment.NewLine + "Stack trace:" + Environment.NewLine + ex.StackTrace);
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