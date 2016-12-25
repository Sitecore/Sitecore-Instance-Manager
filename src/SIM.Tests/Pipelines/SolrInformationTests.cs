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

    [TestMethod]
    public void CanCreateFromXml()
    {
      XmlDocumentEx solr4Definition =
        XmlDocumentEx.LoadXml(@"<response><lst name=""lucene""><str name=""solr-spec-version"">4.0.0</str></lst></response>");
      SolrInformation sut = new SolrInformation(solr4Definition);
      Assert.IsNotNull(sut);
    }

    [TestMethod]
    public void CanCreateFromSolr5()
    {
      XmlDocumentEx solr5Definition =
        XmlDocumentEx.LoadXml(@"<response><lst name=""lucene""><str name=""solr-spec-version"">5.0.0</str></lst><str name=""solr_home"">anonymous</str></response>");
      SolrInformation sut = new SolrInformation(solr5Definition);
      Assert.IsNotNull(sut);
    }

    [TestMethod, ExpectedException(typeof(InvalidSolrInformationResponse))]
    public void Constructor_InvalidSolrNode_Throws()
    {
      XmlDocumentEx solr4Definition =
        XmlDocumentEx.LoadXml(@"<response><lst name=""lucene""><str name=""solr-spec-version"">abc</str></lst></response>");
      new SolrInformation(solr4Definition);

    }

    [TestMethod, ExpectedException(typeof(InvalidSolrInformationResponse))]
    public void Constructor_InvalidSolrVersion_Throws()
    {
      XmlDocumentEx solr4Definition =
        XmlDocumentEx.LoadXml(@"<response><lst name=""lucene""><str name=""solr-spec-version"">3.0.0</str></lst></response>");
      new SolrInformation(solr4Definition);

    }

    [TestMethod, ExpectedException(typeof(InvalidSolrInformationResponse))]
    public void Constructor_MissingBasePathForSolr5_Throws()
    {
      XmlDocumentEx solr4Definition =
        XmlDocumentEx.LoadXml(@"<response><lst name=""lucene""><str name=""solr-spec-version"">5.0.0</str></lst></response>");
      new SolrInformation(solr4Definition);

    }
  }
}
