namespace SIM.Products
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  #region

  #endregion

  public static class ProductHelper
  {
    #region Public Methods

    [NotNull]
    public static string DetectProductFullName([NotNull] string webRootPath)
    {
      Assert.ArgumentNotNull(webRootPath, "webRootPath");

      var jetstreamAssemblies = FileSystem.FileSystem.Local.Directory.GetFiles(Path.Combine(webRootPath, "bin"), "Jetstream.*.dll");
      if (jetstreamAssemblies.Any())
      {
        var solutionInfo = Path.Combine(webRootPath, "Properties\\SolutionInfo.cs");
        const string name = "Sitecore Jetstream";
        if (!FileSystem.FileSystem.Local.File.Exists(solutionInfo))
        {
          return name;
        }

        /*
          [assembly: AssemblyVersion("1.3.0.2113")]
          [assembly: AssemblyFileVersion("13.02.14")]
         */
        var text = FileSystem.FileSystem.Local.File.ReadAllText(solutionInfo);
        const string versionPrefix = @"AssemblyVersion(""";
        const string revisionPrefix = @"AssemblyFileVersion(""";
        var versionPos = text.IndexOf(versionPrefix);
        if (versionPos < 0)
        {
          return name;
        }

        var version = text.Substring(versionPos + versionPrefix.Length, 5);
        var revisionPos = text.IndexOf(revisionPrefix);
        if (revisionPos < 0)
        {
          return "{0} {1}".FormatWith(name, version);
        }

        var revision = text.Substring(revisionPos + revisionPrefix.Length, 8)
          .Replace(".", string.Empty);
        return "{0} {1} rev. {2}".FormatWith(name, version, revision);
      }

      string assemblyPath = Path.Combine(webRootPath, "bin\\Sitecore.Nicam.dll");
      if (FileSystem.FileSystem.Local.File.Exists(assemblyPath))
      {
        FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assemblyPath);
        string value = "Nicam {0}.{1}.{2} rev. {3}".FormatWith(new object[]
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
        string path = Path.Combine(webRootPath, "bin\\" + fileName);
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
      Assert.ArgumentNotNullOrEmpty(webRootPath, "webRootPath");

      return Path.Combine(webRootPath, "bin\\Sitecore.Kernel.dll");
    }

    [CanBeNull]
    public static string LocateAnalytics([NotNull] Product product)
    {
      Assert.ArgumentNotNull(product, "product");

      Assert.IsTrue(product.Name.EqualsIgnoreCase("sitecore cms"), "Analytics can be located only for the sitecore product");

      string rev = product.Revision;
      string odms = product.Version.StartsWith("6.5") ? "dms" : "oms";
      IEnumerable<Product> products = ProductManager.GetProducts(odms, null, rev);
      if (products != null)
      {
        Product analytics = products.FirstOrDefault();
        Assert.IsNotNull(analytics, "Analytics package not found");
        return analytics.PackagePath;
      }

      return null;
    }

    #endregion

    #region Methods

    [NotNull]
    private static string GetProductFullName([NotNull] string assemblyPath)
    {
      Assert.ArgumentNotNull(assemblyPath, "assemblyPath");

      var websiteFolderPath = Path.GetDirectoryName(Path.GetDirectoryName(assemblyPath));
      Assert.IsNotNull(websiteFolderPath, "folder");

      var sitecoreVersionFilePath = Path.Combine(websiteFolderPath, "sitecore\\shell\\sitecore.version.xml");
      Assert.IsNotNull(sitecoreVersionFilePath, "sitecoreVersionFilePath");

      if (FileSystem.FileSystem.Local.File.Exists(sitecoreVersionFilePath))
      {
        try
        {
          var xml = new XmlDocumentEx(sitecoreVersionFilePath);
          var major = xml.SelectSingleNode("information/version/major");
          Assert.IsNotNull(major, "major");

          var minor = xml.SelectSingleNode("information/version/minor");
          Assert.IsNotNull(minor, "minor");

          var build = xml.SelectSingleNode("information/version/build");
          var revision = xml.SelectSingleNode("information/version/revision");
          return "Sitecore CMS {0}.{1}.{2} rev. {3}"
            .FormatWith(major.InnerText, minor.InnerText, build == null ? string.Empty : build.InnerText, revision == null ? string.Empty : revision.InnerText)
            .Replace(". rev", " rev")
            .Trim(" .".ToCharArray());
        }
        catch (Exception ex)
        {
          Log.Warn(ex, "An error occurred during reading {0} file", assemblyPath);
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

      public static readonly AdvancedProperty<string> CoreInstallInstanceNamePattern = AdvancedSettings.Create("Core/Product/InstanceNamePattern", "{ShortName}{ShortVersion}rev{Revision}");

      public static readonly AdvancedProperty<bool> CoreManifestsUpdateEnabled = AdvancedSettings.Create("Core/Manifests/Update/Enabled", false);
      public static readonly AdvancedProperty<string> CoreProductRootFolderNamePattern = AdvancedSettings.Create("App/Product/RootFolderNamePattern", "{ShortName}{ShortVersion}rev{Revision}");

      public static AdvancedProperty<string> CoreProductNamePattern = AdvancedSettings.Create("Core/Product/NamePattern", string.Empty);

      #endregion

      // public readonly static AdvancedProperty<bool> InstallModulesCheckRequirements = AdvancedSettings.Create("Core/Install/CheckRequirements", false);
    }

    #endregion
  }
}