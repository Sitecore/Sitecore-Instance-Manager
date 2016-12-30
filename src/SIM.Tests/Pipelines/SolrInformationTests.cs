using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIM.Pipelines.Install.Modules;

namespace SIM.Tests.Pipelines
{
  [TestClass]
  class SolrInformationTests
  {
    #region Constants

    const string solr4xml = @"<response><lst name=""lucene""><str name=""solr-spec-version"">4.0.0</str></lst><str name=""solr_home"">anonymousPath</str></response>";
    const string solr5xml = @"<response><lst name=""lucene""><str name=""solr-spec-version"">5.0.0</str></lst><str name=""solr_home"">anonymousPath</str></response>";

    readonly XmlDocumentEx solr4definition = XmlDocumentEx.LoadXml(solr4xml);
    readonly XmlDocumentEx solr5definition = XmlDocumentEx.LoadXml(solr5xml);

    #endregion


    [TestMethod]
    public void Constructor_PassedSolr4_ReturnsObject()
    {
      SolrInformation sut = new SolrInformation(solr4definition);
      Assert.IsNotNull(sut);
    }

    [TestMethod]
    public void Constructor_PassedSolr5_ReturnsObject()
    {
      SolrInformation sut = new SolrInformation(solr5definition);
      Assert.IsNotNull(sut);
    }

    [TestMethod, ExpectedException(typeof(SolrInformation.InvalidException))]
    public void Constructor_InvalidSolrNode_Throws()
    {
      XmlDocumentEx definitionWithInvalidVersion =
        XmlDocumentEx.LoadXml(@"<response><lst name=""lucene""><str name=""solr-spec-version"">invalidSolrVersion</str></lst></response>");
      new SolrInformation(definitionWithInvalidVersion);

    }

    [TestMethod, ExpectedException(typeof(SolrInformation.InvalidException))]
    public void Constructor_InvalidSolrVersion_Throws()
    {
      XmlDocumentEx solr4Definition =
        XmlDocumentEx.LoadXml(@"<response><lst name=""lucene""><str name=""solr-spec-version"">3.0.0</str></lst></response>");
      new SolrInformation(solr4Definition);

    }

    
    [TestMethod, ExpectedException(typeof(SolrInformation.InvalidException))]
    public void Constructor_SolrPathMissing_Throws()
    {
      XmlDocumentEx missingPath =
        XmlDocumentEx.LoadXml(@"<response><lst name=""lucene""><str name=""solr-spec-version"">4.0.0</str></lst></response>");
      new SolrInformation(missingPath);

    }

    /// <summary>
    /// Documents behavior of LoadXml method when faced with invalid XML.
    /// </summary>
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Construtor_PassedInvalidXML_ThrowsArgumentNull()
    {
      XmlDocumentEx solrDefinition = XmlDocumentEx.LoadXml(@"invalid xml");
      new SolrInformation(solrDefinition);
    }

    [TestMethod]
    public void Constructor_PathInSolrHome_SetsProperty()
    {
      string anonymousPath = "anonymousPath" + Guid.NewGuid();
      XmlDocumentEx solrDefinition = XmlDocumentEx.LoadXml($@"<response><lst name=""lucene""><str name=""solr-spec-version"">5.0.0</str></lst><str name=""solr_home"">{anonymousPath}</str></response>");

      var info = new SolrInformation(solrDefinition);

      Assert.AreEqual(info.SolrBasePath, anonymousPath);
    }

    /// <summary>
    /// Fallback for Solr 4, which does not set solr_home.
    /// </summary>
    [TestMethod]
    public void Constructor_PathInJavaRuntimeSetting_SetsProperty()
    {
      string anonymousPath = "anonymousPath" + Guid.NewGuid();
      XmlDocumentEx solrDefinition = XmlDocumentEx.LoadXml($@"
    <response>
      <lst name=""lucene""><str name=""solr-spec-version"">4.0.0</str></lst>
      <lst name=""jvm"">
        <lst name=""jmx"">
          <arr name=""commandLineArgs"">
            <str>other setting</str>
            <str>-Dsolr.solr.home={anonymousPath}</str>
            <str>other setting</str>
          </arr>
      </lst>
    </lst>
</response>");

      var info = new SolrInformation(solrDefinition);

      Assert.AreEqual(info.SolrBasePath, anonymousPath);


    }


    /// <summary>
    /// Shows that settings has precedences.
    /// In practice, these should always be the same.
    /// </summary>
    [TestMethod]
    public void Constructor_PathInSettingArgs_UsesSetting()
    {
      // Randomized to ensure separate, with embedded helper value. This 
      // pattern is borrowed from AutoFixture.
      string pathFromSettings = "pathFromSettings" + Guid.NewGuid();
      string pathFromArgs = "pathFromArgs" + Guid.NewGuid();
      XmlDocumentEx solrDefinition = XmlDocumentEx.LoadXml($@"
    <response>
      <lst name=""lucene""><str name=""solr-spec-version"">4.0.0</str></lst>
        <lst name=""jvm""><lst name=""jmx"">
          <arr name=""commandLineArgs"">
            <str>-Dsolr.solr.home={pathFromArgs}</str>
          </arr>
      </lst>
    </lst>
<str name=""solr_home"">{pathFromSettings}</str>
</response>");

      var info = new SolrInformation(solrDefinition);

      Assert.AreEqual( pathFromSettings, info.SolrBasePath);


    }

    [TestMethod]
    public void CollectionTemplate_Solr4_SetToCollection1()
    {
      SolrInformation sut = new SolrInformation(solr4definition);
      
      Assert.AreEqual("collection1", sut.CollectionTemplate);
      Assert.AreEqual(@"anonymousPath\collection1", sut.TemplateFullPath);
    }

    [TestMethod]
    public void CollectionTemplate_solr5_SetToConfigSetDataDrivenConfig()
    {
      SolrInformation sut = new SolrInformation(solr5definition);

      Assert.AreEqual(@"configsets\data_driven_schema_configs", sut.CollectionTemplate);
      Assert.AreEqual(@"anonymousPath\configsets\data_driven_schema_configs", sut.TemplateFullPath);
    }

  }
}
