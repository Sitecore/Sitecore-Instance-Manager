using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SIM.Instances;
using SIM.Pipelines.Install.Modules;
using SIM.Products;

namespace SIM.Tests.Pipelines
{
  [TestClass]
  public class CreateSolrCoreTests
  {

    #region Constants

    private const string DllPath = @"c:\some\website\bin\Sitecore.ContentSearch.dll";
    private const string SchemaPath = @"c:\some\path\SOME_CORE_NAME\conf\schema.xml";
    private const string SolrConfigPath = @"c:\some\path\SOME_CORE_NAME\conf\solrconfig.xml";
    private const string ManagedSchemaPath = @"c:\some\path\SOME_CORE_NAME\conf\managed-schema";

    #endregion

    #region Fields

    private CreateSolrCores _sut;
    private Instance _instance;
    private Product _module;
    private Stream _infoResponse;
    private Stream _coreInfoResponse;

    #endregion

    #region Setup and helper methods

    [TestInitialize]
    public void SetUp()
    {
      _sut = Substitute.For<CreateSolrCores>();
      _instance = Substitute.For<Instance>();
      _instance.WebRootPath.Returns(@"c:\some\website\");
      _module = Substitute.For<Product>();
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(GetConfigXml("SOME_URL", "SOME_CORE_NAME", "SOME_ID"));
      _instance.GetShowconfig().Returns(doc);

    }

    [TestCleanup]
    public void CleanUp()
    {
      _infoResponse.Dispose();
      _coreInfoResponse.Dispose();
    }

    private void Arrange()
    {
      ArrangeGetSolrInfo();
      ArrangeGetCores(@"<lst name='collection1'><str name='instanceDir'>c:\some\path\collection1\</str></lst>");
      _sut.FileExists(SchemaPath).Returns(true);
      _sut.FileExists(ManagedSchemaPath).Returns(false);
      var xmlDocumentEx = XmlDocumentEx.LoadXml("<anonymous />");
      _sut.XmlMerge(Arg.Any<string>(), Arg.Any<string>()).Returns(xmlDocumentEx);

    }

    private void ArrangeGetSolrInfo()
    {
      string response = 
        @"<response><lst name=""lucene""><str name=""solr-spec-version"">4.0.0</str></lst></response>";
      _infoResponse = GenerateStreamFromString(response);
 
      _sut.RequestAndGetResponseStream("SOME_URL/admin/info/system").Returns(_infoResponse);
      
    }

    private static Stream GenerateStreamFromString(string s)
    {
      MemoryStream stream = new MemoryStream();
      StreamWriter writer = new StreamWriter(stream);
      writer.Write(s);
      writer.Flush();
      stream.Position = 0;
      return stream;
    }

    private void Act()
    {
      _sut.Execute(_instance, _module);
    }

    private string GetConfigXml(string someUrl, string someCoreName, string someId)
    {
      return "<sitecore>" +
             "<settings>" +
             $"<setting name='ContentSearch.Solr.ServiceBaseAddress' value='{someUrl}' />" +
             "</settings>" +
             "<contentSearch>" +
             "<configuration>" +
             "<indexes>" +
             $"<index  type='Sitecore.ContentSearch.SolrProvider.SolrSearchIndex, Sitecore.ContentSearch.SolrProvider' id='{someId}'>" +
             $"<param desc='core' id='$(id)'>{someCoreName}</param>" +
             "</index></indexes></configuration></contentSearch></sitecore>";
    }

    private void ArrangeGetCores(string coreInfo)
    {
      HttpWebResponse response = Substitute.For<HttpWebResponse>();
      string returnValue = $"<response><lst name='status' >{coreInfo}</lst></response>";
      _coreInfoResponse = GenerateStreamFromString(returnValue);

      _sut.RequestAndGetResponseStream("SOME_URL/admin/cores").Returns(_coreInfoResponse);
    }

    #endregion

    #region Tests

    [TestMethod]
    public void ShouldGetSystemInfo()
    {
      Arrange();

      Act();

      _sut.Received().RequestAndGetResponseStream("SOME_URL/admin/info/system");
    }

    [TestMethod, ExpectedException(typeof(ApplicationException))]
    public void ShouldThrowIfSolr5()
    {
      Arrange();
      _infoResponse = GenerateStreamFromString(@"<response><lst name=""lucene"">
        <str name=""solr-spec-version"">5.0.0</str></lst></response>");

      _sut.RequestAndGetResponseStream("SOME_URL/admin/info/system").Returns(_infoResponse);
        
      Act();

      
    }


    [TestMethod]
    public void ShouldGetCores()
    {
      Arrange();

      Act();

      _sut.Received().RequestAndGetResponseStream("SOME_URL/admin/cores");
    }

    [TestMethod, ExpectedException(typeof(ApplicationException))]
    public void ShouldThrowIfNoCollection1()
    {
      Arrange();
      ArrangeGetCores("");

      Act();
    }

    [TestMethod]
    public void ShouldCopyCollection1InstanceDirToNewCorePath()
    {
      Arrange();

      Act();

      _sut.Received().CopyDirectory(@"c:\some\path\collection1\", @"c:\some\path\SOME_CORE_NAME\");
    }

    [TestMethod]
    public void ShouldDeletePropertiesFile()
    {
      Arrange();

      Act();

      _sut.Received().DeleteFile(@"c:\some\path\SOME_CORE_NAME\core.properties");
    }

    [TestMethod]
    public void ShouldCallSolrCreateCore()
    {
      Arrange();

      _sut.Execute(_instance,_module);

      var dirPath = @"c:\some\path\SOME_CORE_NAME\";
      string coreName = "SOME_CORE_NAME";
      _sut.Received()
        .RequestAndGetResponseStream(
          $"SOME_URL/admin/cores?action=CREATE&name={coreName}&instanceDir={dirPath}&config=solrconfig.xml&schema=schema.xml&dataDir=data");

    }

    [TestMethod]
    public void ShouldCallGenerateAssembly()
    {
      Arrange();
 
      Act();

      _sut.Received().InvokeSitecoreGenerateSchemaUtility(DllPath, SchemaPath, SchemaPath);
    }

    [TestMethod]
    public void ShouldUseManagedSchemaFileWhenNoSchema()
    {
      Arrange();
      _sut.FileExists(SchemaPath).Returns(false);
      _sut.FileExists(ManagedSchemaPath).Returns(true);

      Act();

      _sut.Received().InvokeSitecoreGenerateSchemaUtility(DllPath, ManagedSchemaPath, SchemaPath);
    }

    [TestMethod, ExpectedException(typeof(FileNotFoundException))]
    public void ShouldThrowIfNoSchema()
    {
      Arrange();
      _sut.FileExists(SchemaPath).Returns(false);
      _sut.FileExists(ManagedSchemaPath).Returns(false);

      Act();
    }

    /// <summary>
    /// See https://github.com/dsolovay/sitecore-instance-manager/issues/38
    /// </summary>
    [TestMethod]
    public void ShouldMergeTermConfigSettings()
    {
      Arrange();

      Act();

      _sut.Received().XmlMerge(SolrConfigPath, CreateSolrCores.SolrConfigPatch);
    }


    /// <summary>
    /// See https://github.com/dsolovay/sitecore-instance-manager/issues/38
    /// </summary>
    [TestMethod] public void ShouldSaveSorlConfigAsNormalizedUtf8()
    {
      Arrange();
      const string nonIndentedXml = "<a><b/></a>";
      var xmlDocumentEx = XmlDocumentEx.LoadXml(nonIndentedXml);
      _sut.XmlMerge(Arg.Any<string>(), Arg.Any<string>()).Returns(xmlDocumentEx);

      Act();

      string xmlDocumentDeclaration_WithUtf8 = @"<?xml version=""1.0"" encoding=""UTF-8"" ?>";
      string indentedXml = "<a>\r\n  <b />\r\n</a>";

      _sut.Received().WriteAllText(
        path: SolrConfigPath,
        text: xmlDocumentDeclaration_WithUtf8 + "\r\n" + indentedXml);
    }

 
    #endregion

  }
}
