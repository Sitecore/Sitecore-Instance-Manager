using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SIM.Instances;
using SIM.Pipelines.Install.Modules;
using SIM.Products;

namespace SIM.Tests.Pipelines
{
  using SIM.Extensions;

  [TestClass]
  public class SwitchConfigsToSolrTests
  {

    #region Constants

    private const string InstanceName = "INSTANCE";
    private const string SomeWebRootPath = @"c:\some\path\website";

    #endregion

    #region Fields

    private SwitchConfigsToSolr _Sut;
    private Instance _Instance;
    private Product _Product;

    #endregion

    #region Setup and helper methods

    [TestInitialize]
    public void Setup()
    {
      _Instance = Substitute.For<Instance>();
      _Instance.Name.Returns(InstanceName);
      _Instance.WebRootPath.Returns(SomeWebRootPath);

      _Product = Substitute.For<Product>();

      _Sut = Substitute.For<SwitchConfigsToSolr>();
    }

    private void Act()
    {
      _Sut.Execute(_Instance, _Product);
    }

    private void ArrangeConfigFiles(IEnumerable<string> enumerable)
    {
       
      _Sut.GetFiles(null, null, SearchOption.AllDirectories).ReturnsForAnyArgs(enumerable.ToArray());
    }

   

    #endregion

    #region Tests

    [TestMethod]
    public void ShouldLoadConfiguration()
    {
      string expectedPath = SomeWebRootPath.EnsureEnd(@"\") + @"App_Config\Include";
      string expectedFilter = "*";
      SearchOption expectedOption = SearchOption.AllDirectories;

      Act();

      _Sut.Received().GetFiles(expectedPath, expectedFilter, expectedOption);
    }

    [TestMethod]
    public void ShouldEnableSolrConfigFilesEndingWithExample()
    {
      string[] returnThis = {SomeWebRootPath + @"\App_Config\Some.Solr.File.config.example"};
      ArrangeConfigFiles(returnThis);

      Act();

      _Sut.Received().RenameFile(
        SomeWebRootPath + @"\App_Config\Some.Solr.File.config.example", 
        SomeWebRootPath + @"\App_Config\Some.Solr.File.config");
    }

    [TestMethod]
    public void ShouldNotEnableSolrConfigFilesContainingIoc()
    {
      string[] returnThis = {SomeWebRootPath + @"\App_Config\Some.Solr.File.IOC.config.example"};
      ArrangeConfigFiles(returnThis);

      Act();

      _Sut.DidNotReceive().RenameFile(
        SomeWebRootPath + @"\App_Config\Some.Solr.File.IOC.config.example",
        SomeWebRootPath + @"\App_Config\Some.Solr.File.IOC.config");
    }
 

    [TestMethod]
    public void ShouldOnlyRemoveFinalWord()
    {
      string[] returnThis = {SomeWebRootPath + @"\App_Config\Some.Solr.File.example.config.example"};
      ArrangeConfigFiles(returnThis);

      Act();

      _Sut.Received().RenameFile(
        SomeWebRootPath + @"\App_Config\Some.Solr.File.example.config.example", 
        SomeWebRootPath + @"\App_Config\Some.Solr.File.example.config");
    }

    [TestMethod]
    public void ShouldEnableSolrConfigFilesEndingWithDisabled()
    {
      string[] returnThis = {SomeWebRootPath + @"\App_Config\Some.Solr.File.config.disabled"};
      ArrangeConfigFiles(returnThis);

      Act();

      _Sut.Received().RenameFile(
        SomeWebRootPath + @"\App_Config\Some.Solr.File.config.disabled", 
        SomeWebRootPath + @"\App_Config\Some.Solr.File.config");
    }

    [TestMethod]
    public void ShouldNotDisableDefaultLuceneFiles()
    {
      string[] returnThis =
      {
        SomeWebRootPath + @"\App_Config\Some.Default.Lucene.config",
        SomeWebRootPath + @"\App_Config\Some.Default.Solr.config",
      };
      ArrangeConfigFiles(returnThis);

      Act();

      _Sut.DidNotReceiveWithAnyArgs().RenameFile("", "");
    }


    [TestMethod]
    public void ShouldDisableLuceneFilesIfSolrEquivalent()
    {
      string[] returnThis = {
        SomeWebRootPath + @"\App_Config\Some.Solr.File.config",
        SomeWebRootPath + @"\App_Config\Some.Lucene.File.config"
      };
      ArrangeConfigFiles(returnThis);

      Act();

      _Sut.Received().RenameFile(
        SomeWebRootPath + @"\App_Config\Some.Lucene.File.config",
        SomeWebRootPath + @"\App_Config\Some.Lucene.File.config.disabled");
    }

    /// <summary>
    /// Ensure that Sitecore.Social.Lucene.Index.Analytics.Facebook.config is disabled.
    /// There is no Solr equivalent.
    /// </summary>
    [TestMethod]
    public void ShouldDisableFacebookFile()
    {
      string[] returnThis = {
        SomeWebRootPath + @"\App_Config\Sitecore.Social.Lucene.Index.Analytics.Facebook.config"
      };
      ArrangeConfigFiles(returnThis);

      Act();

      _Sut.Received().RenameFile(
        SomeWebRootPath + @"\App_Config\Sitecore.Social.Lucene.Index.Analytics.Facebook.config",
        SomeWebRootPath + @"\App_Config\Sitecore.Social.Lucene.Index.Analytics.Facebook.config.disabled");
    }

    [TestMethod]
    public void ShouldIgnoreLuceneFilesIfNoEquivalent()
    {
      string[] returnThis = {
        SomeWebRootPath + @"\App_Config\Some.Lucene.File.config"
      };
      ArrangeConfigFiles(returnThis);

      Act();

      _Sut.DidNotReceive().RenameFile(
        SomeWebRootPath + @"\App_Config\Some.Lucene.File.config",
        SomeWebRootPath + @"\App_Config\Some.Lucene.File.config.disabled");
    }

    [TestMethod]
    public void ShouldRenameCore()
    {
      string[] returnThis = {
        SomeWebRootPath + @"\App_Config\Some.Solr.File.config"
      };
      ArrangeConfigFiles(returnThis);
      
      _Sut.FileReadAllText(SomeWebRootPath + @"\App_Config\Some.Solr.File.config").Returns(@"<root><param desc=""core"">$(id)</param>/root>");


      Act();

      _Sut.Received().FileWriteAllText(SomeWebRootPath + @"\App_Config\Some.Solr.File.config", @"<root><param desc=""core"">INSTANCE_$(id)</param>/root>");
    }

  #endregion

  }
}
