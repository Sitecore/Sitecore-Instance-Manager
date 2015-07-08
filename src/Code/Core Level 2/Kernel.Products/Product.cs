#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using SIM.Base;

#endregion

namespace SIM.Products
{
  #region

  using Ionic.Zip;

  #endregion

  /// <summary>
  ///   The product.
  /// </summary>
  [Serializable]
  public class Product : IXmlSerializable
  {  
    private static bool GetIsPackage(string packagePath, XmlDocument manifest)
    {
      if (string.IsNullOrEmpty(packagePath)) return false;

      const string cacheName = "IsPackage";
      string path = packagePath.ToLowerInvariant();
      using (new ProfileSection("Is it package or not", typeof(Product)))
      {
        ProfileSection.Argument("packagePath", packagePath);
        ProfileSection.Argument("manifest", manifest);

        try
        {
          // cache            
          var entry = CacheManager.GetEntry(cacheName, path);
          if (entry != null)
          {
            var result = entry.EqualsIgnoreCase("true");

            return ProfileSection.Result(result);
          }

          if (
            manifest.With(
              x =>
              x.SelectSingleElement(ManifestPrefix + "standalone") ?? x.SelectSingleElement(ManifestPrefix + "archive")) !=
            null)
          {
            CacheManager.SetEntry(cacheName, path, "false");
            return ProfileSection.Result(false);
          }

          if (
            manifest.With(
              x =>
              x.SelectSingleElement(ManifestPrefix + "package")) !=
            null)
          {
            CacheManager.SetEntry(cacheName, path, "true");

            return ProfileSection.Result(true);
          }

          CacheManager.SetEntry(cacheName, path, "false");

          return ProfileSection.Result(false);
        }
        catch (Exception e)
        {
          Log.Warn("Detecting if the '{0}' file is a Sitecore Package failed with exception.".FormatWith(path), typeof(Product), e);
          CacheManager.SetEntry(cacheName, path, "false");

          return ProfileSection.Result(false);
        }
      }
    }

    #region Constants

    /// <summary>
    ///   The manifest prefix.
    /// </summary>
    public const string ManifestPrefix = "/manifest[@version='1.3']/";

    private const string ProductRevisionPattern = @"(\d\d\d\d\d\d[\d\w\s_\-\!\(\)]*)"; //@"(\d\d\d\d\d\d)";

    /// <summary>
    ///   The product file name pattern.
    /// </summary>
    public static readonly string ProductFileNamePattern = ProductHelper.Settings.CoreProductNamePattern.Value.EmptyToNull() ?? ProductNamePattern + @"[\s]?[\-_]?[\s]?" + ProductVersionPattern + @"[\s\-]*(rev\.|build)[\s]*" + ProductRevisionPattern + @"(.zip)?$";

    /// <summary>
    ///   The product name pattern.
    /// </summary>
    private const string ProductNamePattern = @"([a-zA-Z][a-zA-Z\d\-\s\._]*[a-zA-Z0-9])";

    /// <summary>
    ///   The product version pattern.
    /// </summary>
    private const string ProductVersionPattern = @"(\d\.\d(\.?\d?))";

    #endregion

    #region Fields

    /// <summary>
    ///   The product regex.
    /// </summary>
    public static readonly Regex ProductRegex = new Regex(ProductFileNamePattern, RegexOptions.IgnoreCase);

    /// <summary>
    ///   The undefined.
    /// </summary>
    public static readonly Product Undefined = new Product()
    {
      Name = "Undefined",
      OriginalName = "Undefined",
      PackagePath = string.Empty,
      IsStandalone = true,
      Version = string.Empty,
      ShortName = "undefined",
      Revision = string.Empty
    };

    public readonly static XmlDocumentEx EmptyManifest = XmlDocumentEx.LoadXml("<manifest version=\"1.3\" />");

    /// <summary>
    ///   The manifest.
    /// </summary>
    private XmlDocument manifest;

    private Dictionary<bool, string> readme; 

    /// <summary>
    ///   The short version.
    /// </summary>
    private string shortVersion;

    private bool? isPackage;
    private string packagePath;
    private int? sortOrder;
    private bool? isStandalone;
    private string name;
    private string shortName;

