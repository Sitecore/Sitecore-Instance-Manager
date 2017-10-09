using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIM.Pipelines.Install.Modules;

namespace SIM.Tests.Pipelines
{
  [TestClass]
  class SolrInformationTests
  {
    #region Constants

    const string Solr4Xml = @"<response><lst name=""lucene""><str name=""solr-spec-version"">4.0.0</str></lst><str name=""solr_home"">anonymousPath</str></response>";
    const string Solr5Xml = @"<response><lst name=""lucene""><str name=""solr-spec-version"">5.0.0</str></lst><str name=""solr_home"">anonymousPath</str></response>";

    XmlDocumentEx Solr4Definition { get; } = XmlDocumentEx.LoadXml(Solr4Xml);
    XmlDocumentEx Solr5Definition { get; } = XmlDocumentEx.LoadXml(Solr5Xml);

    #endregion


    [TestMethod]
    public void ConstructorPassedSolr4ReturnsObject()
    {
      SolrInformation sut = new SolrInformation(Solr4Definition);
      Assert.IsNotNull(sut);
    }

    [TestMethod]
    public void ConstructorPassedSolr5ReturnsObject()
    {
      SolrInformation sut = new SolrInformation(Solr5Definition);
      Assert.IsNotNull(sut);
    }

    [TestMethod, ExpectedException(typeof(SolrInformation.InvalidException))]
    public void ConstructorInvalidSolrNodeThrows()
    {
      XmlDocumentEx definitionWithInvalidVersion =
        XmlDocumentEx.LoadXml(@"<response><lst name=""lucene""><str name=""solr-spec-version"">invalidSolrVersion</str></lst></response>");
      new SolrInformation(definitionWithInvalidVersion);

    }

    [TestMethod, ExpectedException(typeof(SolrInformation.InvalidException))]
    public void ConstructorInvalidSolrVersionThrows()
    {
      XmlDocumentEx solr4Definition =
        XmlDocumentEx.LoadXml(@"<response><lst name=""lucene""><str name=""solr-spec-version"">3.0.0</str></lst></response>");
      new SolrInformation(solr4Definition);

    }

    
    [TestMethod, ExpectedException(typeof(SolrInformation.InvalidException))]
    public void ConstructorSolrPathMissingThrows()
    {
      XmlDocumentEx missingPath =
        XmlDocumentEx.LoadXml(@"<response><lst name=""lucene""><str name=""solr-spec-version"">4.0.0</str></lst></response>");
      new SolrInformation(missingPath);

    }

    /// <summary>
    /// Documents behavior of LoadXml method when faced with invalid XML.
    /// </summary>
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void ConstrutorPassedInvalidXmlThrowsArgumentNull()
    {
      XmlDocumentEx solrDefinition = XmlDocumentEx.LoadXml(@"invalid xml");
      new SolrInformation(solrDefinition);
    }

    [TestMethod]
    public void ConstructorPathInSolrHomeSetsProperty()
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
    public void ConstructorPathInJavaRuntimeSettingSetsProperty()
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
    public void ConstructorPathInSettingArgsUsesSetting()
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
    public void CollectionTemplateSolr4SetToCollection1()
    {
      SolrInformation sut = new SolrInformation(Solr4Definition);
      
      Assert.AreEqual("collection1", sut.CollectionTemplate);
      Assert.AreEqual(@"anonymousPath\collection1", sut.TemplateFullPath);
    }

    [TestMethod]
    public void CollectionTemplateSolr5SetToConfigSetDataDrivenConfig()
    {
      SolrInformation sut = new SolrInformation(Solr5Definition);

      Assert.AreEqual(@"configsets\data_driven_schema_configs", sut.CollectionTemplate);
      Assert.AreEqual(@"anonymousPath\configsets\data_driven_schema_configs", sut.TemplateFullPath);
    }

  }
}
