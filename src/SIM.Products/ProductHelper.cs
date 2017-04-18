namespace SIM.Products
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  #region

  #endregion

  public static class ProductHelper
  {
    #region Public Methods

    [NotNull]
    public static string DetectProductFullName([NotNull] string webRootPath)
    {
      Assert.ArgumentNotNull(webRootPath, nameof(webRootPath));

      var jetstreamAssemblies = FileSystem.FileSystem.Local.Directory.GetFiles(Path.Combine(webRootPath, "bin"), "Jetstream.*.dll");
      if (jetstreamAssemblies.Any())
      {
        var solutionInfo = Path.Combine(webRootPath, "Properties\\SolutionInfo.cs");
        const string Name = "Sitecore Jetstream";
        if (!FileSystem.FileSystem.Local.File.Exists(solutionInfo))
        {
          return Name;
        }

        /*
          [assembly: AssemblyVersion("1.3.0.2113")]
          [assembly: AssemblyFileVersion("13.02.14")]
         */
        var text = FileSystem.FileSystem.Local.File.ReadAllText(solutionInfo);
        const string VersionPrefix = @"AssemblyVersion(""";
        const string RevisionPrefix = @"AssemblyFileVersion(""";
        var versionPos = text.IndexOf(VersionPrefix);
        if (versionPos < 0)
        {
          return Name;
        }

        var version = text.Substring(versionPos + VersionPrefix.Length, 5);
        var revisionPos = text.IndexOf(RevisionPrefix);
        if (revisionPos < 0)
        {
          return "{0} {1}".FormatWith(Name, version);
        }

        var revision = text.Substring(revisionPos + RevisionPrefix.Length, 8)
          .Replace(".", string.Empty);
        return "{0} {1} rev. {2}".FormatWith(Name, version, revision);
      }

      var assemblyPath = Path.Combine(webRootPath, "bin\\Sitecore.Nicam.dll");
      if (FileSystem.FileSystem.Local.File.Exists(assemblyPath))
      {
        FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assemblyPath);
        var value = "Nicam {0}.{1}.{2} rev. {3}".FormatWith(new object[]
        {
          0, 0, 0, versionInfo.FileVersion.Replace(".", string.Empty)
        });

        return value;
      }

      foreach (string fileName in new[]
      {
        "Sitecore.Intranet.dll", "MSS.Kernel.dll"
      })
      {
        var path = Path.Combine(webRootPath, "bin\\" + fileName);
        if (FileSystem.FileSystem.Local.File.Exists(path))
        {
          return GetProductFullName(path);
        }
      }

      return GetProductFullName(GetKernelPath(webRootPath));
    }

    [NotNull]
    public static string GetKernelPath([NotNull] string webRootPath)
    {
      Assert.ArgumentNotNullOrEmpty(webRootPath, nameof(webRootPath));

      return Path.Combine(webRootPath, "bin\\Sitecore.Kernel.dll");
    }

    #endregion

    #region Methods

    [NotNull]
    private static string GetProductFullName([NotNull] string assemblyPath)
    {
      Assert.ArgumentNotNull(assemblyPath, nameof(assemblyPath));

      var websiteFolderPath = Path.GetDirectoryName(Path.GetDirectoryName(assemblyPath));
      Assert.IsNotNull(websiteFolderPath, nameof(websiteFolderPath));

      var sitecoreVersionFilePath = Path.Combine(websiteFolderPath, "sitecore\\shell\\sitecore.version.xml");
      Assert.IsNotNull(sitecoreVersionFilePath, nameof(sitecoreVersionFilePath));

      if (FileSystem.FileSystem.Local.File.Exists(sitecoreVersionFilePath))
      {
        try
        {
          var xml = new XmlDocumentEx(sitecoreVersionFilePath);
          var major = xml.SelectSingleNode("information/version/major");
          Assert.IsNotNull(major, nameof(major));

          var minor = xml.SelectSingleNode("information/version/minor");
          Assert.IsNotNull(minor, nameof(minor));

          var build = xml.SelectSingleNode("information/version/build");
          var revision = xml.SelectSingleNode("information/version/revision");
          return "Sitecore CMS {0}.{1}.{2} rev. {3}"
            .FormatWith(major.InnerText, minor.InnerText, build == null ? string.Empty : build.InnerText, revision == null ? string.Empty : revision.InnerText)
            .Replace(". rev", " rev")
            .Trim(" .".ToCharArray());
        }
        catch (Exception ex)
        {
          Log.Warn(ex, $"An error occurred during reading {assemblyPath} file");
        }
      }

      FileInfo kernel = new FileInfo(assemblyPath);
      Assert.IsTrue(kernel.Exists, "It's not a Sitecore product");

      FileVersionInfo info = FileVersionInfo.GetVersionInfo(assemblyPath);
      if (info.ProductVersion == "5.3.0.0")
      {
        if (kernel.LastWriteTime.Year == 2006)
        {
          return "Sitecore CMS 5.3.0 rev. 06802";
        }

        if (kernel.LastWriteTime.Year == 2007)
        {
          return "Sitecore CMS 5.3.0 rev. 070215";
        }
      }

      return info.ProductName + ' ' + info.ProductVersion;
    }

    #endregion

    #region Nested type: Settings

    public static class Settings
    {
      #region Fields

      public static readonly AdvancedProperty<string> CoreInstallInstanceNamePattern = AdvancedSettings.Create("Core/Install/Default/InstanceNamePattern", "{ShortName}{ShortVersion}{UpdateOrRevision}");
      
      public static readonly AdvancedProperty<string> CoreProductHostNameSuffix = AdvancedSettings.Create("App/Install/Default/HostName/Suffix", ".local");

      public static readonly bool CoreProductHostNameEndsWithLocal = !string.IsNullOrEmpty(CoreProductHostNameSuffix.Value);

      public static readonly AdvancedProperty<bool> CoreProductReverseHostName = AdvancedSettings.Create("App/Install/Default/HostName/Reverse", true);

      #endregion
    }

    #endregion
  }
}