    public virtual int SortOrder
    {
      get { return this.sortOrder ?? (int)(this.sortOrder = int.Parse(this.Manifest.With(m => m.SelectSingleElement(ManifestPrefix + "*/sortOrder")).With(m => m.InnerText.EmptyToNull()) ?? "0")); }
    }

    #endregion

    #region Public properties

    /// <summary>
    ///   Gets a value indicating whether SkipPostActions.
    /// </summary>
    public bool SkipPostActions
    {
      get
      {
        XmlDocument document = this.Manifest;
        if (document == null)
        {
          return false;
        }

        XmlElement install = document.SelectSingleElement(ManifestPrefix + "package/install/postStepActions");
        if (install != null)
        {
          string skip = install.GetAttribute("skipStandard");
          if (!string.IsNullOrEmpty(skip) && skip.EqualsIgnoreCase("true"))
          {
            return true;
          }
        }

        return false;
      }
    }

    #endregion

    #region Properties

    #region Public properties

    /// <summary>
    ///   Gets DefaultInstanceName.
    /// </summary>
    [NotNull]
    public string DefaultInstanceName
    {
      get { return this.FormatString(ProductHelper.Settings.CoreInstallInstanceNamePattern.Value); }
    }

    // Used when computing default standalone instance name
    [NotNull]
    public string DefaultFolderName
    {
      get { return this.FormatString(ProductHelper.Settings.CoreProductRootFolderNamePattern.Value); }
    }

    /// <summary>
    ///   Gets a value indicating whether IsArchive.
    /// </summary>
    public bool IsArchive
    {
      get
      {
        return (bool)(this.isArchive ??
          (
            this.isArchive =
            (
              !string.IsNullOrEmpty(this.PackagePath) && (this.Manifest.With(x => x.SelectSingleElement(ManifestPrefix + "archive")) != null || !this.IsPackage && !this.IsStandalone)
            )
          ));
      }
    }

