using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Sitecore.Diagnostics.Base;
using SIM.Instances;
using SIM.Pipelines.InstallModules;
using SIM.Products;

namespace SIM.Pipelines.Install.Modules
{
  using SIM.Extensions;

  /// <summary>
  /// Creates cores for all configured Solr indexes
  /// </summary>
  public class CreateSolrCores : IPackageInstallActions
  {
    #region Constants

    private const string GenerateSchemaClass = "Sitecore.ContentSearch.ProviderSupport.Solr.SchemaGenerator";
    private const string GenerateSchemaMethod = "GenerateSchema";
    public const string SolrConfigPatch =
    @"<config>
        <requestHandler name=""/select"" class=""solr.SearchHandler"">
          <bool name=""terms"">true</bool>
          <lst name=""defaults"">
             <bool name=""terms"">true</bool>
          </lst>
          <arr name=""last-components"">
            <str>terms</str>
          </arr>
        </requestHandler>
      </config>";

    #endregion

    #region Public methods

    public void Execute(Instance instance, Product module)
    {
      XmlDocument sitecoreConfig = instance.GetShowconfig();
      string solrUrl = GetUrlFrom(sitecoreConfig);
      XmlNodeList solrIndexes = GetSolrIndexNodesFrom(sitecoreConfig);

      SolrInfoDto info = GetSolrInfo(solrUrl);

      string sourcePath = GetSourcePath(info, solrUrl);

      foreach (XmlElement node in solrIndexes)
      {
        string coreName = GetCoreName(node);
        var oldValue = info.Version == 4 ? "collection1" : @"configsets\basic_configs";
        string corePath = sourcePath.Replace(oldValue, coreName);
        this.CopyDirectory(sourcePath, corePath);
        DeleteCopiedCorePropertiesFile(corePath);
        UpdateSchema(instance.WebRootPath, corePath);
        UpdateSolrConfig(corePath);
        CallSolrCreateCoreAPI(solrUrl, coreName, corePath);
      }
    }

    #endregion

    #region Private methods

