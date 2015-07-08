using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIM.Base;
using SIM.Products;

namespace SIM.Tests.Products
{
  [TestClass]
  public class ManifestHelperTest
  {
    [TestInitialize]
    public void Initialize()
    {
      TestHelper.Initialize();
      ManifestHelper.CustomManifestsLocations = new[]
      {
        new ManifestHelper.LookupFolder(Path.GetFullPath(@"..\..\..\Code\Core Level 2\Kernel.Products\Manifests"), true),
        new ManifestHelper.LookupFolder(Path.GetFullPath(@"..\..\..\Code\WPF Client\Tool.Windows\Packages"), true),
        new ManifestHelper.LookupFolder(Path.GetFullPath(@"..\..\..\Code\WPF Client\Tool.Plugins\ConfigPatcher\Plugins\Config Patcher"), true),
        new ManifestHelper.LookupFolder(Path.GetFullPath(@"..\..\..\Code\WPF Client\Tool.Plugins\SupportToolbox\Plugins\Support Toolbox"), true),
        new ManifestHelper.LookupFolder(Path.GetFullPath(@"..\..\..\Code\WPF Client\Tool.Plugins\SupportWorkaround\Plugins\Support Workaround"), true)
      };
    }

    [TestMethod]
    public void GetFileNamePatternsTestNull()
    {
      TestHelper.MustFail(() => ManifestHelper.GetFileNamePatterns(null));
    }

    [TestMethod]
    public void GetFileNamePatternsTestNullEmpty()
    {
      TestHelper.MustFail(() => ManifestHelper.GetFileNamePatterns(null, string.Empty));
    }

    [TestMethod]
    public void GetFileNamePatternsTestNullHello()
    {
      TestHelper.MustFail(() => ManifestHelper.GetFileNamePatterns(null, "hello"));
    }

    [TestMethod]
    public void GetFileNamePatternsTestEmpty()
    {
      TestHelper.MustFail(() => ManifestHelper.GetFileNamePatterns(string.Empty));
    }

    [TestMethod]
    public void GetFileNamePatternsTestEmptyEmpty()
    {
      TestHelper.MustFail(() => ManifestHelper.GetFileNamePatterns(string.Empty, string.Empty));
    }

    [TestMethod]
    public void GetFileNamePatternsTestCustomPackage()
    {
      // CMS
      {
        var path = "C:\\Support Toolbox.zip";
        var actual = ManifestHelper.GetFileNamePatterns(path);
        var expect = new[] { "Support Toolbox" };
        TestHelper.AreEqual(actual, expect, "Path is " + path);
      }
    }

    [TestMethod]
    public void GetFileNamePatternsTestCms()
    {
      // CMS
      {
        var path = "C:\\Sitecore 1.2.3 rev. 456789.zip";
        var actual = ManifestHelper.GetFileNamePatterns(path);
        var expect = new[] { "Sitecore", "Sitecore 1", "Sitecore 1.2", "Sitecore 1.2.3", "Sitecore 1.2.3 rev. 456789" };
        TestHelper.AreEqual(actual, expect, "Path is " + path);
      }
    }

    [TestMethod]
    public void GetFileNamePatternsTestNoSpaces()
    {
      // no spaces
      {
        var path = "C:\\Sitecore 1.2.3rev.456789.zip";
        var actual = ManifestHelper.GetFileNamePatterns(path);
        var expect = new[] { "Sitecore", "Sitecore 1", "Sitecore 1.2", "Sitecore 1.2.3", "Sitecore 1.2.3 rev. 456789" };
        TestHelper.AreEqual(actual, expect, "Path is " + path);
      }
    }

    [TestMethod]
    public void GetFileNamePatternsTestTwoDigits()
    {
      // two digits
      {
        var path = "C:\\Sitecore 7.4 rev. 456789.zip";
        var actual = ManifestHelper.GetFileNamePatterns(path);
        var expect = new[] { "Sitecore", "Sitecore 7", "Sitecore 7.4", "Sitecore 7.4 rev. 456789" };
        TestHelper.AreEqual(actual, expect, "Path is " + path);
      }
    }

    [TestMethod]
    public void GetFileNamePatternsTestUpgrade1()
    {
      // IGNORE UPGRADE
      {
        var path = "C:\\Web Forms for Marketers - 1.0.1 rev. 090601 upgrade.zip";
        var actual = ManifestHelper.GetFileNamePatterns(path);
        var expect = new string[0];
        TestHelper.AreEqual(actual, expect, "Path is " + path);
      }
    }

    [TestMethod]
    public void GetFileNamePatternsTestUpgrade2()
    {
      // IGNORE upgrade
      {
        var path = "sitecore active directory - 1.0.1 rev. 090302(upgrade).zip";
        var actual = ManifestHelper.GetFileNamePatterns(path);
        var expect = new string[0];
        TestHelper.AreEqual(actual, expect, "Path is " + path);
      }
    }