    /// <summary>
    ///   Gets a value indicating whether IsPackage.
    /// </summary>
    public bool IsPackage
    {
      get { return (bool)(this.isPackage ?? (this.isPackage = GetIsPackage(this.PackagePath, this.Manifest))); }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether IsStandalone.
    /// </summary>
    public bool IsStandalone
    {
      get
      {
        return (bool)(this.isStandalone ?? (this.isStandalone = this.Manifest.With(m => m.SelectSingleElement(ManifestPrefix + "standalone")) != null));
      }
      set { this.isStandalone = value; }
    }

    /// <summary>
    ///   Gets Manifest.
    /// </summary>
    [CanBeNull]
    public XmlDocument Manifest
    {
      get
      {
        if (this.manifest == null)
        {
          string packageFile = this.PackagePath;
          if (string.IsNullOrEmpty(packageFile)) return EmptyManifest;

          // get from cache
          const string cacheName = "manifest";
          var text = CacheManager.GetEntry(cacheName, packageFile);
          if (text != null)
          {
            this.manifest = XmlDocumentEx.LoadXml(text);
            return this.manifest;
          }

          this.manifest = ManifestHelper.Compute(packageFile, this.OriginalName);

          // put to cache
          CacheManager.SetEntry(cacheName, packageFile, manifest.OuterXml);
        }

        return this.manifest;
      }
    }

    /// <summary>
    ///   Gets or sets ManifestBaseFilePath.
    /// </summary>
    private bool? isArchive;

    public static readonly XmlDocumentEx ArchiveManifest = XmlDocumentEx.LoadXml(@"<manifest version=""1.3"">
  <archive>
    <install>
      <actions>
        <extract />
      </actions>
    </install>
  </archive>
</manifest>");

    public static readonly XmlDocumentEx PackageManifest = XmlDocumentEx.LoadXml(@"<manifest version=""1.3"">
  <package />
</manifest>");

    private string searchToken;

    /// <summary>
    ///   Gets or sets Name.
    /// </summary>
    [NotNull]
    public string Name
    {
      get { return this.name ?? (this.name = this.Manifest.With(m => m.SelectSingleElement(ManifestPrefix + "*/name")).With(x => x.InnerText.EmptyToNull()) ?? NormalizeName(this.OriginalName)); }
      set { this.name = NormalizeName(value); }
    }

    private string NormalizeName(string name)
    {
      string result = string.Empty;
      var words = name.Split();
      foreach (var word in words)
      {
        if (!string.IsNullOrEmpty(word))
        {
          result += " " + char.ToUpperInvariant(word[0]) + (word.Length > 1 ? word.Substring(1) : string.Empty);
        }
      }

      return result.TrimStart();
    }

    /// <summary>
    ///   Gets or sets PackagePath.
    /// </summary>
    [NotNull]
    public string PackagePath
    {
      get { return this.packagePath; }
      set
      {
        this.packagePath = value;
        this.isPackage = GetIsPackage(value, this.Manifest);
      }
    }

    public Dictionary<bool, string> Readme
    {
      get
      {
        if (readme == null)
        {
          return readme = GetReadme();
        }

        return readme;
      }
    }

    /// <summary>
    ///   Revision is typically YYMMDD (e.g. 130529) but also could be YYMMDD_postfix (e.g. 130529_oracle)
    /// </summary>
    [NotNull]
    public string Revision
    {
      get;
      set;
    }

    /// <summary>
    ///   Gets RevisionAndLabel.
    /// </summary>
    [NotNull]
    public string RevisionAndLabel
    {
      get
      {
        if (this.Manifest != null && this.Manifest != EmptyManifest)
        {
          string label = this.Label;
          if (!string.IsNullOrEmpty(label))
          {
            return this.Revision + " - " + label;
          }
        }

        return this.Revision;
      }
    }

    [CanBeNull]
    public string Label
    {
      get
      {
        return this.Manifest.With(m => m.SelectSingleElement(ManifestPrefix + "*/label").With(x => x.InnerText));
      }
    }

    /// <summary>
    ///   Gets or sets ShortName.
    /// </summary>
    [NotNull]
    public string ShortName
    {
      get
      {
        return this.shortName ??
               (this.shortName = this.Manifest.With(m => m.SelectSingleElement(ManifestPrefix + "*/shortName")).With(m => m.InnerText.EmptyToNull()) ??
                this.Name.Split(' ')
                    .Aggregate(string.Empty,
                               (current, word) =>
                               current +
                               (word.Length > 0 ? word[0].ToString(CultureInfo.InvariantCulture) : string.Empty))
                    .ToLower());
      }
      set { this.shortName = value; }
    }

    /// <summary>
    ///   Gets ShortVersion.
    /// </summary>
    [NotNull]
    public string ShortVersion
    {
      get
      {
        return this.shortVersion ?? (this.shortVersion = this.Version.Replace(".", string.Empty));
      }
    }

    /// <summary>
    ///   Gets or sets Version.
    /// </summary>
    [NotNull]
    public string Version
    {
      get;
      set;
    }

    /// <summary>
    ///   Gets VersionAndRevision.
    /// </summary>
    [NotNull]
    public string VersionAndRevision
    {
      get
      {
        const string Pattern = "{0} rev. {1}";
        var longVersion = this.Version.Length == "7.0".Length ? this.Version + ".0" : this.Version;
        return Pattern.FormatWith(longVersion, this.Revision).TrimEnd(" rev. ");
      }
    }

    #endregion

    #region Private properties

    /// <summary>
    ///   Gets or sets OriginalName.
    /// </summary>
    public string OriginalName
    {
      get;
      set;
    }

    public string SearchToken
    {
      get { return this.searchToken ?? (this.searchToken = this.ShortName + this.shortVersion + this.ToString()); }
    }

    #endregion

    #endregion

    #region Public Methods

    private Dictionary<bool, string> GetReadme()
    {
      var readmeNode = this.Manifest.GetElementsByTagName("readme")[0];
      if (readmeNode != null && !string.IsNullOrEmpty(readmeNode.InnerText))
      {
        return new Dictionary<bool, string> { { true, readmeNode.InnerText } };
      }

      var tempExtractFolderPath = Path.Combine(Directory.GetParent(PackagePath).FullName, "TempExtract");
      if (!Directory.Exists(tempExtractFolderPath)) Directory.CreateDirectory(tempExtractFolderPath);

      using (var zip = ZipFile.Read(this.PackagePath))
      {
        ZipEntry readmeEntry;

        var packageEntry = zip["package.zip"];

        if (packageEntry != null)
        {
          packageEntry.Extract(tempExtractFolderPath, ExtractExistingFileAction.OverwriteSilently);

          using (var packageZip = ZipFile.Read(Path.Combine(tempExtractFolderPath, "package.zip")))
          {
            readmeEntry = packageZip["metadata/sc_readme.txt"];
            if (readmeEntry != null) readmeEntry.Extract(tempExtractFolderPath, ExtractExistingFileAction.OverwriteSilently);
          }
        }
        else
        {
          readmeEntry = zip["metadata/sc_readme.txt"];
          if (readmeEntry != null) readmeEntry.Extract(tempExtractFolderPath, ExtractExistingFileAction.OverwriteSilently);
        }
      }

      string readmeText;

      var path = Path.Combine(tempExtractFolderPath, "metadata", "sc_readme.txt");
      try
      {
        readmeText = File.ReadAllText(path);
      }
      catch (Exception ex)
      {
        Log.Warn("An error occurred during extracting readme text from " + path, this, ex);
        readmeText = string.Empty;
      }

      Directory.Delete(tempExtractFolderPath, true);
      return new Dictionary<bool, string> { { false, readmeText } };
    }

    /// <summary>
    /// The parse.
    /// </summary>
    /// <param name="path">
    /// The path. 
    /// </param>
    /// <returns>
    /// The parse 
    /// </returns>
    [NotNull]
    public static Product Parse([NotNull] string path)
    {
      Assert.ArgumentNotNull(path, "path");

      Product product;
      return TryParse(path, out product) && product != null ? product : Undefined;
    }

    /// <summary>
    /// The try parse.
    /// </summary>
    /// <param name="packagePath">
    /// The file. 
    /// </param>
    /// <param name="product">
    /// The product. 
    /// </param>
    /// <returns>
    /// The try parse 
    /// </returns>
    public static bool TryParse([NotNull] string packagePath, [CanBeNull] out Product product)
    {
      Assert.ArgumentNotNull(packagePath, "file");

      product = null;
      Match match = ProductRegex.Match(packagePath);
      if (!match.Success)
      {
        return false;
      }

      return TryParse(packagePath, match, out product);
    }

    /// <summary>
    /// The is match requirements.
    /// </summary>
    /// <param name="instanceProduct">
    /// The instance product. 
    /// </param>
    /// <returns>
    /// The is match requirements 
    /// </returns>
    public bool IsMatchRequirements([NotNull] Product instanceProduct)
    {
      Assert.ArgumentNotNull(instanceProduct, "instanceProduct");

      //!ProductHelper.Settings.InstallModulesCheckRequirements.Value&& 
      if (!this.Name.EqualsIgnoreCase("Sitecore Analytics"))
      {
        return true;
      }

      foreach (XmlElement product in this.Manifest.SelectElements(ManifestPrefix + "*/requirements/product"))
      {
        IEnumerable<XmlElement> rules = product.ChildNodes.OfType<XmlElement>().ToArray();
        foreach (IGrouping<string, XmlElement> group in rules.GroupBy(ch => ch.Name.ToLower()))
        {
          string key = group.Key;
          switch (key)
          {
            case "version":
              if (rules.Where(r => r.Name.ToLower() == key).All(s => !this.VersionMatch(instanceProduct, s.GetAttribute("value"), s)))
              {
                return false;
              }

              break;
            case "revision":
              if (rules.Where(r => r.Name.ToLower() == key).All(s => !this.RevisionMatch(instanceProduct, s.GetAttribute("value"))))
              {
                return false;
              }

              break;
          }
        }
      }

      return true;
    }

    /// <summary>
    ///   Used for searching product by its name
    /// </summary>
    /// <returns> The to string. </returns>
    public override string ToString()
    {
      return this.DisplayName;
    }

    [NotNull]
    public string DisplayName
    {
      get
      {
      const string Pattern = "{0} {1} rev. {2}";
        return Pattern.FormatWith(this.Name, this.Version, this.Revision).TrimEnd(" rev. ").TrimEnd(' ');
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The try parse.
    /// </summary>
    /// <param name="packagePath">
    /// The file. 
    /// </param>
    /// <param name="match">
    /// The match. 
    /// </param>
    /// <param name="product">
    /// The product. 
    /// </param>
    /// <returns>
    /// The try parse 
    /// </returns>
    private static bool TryParse([NotNull] string packagePath, [NotNull] Match match, [CanBeNull] out Product product)
    {
      Assert.ArgumentNotNull(packagePath, "file");
      Assert.ArgumentNotNull(match, "match");

      product = null;
      if (!FileSystem.Local.File.Exists(packagePath))
      {
        packagePath = null;
      }

      string originalName = match.Groups[1].Value;
      string name = originalName;
      string shortName = null;
      string version = match.Groups[2].Value;
      string revision = match.Groups[5].Value;

      if (name.EqualsIgnoreCase("sitecore") && version[0] == '5')
      {
        return false;
      };

      product = ProductManager.Products.FirstOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && p.OriginalName.Equals(originalName) && p.ShortName.EqualsIgnoreCase(shortName) && p.Revision.EqualsIgnoreCase(revision))
        ?? new Product
      {
        OriginalName = originalName,
        PackagePath = packagePath,
        Version = version,
        Revision = revision
      };

      return true;
    }

    /// <summary>
    /// The format string.
    /// </summary>
    /// <param name="pattern">
    /// The pattern. 
    /// </param>
    /// <returns>
    /// The format string 
    /// </returns>
    [NotNull]
    private string FormatString([NotNull] string pattern)
    {
      Assert.ArgumentNotNull(pattern, "pattern");

      return pattern.Replace("{ShortName}", this.ShortName).Replace("{Name}", this.Name).Replace("{ShortVersion}", this.ShortVersion).Replace("{Version}", this.Version).Replace("{Revision}", this.Revision);
    }

    /// <summary>
    /// The revision match.
    /// </summary>
    /// <param name="instanceProduct">
    /// The instance product. 
    /// </param>
    /// <param name="revision">
    /// The revision. 
    /// </param>
    /// <returns>
    /// The revision match 
    /// </returns>
    private bool RevisionMatch([NotNull] Product instanceProduct, [NotNull] string revision)
    {
      Assert.ArgumentNotNull(instanceProduct, "instanceProduct");
      Assert.ArgumentNotNull(revision, "revision");

      if (string.IsNullOrEmpty(revision))
      {
        revision = this.Revision;
      }

      string instanceRevision = instanceProduct.Revision;
      if (instanceRevision == revision || string.IsNullOrEmpty(instanceRevision))
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// The version match.
    /// </summary>
    /// <param name="instanceProduct">
    /// The instance product. 
    /// </param>
    /// <param name="version">
    /// The version. 
    /// </param>
    /// <param name="versionRule">
    /// The version rule. 
    /// </param>
    /// <returns>
    /// The version match 
    /// </returns>
    private bool VersionMatch([NotNull] Product instanceProduct, [NotNull] string version, [NotNull] XmlElement versionRule)
    {
      Assert.ArgumentNotNull(instanceProduct, "instanceProduct");
      Assert.ArgumentNotNull(version, "version");
      Assert.ArgumentNotNull(versionRule, "versionRule");

      if (string.IsNullOrEmpty(version))
      {
        version = this.Version;
      }

      string instanceVersion = instanceProduct.Version;
      if (instanceVersion == version)
      {
        var rules = versionRule.SelectElements("revision").ToArray();
        if (rules.Length == 0)
        {
          return true;
        }

        if (rules.Any(s => this.RevisionMatch(instanceProduct, s.GetAttribute("value"))))
        {
          return true;
        }
      }

      return false;
    }

    #endregion

    public static Product GetFilePackageProduct(string packagePath)
    {
      return FileSystem.Local.File.Exists(packagePath) ? new Product { PackagePath = packagePath, IsStandalone = false, Name = Path.GetFileName(packagePath) } : null;
    }

    System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
    {
      throw new NotImplementedException();
    }

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
      throw new NotImplementedException();
    }

    void IXmlSerializable.WriteXml(XmlWriter writer)
    {
      foreach (var property in this.GetType().GetProperties())
      {
        object value = property.GetValue(this, new object[0]);
        var xml = value as XmlDocument;
        if (xml != null)
        {
          writer.WriteNode(new XmlNodeReader(XmlDocumentEx.LoadXml("<Manifest>" + xml.OuterXml + "</Manifest>")), false);
          continue;
        }

        writer.WriteElementString(property.Name, string.Empty, (value ?? string.Empty).ToString());
      }
    }
  }
}