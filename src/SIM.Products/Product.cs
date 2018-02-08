namespace SIM.Products
{
  #region

  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using System.Xml;
  using System.Xml.Schema;
  using System.Xml.Serialization;
  using Ionic.Zip;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base.Extensions.DictionaryExtensions;
  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.Diagnostics.InfoService.Client.Model;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  #endregion

  [Serializable]
  public class Product : IXmlSerializable
  {
    #region Constants

    public const string ManifestPrefix = "/manifest/";

    private const string ProductNamePattern = @"([a-zA-Z][a-zA-Z\d\-\s\._]*[a-zA-Z0-9])";

    private const string ProductRevisionPattern = @"(\d\d\d\d\d\d([\d\w\s_\-\!\(\)]*|[\d\w\s_\-\!\(\)]+\.[\d\w\s_\-\!\(\)]+))"; // @"(\d\d\d\d\d\d)";

    private const string ProductVersionPattern = @"(\d\.\d(\.?\d?))";

    #endregion

    #region Fields

    public static XmlDocumentEx EmptyManifest { get; } = XmlDocumentEx.LoadXml("<manifest version=\"1.4\" />");

    public static string ProductFileNamePattern { get; } = $@"{ProductNamePattern}[\s]?[\-_]?[\s]?{ProductVersionPattern}[\s\-]*(rev\.|build)[\s]*{ProductRevisionPattern}(.zip)?$";

    public static Regex ProductRegex { get; } = new Regex(ProductFileNamePattern, RegexOptions.IgnoreCase);

    public static Product Undefined { get; } = new Product()
    {
      Name = "Undefined", 
      OriginalName = "Undefined", 
      PackagePath = string.Empty, 
      IsStandalone = true, 
      ShortName = "undefined", 
      TwoVersion = string.Empty,
      TriVersion = string.Empty,
      Revision = string.Empty
    };

    private bool? _IsPackage;
    private bool? _IsStandalone;
    private XmlDocument _Manifest;
    private string _Name;
    private string _PackagePath;
    private Dictionary<bool, string> _Readme;
    private string _ShortName;
    private string _ShortVersion;
    private int? _SortOrder;

    #endregion

    #region Public properties

    public bool SkipPostActions
    {
      get
      {
        XmlDocument document = Manifest;
        if (document == null)
        {
          return false;
        }

        XmlElement install = document.SelectSingleElement(ManifestPrefix + "package/install/postStepActions");
        if (install != null)
        {
          var skip = install.GetAttribute("skipStandard");
          if (!string.IsNullOrEmpty(skip) && skip.EqualsIgnoreCase("true"))
          {
            return true;
          }
        }

        return false;
      }
    }

    public virtual int SortOrder
    {
      get
      {
        return _SortOrder ?? (int)(_SortOrder = int.Parse(Manifest.With(m => m.SelectSingleElement(ManifestPrefix + "*/sortOrder")).With(m => m.InnerText.EmptyToNull()) ?? "0"));
      }
    }

    #endregion

    #region Properties

    // Used when computing default standalone instance name
    #region Fields

    public static XmlDocumentEx ArchiveManifest { get; } = XmlDocumentEx.LoadXml(@"<manifest version=""1.4"">
  <archive>
    <install>
      <actions>
        <extract />
      </actions>
    </install>
  </archive>
</manifest>");

    public static XmlDocumentEx PackageManifest { get; } = XmlDocumentEx.LoadXml(@"<manifest version=""1.4"">
  <package />
</manifest>");
    private bool? _IsArchive;
    private string _SearchToken;
    public static IServiceClient Service { get; } = new ServiceClient();

    [CanBeNull]
    private IRelease _Release;

    private bool _UnknownRelease;

    #endregion

    #region Public properties      

    [NotNull]
    public string DefaultInstanceName
    {
      get
      {
        return FormatString(ProductHelper.Settings.CoreInstallInstanceNamePattern.Value);
      }
    }

    public bool IsArchive
    {
      get
      {
        return (bool)(_IsArchive ??
                      (
                        _IsArchive =
                          !string.IsNullOrEmpty(PackagePath) && (Manifest.With(x => x.SelectSingleElement(ManifestPrefix + "archive")) != null || !IsPackage && !IsStandalone)
                        ));
      }
    }

    public bool IsPackage
    {
      get
      {
        return (bool)(_IsPackage ?? (_IsPackage = GetIsPackage(PackagePath, Manifest)));
      }
    }

    public bool IsStandalone
    {
      get
      {
        return (bool)(_IsStandalone ?? (_IsStandalone = Manifest.With(m => m.SelectSingleElement(ManifestPrefix + "standalone")) != null));
      }

      set
      {
        _IsStandalone = value;
      }
    }

    [CanBeNull]
    public string Label
    {
      get
      {
        return Manifest.With(m => m.SelectSingleElement(ManifestPrefix + "*/label").With(x => x.InnerText)) ?? Release.With(x => x.Label);
      }
    }

    [CanBeNull]
    public IRelease Release
    {
      get
      {
        var release = _Release;
        if (release != null)
        {
          return release;
        }

        if (_UnknownRelease || Name != "Sitecore CMS")
        {
          return null;
        }


        var ver = TriVersion;
        switch (ver)
        {
          case "6.6.0":
            ver = "6.6";
            break;
          case "6.5.0":
            ver = "6.5";
            break;
        }

        try
        {
          try
          {
            release = Service.GetRelease("Sitecore CMS", ver, Revision);
          }
          catch
          {
            release = Service.GetRelease("Sitecore CMS", ver);
          }
        }
        catch (Exception ex)
        {
          Log.Error(ex, $"Failed to find Sitecore CMS {ver} (rev. {Revision})");

          _UnknownRelease = true;

          return null;
        }

        _Release = release;

        return release;
      }
    }

    [CanBeNull]
    public XmlDocument Manifest
    {
      get
      {
        if (_Manifest == null)
        {
          var packageFile = PackagePath;
          if (string.IsNullOrEmpty(packageFile))
          {
            return EmptyManifest;
          }

          // get from cache
          const string CacheName = "manifest";
          var text = CacheManager.GetEntry(CacheName, packageFile);
          if (text != null)
          {
            _Manifest = XmlDocumentEx.LoadXml(text);
            return _Manifest;
          }

          _Manifest = ManifestHelper.Compute(packageFile, OriginalName);

          // put to cache
          CacheManager.SetEntry(CacheName, packageFile, _Manifest.OuterXml);
        }

        return _Manifest;
      }
    }

    [NotNull]
    public string Name
    {
      get
      {
        return _Name ?? (_Name = Manifest.With(m => m.SelectSingleElement(ManifestPrefix + "*/name")).With(x => x.InnerText.EmptyToNull()) ?? NormalizeName(OriginalName));
      }

      set
      {
        _Name = NormalizeName(value);
      }
    }

    public string OriginalName { get; set; }

    [NotNull]
    public string PackagePath
    {
      get
      {
        return _PackagePath;
      }

      set
      {
        _PackagePath = value;
        _IsPackage = GetIsPackage(value, Manifest);
      }
    }

    //maybe used implicitly
    [UsedImplicitly]
    public Dictionary<bool, string> Readme
    {
      get
      {
        if (_Readme == null)
        {
          return _Readme = GetReadme();
        }

        return _Readme;
      }
    }

    [NotNull]
    public string Revision { get; set; }

    [NotNull]
    [UsedImplicitly]
    public string RevisionAndLabel
    {
      get
      {
        var label = Label;
        if (!string.IsNullOrEmpty(label))
        {
          return $"{Revision} - {label}";
        }

        return Revision;
      }
    }

    public string SearchToken
    {
      get
      {
        return _SearchToken ?? (_SearchToken = ShortName + _ShortVersion + ToString());
      }
    }

    [NotNull]
    public string ShortName
    {
      get
      {
        return _ShortName ??
               (_ShortName = Manifest.With(m => m.SelectSingleElement(ManifestPrefix + "*/shortName")).With(m => m.InnerText.EmptyToNull()) ??
                                 Name.Split(' ')
                                   .Aggregate(string.Empty, 
                                     (current, word) =>
                                       current +
                                       (word.Length > 0 ? word[0].ToString(CultureInfo.InvariantCulture) : string.Empty))
                                   .ToLower());
      }

      set
      {
        _ShortName = value;
      }
    }

    [NotNull]
    public string ShortVersion
    {
      get
      {
        return _ShortVersion ?? (_ShortVersion = TwoVersion.Replace(".", string.Empty));
      }
    }

    [NotNull]
    public string TwoVersion { get; set; }

    [NotNull]
    public string VersionAndRevision
    {
      get
      {
        const string Pattern = "{0} rev. {1}";
        var longVersion = TwoVersion.Length == "7.0".Length ? TwoVersion + ".0" : TwoVersion;
        return Pattern.FormatWith(longVersion, Revision).TrimEnd(" rev. ");
      }
    }

    [NotNull]
    public string UpdateOrRevision
    {
      get
      {
        try
        {
          return $"{Update}";
        }
        catch (Exception ex)
        {
          Log.Warn(ex, "Failed to retrieve Update, using Revision instead");

          return $"r{Revision}";
        }
      }
    }

    #endregion

    #region Private methods

    private string NormalizeName(string name)
    {
      var result = string.Empty;
      var words = name.Split();
      foreach (var word in words)
      {
        if (!string.IsNullOrEmpty(word))
        {
          result += $" {char.ToUpperInvariant(word[0])}{(word.Length > 1 ? word.Substring(1) : string.Empty)}";
        }
      }

      return result.TrimStart();
    }

    #endregion

    #endregion

    #region Public Methods

    #region Public properties

    [NotNull]
    public string DisplayName
    {
      get
      {
        return ToString(true);
      }
    }

    public int Update => Release.Version.Update;

    #endregion

    #region Public methods

    [NotNull]
    public static Product Parse([NotNull] string path)
    {
      Assert.ArgumentNotNull(path, nameof(path));

      Product product;
      return TryParse(path, out product) && product != null ? product : Undefined;
    }

    public static bool TryParse([NotNull] string packagePath, [CanBeNull] out Product product)
    {
      Assert.ArgumentNotNull(packagePath, nameof(packagePath));

      product = null;
      Match match = ProductRegex.Match(packagePath);
      if (!match.Success)
      {
        return false;
      }

      return TryParse(packagePath, match, out product);
    }

    public bool IsMatchRequirements([NotNull] Product instanceProduct)
    {
      Assert.ArgumentNotNull(instanceProduct, nameof(instanceProduct));

      // !ProductHelper.Settings.InstallModulesCheckRequirements.Value&& 
      if (!Name.EqualsIgnoreCase("Sitecore Analytics"))
      {
        return true;
      }

      foreach (XmlElement product in Manifest.SelectElements(ManifestPrefix + "*/requirements/product"))
      {
        IEnumerable<XmlElement> rules = product.ChildNodes.OfType<XmlElement>().ToArray();
        foreach (IGrouping<string, XmlElement> group in rules.GroupBy(ch => ch.Name.ToLower()))
        {
          var key = group.Key;
          switch (key)
          {
            case "version":
              if (rules.Where(r => r.Name.ToLower() == key).All(s => !VersionMatch(instanceProduct, s.GetAttribute("value"), s)))
              {
                return false;
              }

              break;
            case "revision":
              if (rules.Where(r => r.Name.ToLower() == key).All(s => !RevisionMatch(instanceProduct, s.GetAttribute("value"))))
              {
                return false;
              }

              break;
          }
        }
      }

      return true;
    }

    public override string ToString()
    {
      return ToString(false);
    }

    public virtual string ToString(bool triVersion)
    {
      const string Pattern = "{0} {1} rev. {2}";

      return Pattern.FormatWith(Name, triVersion ? TriVersion : TwoVersion, Revision).TrimEnd(" rev. ").TrimEnd(' ');
    }

    #endregion

    #region Private methods

    private Dictionary<bool, string> GetReadme()
    {
      var readmeNode = Manifest.GetElementsByTagName("readme")[0];
      if (readmeNode != null && !string.IsNullOrEmpty(readmeNode.InnerText))
      {
        return new Dictionary<bool, string>
        {
          {
            true, readmeNode.InnerText
          }
        };
      }

      var tempExtractFolderPath = Path.Combine(Directory.GetParent(PackagePath).FullName, "TempExtract");
      if (!Directory.Exists(tempExtractFolderPath))
      {
        Directory.CreateDirectory(tempExtractFolderPath);
      }

      using (var zip = ZipFile.Read(PackagePath))
      {
        ZipEntry readmeEntry;

        var packageEntry = zip["package.zip"];

        if (packageEntry != null)
        {
          packageEntry.Extract(tempExtractFolderPath, ExtractExistingFileAction.OverwriteSilently);

          using (var packageZip = ZipFile.Read(Path.Combine(tempExtractFolderPath, "package.zip")))
          {
            readmeEntry = packageZip["metadata/sc_readme.txt"];
            if (readmeEntry != null)
            {
              readmeEntry.Extract(tempExtractFolderPath, ExtractExistingFileAction.OverwriteSilently);
            }
          }
        }
        else
        {
          readmeEntry = zip["metadata/sc_readme.txt"];
          if (readmeEntry != null)
          {
            readmeEntry.Extract(tempExtractFolderPath, ExtractExistingFileAction.OverwriteSilently);
          }
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
        Log.Warn(ex, $"An error occurred during extracting readme text from {path}");
        readmeText = string.Empty;
      }

      Directory.Delete(tempExtractFolderPath, true);
      return new Dictionary<bool, string>
      {
        {
          false, readmeText
        }
      };
    }

    #endregion

    #endregion

    #region Methods

    private static bool TryParse([NotNull] string packagePath, [NotNull] Match match, [CanBeNull] out Product product)
    {
      Assert.ArgumentNotNull(packagePath, nameof(packagePath));
      Assert.ArgumentNotNull(match, nameof(match));

      product = null;
      if (!FileSystem.FileSystem.Local.File.Exists(packagePath))
      {
        packagePath = null;
      }

      var originalName = match.Groups[1].Value;
      var name = originalName;
      string shortName = null;
      var version = match.Groups[2].Value;
      var revision = match.Groups[5].Value;

      if (name.EqualsIgnoreCase("sitecore") && version[0] == '5')
      {
        return false;
      }
      
      var arr = version.Split('.');

      product = ProductManager.Products.FirstOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && p.OriginalName.Equals(originalName) && p.ShortName.EqualsIgnoreCase(shortName) && p.Revision.EqualsIgnoreCase(revision))
                ?? new Product
                {
                  OriginalName = originalName, 
                  PackagePath = packagePath, 
                  TwoVersion = $"{arr[0]}.{arr[1]}",
                  TriVersion = version,
                  Revision = revision
                };

      return true;
    }

    public string TriVersion { get; set; }

    [NotNull]
    private string FormatString([NotNull] string pattern)
    {
      Assert.ArgumentNotNull(pattern, nameof(pattern));

      return pattern.Replace("{ShortName}", ShortName).Replace("{Name}", Name).Replace("{ShortVersion}", ShortVersion).Replace("{Version}", TwoVersion).Replace("{Revision}", Revision)
        .Replace("{UpdateOrRevision}", UpdateOrRevision);
    }

    private bool RevisionMatch([NotNull] Product instanceProduct, [NotNull] string revision)
    {
      Assert.ArgumentNotNull(instanceProduct, nameof(instanceProduct));
      Assert.ArgumentNotNull(revision, nameof(revision));

      if (string.IsNullOrEmpty(revision))
      {
        revision = Revision;
      }

      var instanceRevision = instanceProduct.Revision;
      if (instanceRevision == revision || string.IsNullOrEmpty(instanceRevision))
      {
        return true;
      }

      return false;
    }

    private bool VersionMatch([NotNull] Product instanceProduct, [NotNull] string version, [NotNull] XmlElement versionRule)
    {
      Assert.ArgumentNotNull(instanceProduct, nameof(instanceProduct));
      Assert.ArgumentNotNull(version, nameof(version));
      Assert.ArgumentNotNull(versionRule, nameof(versionRule));

      if (string.IsNullOrEmpty(version))
      {
        version = TwoVersion;
      }

      var instanceVersion = instanceProduct.TwoVersion;
      if (instanceVersion == version)
      {
        var rules = versionRule.SelectElements("revision").ToArray();
        if (rules.Length == 0)
        {
          return true;
        }

        if (rules.Any(s => RevisionMatch(instanceProduct, s.GetAttribute("value"))))
        {
          return true;
        }
      }

      return false;
    }

    #endregion

    #region Public methods

    public static Product GetFilePackageProduct(string packagePath)
    {
      return FileSystem.FileSystem.Local.File.Exists(packagePath) ? new Product
      {
        PackagePath = packagePath, 
        IsStandalone = false, 
        Name = Path.GetFileName(packagePath)
      } : null;
    }

    #endregion

    #region Private methods

    private static bool GetIsPackage(string packagePath, XmlDocument manifest)
    {
      if (string.IsNullOrEmpty(packagePath))
      {
        return false;
      }

      const string CacheName = "IsPackage";
      var path = packagePath.ToLowerInvariant();
      using (new ProfileSection("Is it package or not"))
      {
        ProfileSection.Argument("packagePath", packagePath);
        ProfileSection.Argument("manifest", manifest);

        try
        {
          // cache            
          var entry = CacheManager.GetEntry(CacheName, path);
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
            CacheManager.SetEntry(CacheName, path, "false");
            return ProfileSection.Result(false);
          }

          if (
            manifest.With(
              x =>
                x.SelectSingleElement(ManifestPrefix + "package")) !=
            null)
          {
            CacheManager.SetEntry(CacheName, path, "true");

            return ProfileSection.Result(true);
          }

          CacheManager.SetEntry(CacheName, path, "false");

          return ProfileSection.Result(false);
        }
        catch (Exception e)
        {
          Log.Warn(string.Format("Detecting if the '{0}' file is a Sitecore Package failed with exception.", path, e));
          CacheManager.SetEntry(CacheName, path, "false");

          return ProfileSection.Result(false);
        }
      }
    }

    XmlSchema IXmlSerializable.GetSchema()
    {
      throw new NotImplementedException();
    }

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
      throw new NotImplementedException();
    }

    void IXmlSerializable.WriteXml(XmlWriter writer)
    {
      foreach (var property in GetType().GetProperties())
      {
        object value = property.GetValue(this, new object[0]);
        var xml = value as XmlDocument;
        if (xml != null)
        {
          writer.WriteNode(new XmlNodeReader(XmlDocumentEx.LoadXml($"<Manifest>{xml.OuterXml}</Manifest>")), false);
          continue;
        }

        writer.WriteElementString(property.Name, string.Empty, (value ?? string.Empty).ToString());
      }
    }

    #endregion

    /// <summary>
    /// Force manifest to be reloaded from disk, to reset variable replacements.
    /// </summary>
    public void ResetManifest()
    {
      _Manifest = null;
    }
  }
}
