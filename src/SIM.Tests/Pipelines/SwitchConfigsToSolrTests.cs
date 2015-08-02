using System;
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
  public class SwitchConfigsToSolrTests
  {

    private SwitchConfigsToSolr _sut;
    private Instance _instance;
    private Product _product;
    private string SomeWebRootPath = @"c:\some\path\website";
    private string InstanceName = "INSTANCE";

    [TestInitialize]
    public void Setup()
    {
      _instance = Substitute.For<Instance>();
      _instance.Name.Returns(InstanceName);
      _instance.WebRootPath.Returns(SomeWebRootPath);

      _product = Substitute.For<Product>();

      _sut = Substitute.For<SwitchConfigsToSolr>();
    }

    private void Act()
    {
      _sut.Execute(_instance, _product);
    }

    [TestMethod]
    public void ShouldEnableSolrConfigFilesEndingWith_example()
    {
      _sut.GetConfigFiles().Returns(new string[] {SomeWebRootPath + @"\App_Config\Some.Solr.File.config.example"});

      Act();

      _sut.Received().RenameFile(
        SomeWebRootPath + @"\App_Config\Some.Solr.File.config.example", 
        SomeWebRootPath + @"\App_Config\Some.Solr.File.config");
    }

    [TestMethod]
    public void ShouldOnlyRemoveFinalWord()
    {
      _sut.GetConfigFiles().Returns(new string[] {SomeWebRootPath + @"\App_Config\Some.Solr.File.example.config.example"});

      Act();

      _sut.Received().RenameFile(
        SomeWebRootPath + @"\App_Config\Some.Solr.File.example.config.example", 
        SomeWebRootPath + @"\App_Config\Some.Solr.File.example.config");
    }

    [TestMethod]
    public void ShouldEnableSolrConfigFilesEndingWith_disabled()
    {
      _sut.GetConfigFiles().Returns(new string[] {SomeWebRootPath + @"\App_Config\Some.Solr.File.config.disabled"});

      Act();

      _sut.Received().RenameFile(
        SomeWebRootPath + @"\App_Config\Some.Solr.File.config.disabled", 
        SomeWebRootPath + @"\App_Config\Some.Solr.File.config");
    }


    [TestMethod]
    public void ShouldDisableLuceneFilesIfSolrEquivalent()
    {
      _sut.GetConfigFiles().Returns(new string[]
      {
        SomeWebRootPath + @"\App_Config\Some.Solr.File.config",
        SomeWebRootPath + @"\App_Config\Some.Lucene.File.config"
      });

      Act();

      _sut.Received().RenameFile(
        SomeWebRootPath + @"\App_Config\Some.Lucene.File.config",
        SomeWebRootPath + @"\App_Config\Some.Lucene.File.config.disabled");
    }

    [TestMethod]
    public void ShouldIgnoreLuceneFilesIfNoEquivalent()
    {
      _sut.GetConfigFiles().Returns(new string[]
      {
        SomeWebRootPath + @"\App_Config\Some.Lucene.File.config"
      });

      Act();

      _sut.DidNotReceive().RenameFile(
        SomeWebRootPath + @"\App_Config\Some.Lucene.File.config",
        SomeWebRootPath + @"\App_Config\Some.Lucene.File.config.disabled");
    }

    [TestMethod]
    public void ShouldRenameCore()
    {
      var solrConfigPath = SomeWebRootPath + @"\App_Config\Some.Solr.File.config";
      _sut.GetConfigFiles().Returns(new string[]
      {
        solrConfigPath
      });
      
      _sut.FileReadAllText(solrConfigPath).Returns(@"<root><param desc=""core"">$(id)</param>/root>");


      Act();

      _sut.Received().FileWriteAllText(solrConfigPath, @"<root><param desc=""core"">INSTANCE_$(id)</param>/root>");
    }
  }
}