    [TestMethod]
    public void GetFileNamePatternsTestUpgrade3()
    {
      // IGNORE upgrade
      {
        var path = "sitecore intranet portal 3.1.0 rev. 100421 preparetoupgrade.zip";
        var actual = ManifestHelper.GetFileNamePatterns(path);
        var expect = new string[0];
        TestHelper.AreEqual(actual, expect, "Path is " + path);
      }
    }

    [TestMethod]
    public void GetFileNamePatternsTestOracle()
    {
      // IGNORE Oracle
      {
        var path = "sitecore active directory - 1.0.1 rev. 090302(oracle).zip";
        var actual = ManifestHelper.GetFileNamePatterns(path);
        var expect = new string[0];
        TestHelper.AreEqual(actual, expect, "Path is " + path);
      }
    }

    [TestMethod]
    public void GetFileNamePatternsTestExe()
    {
      // IGNORE exe
      {
        var path = "sitecore active directory - 1.0.1 rev. 090302(exe).zip";
        var actual = ManifestHelper.GetFileNamePatterns(path);
        var expect = new string[0];
        TestHelper.AreEqual(actual, expect, "Path is " + path);
      }
    }

    [TestMethod]
    public void GetFileNamePatternsTestModule1()
    {
      // Module
      {
        var path = "C:\\Web Forms for Marketers - 1.0.1 rev. 090601.zip";
        var actual = ManifestHelper.GetFileNamePatterns(path);
        var expect = new[] { "Web Forms for Marketers", "Web Forms for Marketers 1", "Web Forms for Marketers 1.0", "Web Forms for Marketers 1.0.1", "Web Forms for Marketers 1.0.1 rev. 090601" };
        TestHelper.AreEqual(actual, expect, "Path is " + path);
      }
    }

    [TestMethod]
    public void GetFileNamePatternsTestModule2()
    {
      // Module
      {
        var path = "C:\\Web Forms for Marketers-2.1.0 rev. 100806.zip";
        var actual = ManifestHelper.GetFileNamePatterns(path);
        var expect = new[] { "Web Forms for Marketers", "Web Forms for Marketers 2", "Web Forms for Marketers 2.1", "Web Forms for Marketers 2.1.0", "Web Forms for Marketers 2.1.0 rev. 100806" };
        TestHelper.AreEqual(actual, expect, "Path is " + path);
      }
    }

    [TestMethod]
    public void GetFileNamePatternsTestPostfix()
    {
      // postfix
      {
        var path = "C:\\Web Forms for Marketers-2.1.0 rev. 100806_test.zip";
        var actual = ManifestHelper.GetFileNamePatterns(path);
        var expect = new[] { "Web Forms for Marketers", "Web Forms for Marketers 2", "Web Forms for Marketers 2.1", "Web Forms for Marketers 2.1.0", "Web Forms for Marketers 2.1.0 rev. 100806_test" };
        TestHelper.AreEqual(actual, expect, "Path is " + path);
      }
    }

    [TestMethod]
    public void GetFileNamePatternsTestSpecialChars1()
    {
      // Special characters 
      {
        var path = "C:\\sip_ad_samplecode 2.2.0 rev.081204.zip";
        var actual = ManifestHelper.GetFileNamePatterns(path);
        var expect = new[] { "sip_ad_samplecode", "sip_ad_samplecode 2", "sip_ad_samplecode 2.2", "sip_ad_samplecode 2.2.0", "sip_ad_samplecode 2.2.0 rev. 081204" };
        TestHelper.AreEqual(actual, expect, "Path is " + path);
      }
    }

    [TestMethod]
    public void GetFileNamePatternsTestSpecialChars2()
    {
      {
        var path = "C:\\Site1-core2 1.2.3 rev. 456789.zip";
        var actual = ManifestHelper.GetFileNamePatterns(path);
        var expect = new[] { "Site1-core2", "Site1-core2 1", "Site1-core2 1.2", "Site1-core2 1.2.3", "Site1-core2 1.2.3 rev. 456789" };
        TestHelper.AreEqual(actual, expect, "Path is " + path);
      }
    }

    [TestMethod]
    public void GetFileNamePatternsTestLongRevision()
    {
      // long revision
      {
        var path = "C:\\sip_ad_samplecode 2.2.0 rev.081204 123 lol 088888.zip";
        var actual = ManifestHelper.GetFileNamePatterns(path);
        var expect = new[] { "sip_ad_samplecode", "sip_ad_samplecode 2", "sip_ad_samplecode 2.2", "sip_ad_samplecode 2.2.0", "sip_ad_samplecode 2.2.0 rev. 081204 123 lol 088888" };
        TestHelper.AreEqual(actual, expect, "Path is " + path);
      }
    }