    private void UpdateSolrConfig(string corePath)
    {
      string filePath = corePath.EnsureEnd(@"\") + @"conf\solrconfig.xml";
      XmlDocumentEx mergedXml = this.XmlMerge(filePath, SolrConfigPatch);
      string mergedString = this.NormalizeXml(mergedXml);
      this.WriteAllText(filePath, mergedString);
    }

    private static XmlNodeList GetSolrIndexNodesFrom(XmlDocument config)
    {
      return config.SelectNodes(
        "/sitecore/contentSearch/configuration/indexes/index[@type='Sitecore.ContentSearch.SolrProvider.SolrSearchIndex, Sitecore.ContentSearch.SolrProvider']");
    }

    private static string GetUrlFrom(XmlDocument config)
    {
      XmlNode serviceBaseAddressNode = config.SelectSingleNode("/sitecore/settings/setting[@name='ContentSearch.Solr.ServiceBaseAddress']");
      serviceBaseAddressNode = Assert.IsNotNull(serviceBaseAddressNode,
        "ContentSearch.Solr.ServiceBaseAddress not found in configuration.");

      XmlAttribute valueAttribute = serviceBaseAddressNode.Attributes["value"];
      Assert.IsNotNull(valueAttribute, "ContentSearch.Solr.ServiceBaseAddress value attribute not found.");
      return valueAttribute.Value;
    }

    private void UpdateSchema(string webRootPath, string corePath)
    {
      string contentSearchDllPath = webRootPath.EnsureEnd(@"\") + @"bin\Sitecore.ContentSearch.dll";
      string schemaPath = corePath.EnsureEnd(@"\") + @"conf\schema.xml";
      string managedSchemaPath = corePath.EnsureEnd(@"\") + @"conf\managed-schema";

      bool schemaExists = FileExists(schemaPath);
      bool managedSchemaExists = FileExists(managedSchemaPath);

      if (!schemaExists && !managedSchemaExists)
      {
        throw new FileNotFoundException($"Schema file not found: Checked here {schemaPath} and here {managedSchemaPath}.");
      }

      string inputPath = managedSchemaExists ? managedSchemaPath : schemaPath;
      string outputPath = schemaPath;
      this.InvokeSitecoreGenerateSchemaUtility(contentSearchDllPath, inputPath, outputPath);
    }

    private static string GetCoreName(XmlElement node)
    {
      var coreElement = node.SelectSingleNode("param[@desc='core']") as XmlElement;
      string id = node.Attributes["id"].InnerText;
      coreElement = Assert.IsNotNull(coreElement, "param[@desc='core'] not found in Solr configuration file");
      string coreName = coreElement.InnerText.Replace("$(id)", id);
      return coreName;
    }

    private void CallSolrCreateCoreAPI(string url, string coreName, string instanceDir)
    {
      this.RequestAndGetResponseStream($"{url}/admin/cores?action=CREATE&name={coreName}&instanceDir={instanceDir}&config=solrconfig.xml&schema=schema.xml&dataDir=data");
    }

    private void DeleteCopiedCorePropertiesFile(string newCorePath)
    {
      string path = string.Format(newCorePath.EnsureEnd(@"\") + "core.properties");
      this.DeleteFile(path);
    }

    private SolrInfoDto GetSolrInfo(string url)
    {
      var response = this.RequestAndGetResponseStream($"{url}/admin/info/system");
      var doc = new XmlDocument();
      doc.Load(response);
      var node = doc.SelectSingleNode("/response/lst[@name='lucene']/str[@name='solr-spec-version']");

      var dto = new SolrInfoDto();
      if (node != null && !node.InnerText.IsNullOrEmpty()) dto.Version = GetMajorVersionFrom(node.InnerText);

      dto.SolrBasePath =
        doc.SelectSingleNode("/response/str[@name='solr_home']")?.InnerText ?? "";

      return dto;
    }

    private int? GetMajorVersionFrom(string version)
    {
      var r = new Regex(@"^(\d+)\.\d+.\d+");
      GroupCollection groups = r.Match(version).Groups;
      int value;
      if (groups.Count > 1 && int.TryParse(groups[1].Value, out value))
      {
        return value;
      }
      return null;
    }

    private string GetSourcePath(SolrInfoDto info, string url)
    {
      if (info.Version == 4)
      {
        var response = this.RequestAndGetResponseStream($"{url}/admin/cores");

        var doc = new XmlDocument();
        doc.Load(response);

        XmlNode collection1Node = doc.SelectSingleNode("/response/lst[@name='status']/lst[@name='collection1']");
        if (collection1Node == null) throw new ApplicationException("collection1 not found");

        return collection1Node.SelectSingleNode("str[@name='instanceDir']").InnerText;
      }
      else
      {
        return info.SolrBasePath.EnsureEnd(@"\") + @"configsets\basic_configs";
      }

    }

    private string NormalizeXml(XmlDocumentEx xml)
    {
      string outerXml = xml.OuterXml;
      string formatted = XmlDocumentEx.Normalize(outerXml);
      Regex r = new Regex(@"^<\?.*\?>");
      string corrected = r.Replace(formatted, @"<?xml version=""1.0"" encoding=""UTF-8"" ?>");  //Solr requires UTF-8.
      return corrected;
    }

    #endregion

    #region Virtual methods

    // All system calls are wrapped in virtual methods for unit testing.

    public virtual void CopyDirectory(string sourcePath, string destinationPath)
    {
      FileSystem.FileSystem.Local.Directory.Copy(sourcePath, destinationPath, recursive: true);
    }

    public virtual void WriteAllText(string path, string text)
    {
      FileSystem.FileSystem.Local.File.WriteAllText(path, text);
    }

    public virtual void DeleteFile(string path)
    {
      FileSystem.FileSystem.Local.File.Delete(path);
    }

    public virtual bool FileExists(string path)
    {
      return FileSystem.FileSystem.Local.File.Exists(path);
    }

    public virtual Stream RequestAndGetResponseStream(string url)
    {
      return WebRequestHelper.RequestAndGetResponse(url).GetResponseStream();
    }

    /// <summary>
    /// Dynamically loads GenerateSchema class from target site, and uses it
    /// to applied required changes to the Solr schema.xml file.
    /// </summary>
    public virtual void InvokeSitecoreGenerateSchemaUtility(string dllPath, string inputPath, string outputPath)
    {
      Assembly assembly = ReflectionUtil.GetAssembly(dllPath);
      Type generateSchema = ReflectionUtil.GetType(assembly, GenerateSchemaClass);
      object obj = ReflectionUtil.CreateObject(generateSchema);
      ReflectionUtil.InvokeMethod(obj, GenerateSchemaMethod, inputPath, outputPath);
    }

    public virtual XmlDocumentEx XmlMerge(string filePath, string xmlString)
    {
      XmlDocumentEx doc1 = XmlDocumentEx.LoadFile(filePath);
      XmlDocument doc2 = XmlDocumentEx.LoadXml(xmlString);
      return doc1.Merge(doc2);
    }

    #endregion
  }
}
