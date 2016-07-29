namespace SIM.Tool.Windows
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Xml;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.NuGet.Core;
  using SIM.Extensions;
  using SIM.Tool.Base.Profiles;

  public static class NuGetHelper
  {
    public static string NuGetFolderPath => Environment.ExpandEnvironmentVariables(WindowsSettings.AppNuGetDirectory.Value);

    public static void GeneratePackages([NotNull] string directory, [CanBeNull] string outputFolderPath)
    {
      Assert.ArgumentNotNull(directory, nameof(directory));

      outputFolderPath = outputFolderPath ?? NuGetFolderPath;

      Log.Info("Generating NuGet packages from {0} to {1}", ProfileManager.Profile.LocalRepository, outputFolderPath);

      var pattern = "*rev*.zip";
      var zipfiles = Directory.GetFiles(directory, pattern, SearchOption.AllDirectories);
      foreach (var filePath in zipfiles)
      {
        var file = new FileInfo(filePath);
        try
        {
          GeneratePackages(file, outputFolderPath);
        }
        catch (Exception ex)
        {
          Log.Error("Error processing file " + file + ". " + ex.Message + Environment.NewLine + "Stack trace:" + Environment.NewLine + ex.StackTrace);
        }
      }
    }

    public static void GeneratePackages([NotNull] FileInfo file, [CanBeNull] string outputFolderPath = null)
    {
      Assert.ArgumentNotNull(file, nameof(file));

      Log.Info("Generating NuGet packages from {0} to {1} ", file, outputFolderPath);

      // Create nupkg file
      new PackageGenerator().Generate(file.FullName, outputFolderPath ?? NuGetFolderPath);
    }

    public static void UpdateSettings([CanBeNull] string nugetFolderPath = null)
    {
      nugetFolderPath = nugetFolderPath ?? NuGetFolderPath;

      var nugetConfigPath = Environment.ExpandEnvironmentVariables(@"%appdata%\NuGet\nuget.config");
      var bakFilePath = nugetConfigPath + ".bak";
      if (!File.Exists(bakFilePath))
      {
        File.Copy(nugetConfigPath, bakFilePath);
      }

      var nugetConfig = XmlDocumentEx.LoadFile(nugetConfigPath);
      Assert.IsNotNull(nugetConfig, nameof(nugetConfig));

      var config = nugetConfig.SelectSingleNode("/configuration") ?? nugetConfig.DocumentElement.AddElement("configuration");
      Assert.IsNotNull(config, nameof(config));

      var keyName = "Sitecore NuGet";
      var packageSources = nugetConfig.SelectSingleElement("/configuration/packageSources") ?? config.AddElement("packageSources");
      Assert.IsNotNull(packageSources, nameof(packageSources));

      var addElements = packageSources.ChildNodes.OfType<XmlElement>();
      var add = addElements.FirstOrDefault(x => x.GetAttribute("key").Equals(keyName, StringComparison.OrdinalIgnoreCase)) ?? packageSources.AddElement("add");
      Assert.IsNotNull(add, nameof(add));

      add.SetAttribute("key", keyName);
      add.SetAttribute("value", nugetFolderPath);
      nugetConfig.Save();
    }
  }
}