    [TestMethod]
    [DeploymentItem("TestData\\Custom\\Plugins", "Custom\\Plugins")]
    public void ComputeTestCustomPlugins()
    {
      ComputeTestFolder(Path.GetFullPath("Custom\\Plugins"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\APS", "Products\\APS")]
    public void ComputeTestAps()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\APS"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore 6.0.0", "Products\\Sitecore 6.0.0")]
    public void ComputeTestSitecore600()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore 6.0.0"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore 6.0.1", "Products\\Sitecore 6.0.1")]
    public void ComputeTestSitecore601()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore 6.0.1"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore 6.0.2", "Products\\Sitecore 6.0.2")]
    public void ComputeTestSitecore602()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore 6.0.2"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore 6.1.0", "Products\\Sitecore 6.1.0")]
    public void ComputeTestSitecore610()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore 6.1.0"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore 6.2.0", "Products\\Sitecore 6.2.0")]
    public void ComputeTestSitecore620()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore 6.2.0"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore 6.3.0", "Products\\Sitecore 6.3.0")]
    public void ComputeTestSitecore630()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore 6.3.0"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore 6.3.1", "Products\\Sitecore 6.3.1")]
    public void ComputeTestSitecore631()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore 6.3.1"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore 6.4.0", "Products\\Sitecore 6.4.0")]
    public void ComputeTestSitecore640()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore 6.4.0"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore 6.4.1", "Products\\Sitecore 6.4.1")]
    public void ComputeTestSitecore641()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore 6.4.1"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore 6.5.0", "Products\\Sitecore 6.5.0")]
    public void ComputeTestSitecore650()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore 6.5.0"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore 6.6.0", "Products\\Sitecore 6.6.0")]
    public void ComputeTestSitecore660()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore 6.6.0"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore 7.0", "Products\\Sitecore 7.0")]
    public void ComputeTestSitecore7()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore 7.0"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore 7.1", "Products\\Sitecore 7.1")]
    public void ComputeTestSitecore71()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore 7.1"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore 7.2", "Products\\Sitecore 7.2")]
    public void ComputeTestSitecore72()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore 7.2"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore 7.5", "Products\\Sitecore 7.5")]
    public void ComputeTestSitecore75()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore 7.5"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore Azure", "Products\\Sitecore Azure")]
    public void ComputeTestSitecoreAzure()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore Azure"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore E-Commerce", "Products\\Sitecore E-Commerce")]
    public void ComputeTestSitecoreECommerce()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore E-Commerce"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore Foundry", "Products\\Sitecore Foundry")]
    public void ComputeTestSitecoreFoundry()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore Foundry"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore Media Framework", "Products\\Sitecore Media Framework")]
    public void ComputeTestSitecoreMediaFramework()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore Media Framework"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\Sitecore OBEC", "Products\\Sitecore OBEC")]
    public void ComputeTestSitecoreOBEC()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\Sitecore OBEC"));
    }

    [TestMethod]
    [DeploymentItem("TestData\\Products\\SitecoreIntranetPortal", "Products\\SitecoreIntranetPortal")]
    public void ComputeTestSitecoreIntranetPortal()
    {
      ComputeTestFolder(Path.GetFullPath("Products\\SitecoreIntranetPortal"));
    }

    private void ComputeTestFolder(string path)
    {
      foreach (var directory in FileSystem.Local.Directory.GetDirectories(path))
      {
        ComputeTestFolder(directory);
      }

      foreach (var file in FileSystem.Local.Directory.GetFiles(path, "*.zip"))
      {
        ComputeTestFile(file);
      }
    }

    private void ComputeTestFile(string file)
    {
      var expectedManifestPath = file + ".xml";
      if (!File.Exists(expectedManifestPath))
      {
        Log.Warn("The {0} file does not exist, maybe due to too long file path".FormatWith(file), this);
        return;
      }

      var expectedManifest = XmlDocumentEx.LoadFile(expectedManifestPath);
      var actualManifest = ManifestHelper.Compute(file);

      try
      {
        TestHelper.AreEqual(actualManifest, expectedManifest);
      }
      catch
      {
        var tempFileName = Path.GetTempFileName() + "." + Path.GetFileName(expectedManifestPath);
        actualManifest.Save(tempFileName);
        actualManifest = XmlDocumentEx.LoadFile(tempFileName);
        TestHelper.AreEqual(actualManifest, expectedManifest, @"Change expected CMD:
copy ""{0}"" ""{1}"" /Y".FormatWith(file, Path.GetFullPath(Environment.CurrentDirectory + "\\..\\..\\..\\Tests\\Tests\\TestData") + tempFileName.Replace(Environment.CurrentDirectory, string.Empty)));
      }
    }
  }
}
