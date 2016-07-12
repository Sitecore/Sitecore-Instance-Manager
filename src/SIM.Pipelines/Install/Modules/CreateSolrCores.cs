using System;
using System.CodeDom;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;
using Sitecore.Diagnostics.Base;
using SIM.Instances;
using SIM.Pipelines.InstallModules;
using SIM.Products;

namespace SIM.Pipelines.Install.Modules
{
  /// <summary>
  /// Creates cores for all configured Solr indexes
  /// </summary>
  public class CreateSolrCores : IPackageInstallActions
  {

    #region Constants

    private const string GenerateSchemaClass = "Sitecore.ContentSearch.ProviderSupport.Solr.SchemaGenerator";
    private const string GenerateSchemaMethod = "GenerateSchema";

    #endregion

    #region Public methods

    public void Execute(Instance instance, Product module)
    {
      XmlDocument config = instance.GetShowconfig();
      string solrUrl = GetUrl(config);
      XmlNodeList solrIndexes = GetSolrIndexNodes(config);
      string defaultCollectionPath = GetDefaultCollectionPath(solrUrl);

      foreach (XmlElement node in solrIndexes)
      {
        string coreName = GetCoreName(node);
        string corePath = defaultCollectionPath.Replace("collection1", coreName);
        this.CopyDirectory(defaultCollectionPath, corePath);
        DeleteCopiedCorePropertiesFile(corePath);
        UpdateSchema(instance.WebRootPath, corePath);
        CallSolrCreateCoreAPI(solrUrl, coreName, corePath);
      }
    }

    #endregion

    #region Private methods

    private static XmlNodeList GetSolrIndexNodes(XmlDocument config)
    {
      return config.SelectNodes(
        "/sitecore/contentSearch/configuration/indexes/index[@type='Sitecore.ContentSearch.SolrProvider.SolrSearchIndex, Sitecore.ContentSearch.SolrProvider']");
    }

    private static string GetUrl(XmlDocument config)
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
        throw new FileNotFoundException(string.Format("Schema file not found: Checked here {0} and here {1}.", schemaPath, managedSchemaPath));
      }

      string inputPath = managedSchemaExists ? managedSchemaPath : schemaPath;
      string outputPath = schemaPath;
      this.GenerateSchema(contentSearchDllPath, inputPath, outputPath);
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
      this.RequestAndGetResponse(string.Format(
        "{0}/admin/cores?action=CREATE&name={1}&instanceDir={2}&config=solrconfig.xml&schema=schema.xml&dataDir=data", url, coreName, instanceDir));
    }

    private void DeleteCopiedCorePropertiesFile(string newCorePath)
    {
      string path = string.Format(newCorePath.EnsureEnd(@"\") + "core.properties");
      this.DeleteFile(path);
    }

    private string GetDefaultCollectionPath(string url)
    {
      var response = this.RequestAndGetResponse(string.Format(
        "{0}/admin/cores", url));

      var doc = new XmlDocument();
      doc.Load(response.GetResponseStream());

      XmlNode collection1Node = doc.SelectSingleNode("/response/lst[@name='status']/lst[@name='collection1']");
      if (collection1Node == null) throw new ApplicationException("collection1 not found");

      return collection1Node.SelectSingleNode("str[@name='instanceDir']").InnerText;
    }

    #endregion

    #region Virtual methods
    // All system calls are wrapped in virtual methods for unit testing.

    public virtual HttpWebResponse RequestAndGetResponse(string url)
    {
      return WebRequestHelper.RequestAndGetResponse(url);
    }

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

    /// <summary>
    /// Dynamically loads GenerateSchema class from target site, and uses it
    /// to applied required changes to the Solr schema.xml file.
    /// </summary>
    public virtual void GenerateSchema(string dllPath, string inputPath, string outputPath)
    {
      Assembly assembly = ReflectionUtil.GetAssembly(dllPath);
      Type generateSchema = ReflectionUtil.GetType(assembly, GenerateSchemaClass);
      object obj = ReflectionUtil.CreateObject(generateSchema);
      ReflectionUtil.InvokeMethod(obj, GenerateSchemaMethod, inputPath, outputPath);
    }

    #endregion
  }
}
