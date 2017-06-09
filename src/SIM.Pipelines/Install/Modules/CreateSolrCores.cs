using System;
using System.IO;
using System.Reflection;
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

    /// <summary>
    /// Patch to SolrConfig.xml to enable term support, as specified in Sitecore documentation.
    /// https://doc.sitecore.net/sitecore_experience_platform/82/setting_up__maintaining/search_and_indexing/walkthrough_setting_up_solr#_Enable_Solr_term
    /// </summary>
    public const string SolrTermSuppportPatch =
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
      SolrInformation info = GetSolrInformation(solrUrl);
      string sourcePath = info.TemplateFullPath;
       
      foreach (XmlElement node in solrIndexes)
      {
        string coreName = GetCoreName(node);
        string newCorePath = info.SolrBasePath.EnsureEnd(@"\") + coreName;
        CopyDirectory(sourcePath, newCorePath);
        RemoveCorePropertiesFile(newCorePath);
        UpdateSchema(instance.WebRootPath, newCorePath);
        UpdateSolrConfig(newCorePath);
        CreateSolrCore(solrUrl, coreName, newCorePath);
      }
    }

    #endregion

    #region Private methods

    private void UpdateSolrConfig(string corePath)
    {
      string filePath = corePath.EnsureEnd(@"\") + @"conf\solrconfig.xml";
      XmlDocumentEx mergedXml = XmlMerge(filePath, SolrTermSuppportPatch);
      EnsureClassicSchemaMode(mergedXml);
      RemoveAddSchemaFieldsProcessor(mergedXml);
      string mergedString = NormalizeXml(mergedXml);
      WriteAllText(filePath, mergedString);
    }

    private void EnsureClassicSchemaMode(XmlDocumentEx solrConfig)
    {
      var oldSchemaFactory = solrConfig.SelectSingleElement("/config/schemaFactory");
      var newSchemaFactory = solrConfig.CreateElement("schemaFactory");
      newSchemaFactory.SetAttribute("class", "ClassicIndexSchemaFactory");
      if (oldSchemaFactory == null)
      {
        solrConfig.DocumentElement.AppendChild(newSchemaFactory);
      }
      else
      {
        solrConfig.DocumentElement.ReplaceChild(newSchemaFactory, oldSchemaFactory);
      }
    }

    private void RemoveAddSchemaFieldsProcessor(XmlDocumentEx solrConfig)
    {
      var element =
        solrConfig.SelectSingleNode(
          "/config/updateRequestProcessorChain/processor[@class='solr.AddSchemaFieldsUpdateProcessorFactory']");
      if (element != null)
      {
        element.ParentNode.RemoveChild(element);
      }
    }

    private static XmlNodeList GetSolrIndexNodesFrom(XmlDocument sitecoreConfig)
    {
      return sitecoreConfig.SelectNodes(
        "/sitecore/contentSearch/configuration/indexes/index[@type='Sitecore.ContentSearch.SolrProvider.SolrSearchIndex, Sitecore.ContentSearch.SolrProvider']");
    }

    private static string GetUrlFrom(XmlDocument sitecoreConfig)
    {
      XmlNode serviceBaseAddressNode = sitecoreConfig.SelectSingleNode("/sitecore/settings/setting[@name='ContentSearch.Solr.ServiceBaseAddress']");
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
        throw new FileNotFoundException($"The Schema.xml file was not found: Checked here {schemaPath} and here {managedSchemaPath}. This probably means that the collection1 (for Solr 4.x) or configsets directory (for Solr 5+) has been modified.  Please copy these from a fresh install of Solr of the same release version you are running.");
      }

      string inputPath = managedSchemaExists ? managedSchemaPath : schemaPath;
      string outputPath = schemaPath;
      InvokeSitecoreGenerateSchemaUtility(contentSearchDllPath, inputPath, outputPath);
    }

    private static string GetCoreName(XmlElement node)
    {
      var coreElement = node.SelectSingleNode("param[@desc='core']") as XmlElement;
      string id = node.Attributes["id"].InnerText;
      coreElement = Assert.IsNotNull(coreElement, "param[@desc='core'] not found in Solr configuration file");
      string coreName = coreElement.InnerText.Replace("$(id)", id);
      return coreName;
    }

    private void CreateSolrCore(string url, string coreName, string instanceDir)
    {
      RequestAndGetResponseStream($"{url}/admin/cores?action=CREATE&name={coreName}&instanceDir={instanceDir}&config=solrconfig.xml&schema=schema.xml&dataDir=data");
    }

    private void RemoveCorePropertiesFile(string newCorePath)
    {
      string path = string.Format(newCorePath.EnsureEnd(@"\") + "core.properties");
      DeleteFile(path);
    }

    private SolrInformation GetSolrInformation(string url)
    {
      string solrInfoUrl = $"{url}/admin/info/system";
      Stream response = RequestAndGetResponseStream(solrInfoUrl);
      string responseAsString = GetStringFromStream(response);
      var doc = XmlDocumentEx.LoadXml(responseAsString);
        
      return new SolrInformation(doc);
    }

    private string GetStringFromStream(Stream stream)
    {
      using (var reader = new StreamReader(stream))
      {
        return reader.ReadToEnd();
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
