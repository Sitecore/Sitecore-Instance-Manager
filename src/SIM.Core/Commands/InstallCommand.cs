namespace SIM.Core.Commands
{
  using System.IO;
  using System.Linq;
  using SIM.Adapters.SqlServer;
  using SIM.Core.Common;
  using SIM.Pipelines;
  using SIM.Pipelines.Install;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class InstallCommand : AbstractCommand<string[]>
  {
    [CanBeNull]
    public virtual string Name { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string Product { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string Version { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string Revision { get; [UsedImplicitly] set; }

    protected override void DoExecute(CommandResult<string[]> result)
    {
      Assert.ArgumentNotNull(result, "result");

      var name = this.Name;
      Assert.ArgumentNotNullOrEmpty(name, "name");

      var product = this.Product;
      var version = this.Version;
      var revision = this.Revision;

      var profile = Profile.Read();
      var repository = profile.LocalRepository;
      Ensure.IsNotNullOrEmpty(repository, "Profile.LocalRepository is not specified");
      Ensure.IsTrue(Directory.Exists(repository), "Profile.LocalRepository points to missing folder");

      var license = profile.License;
      Ensure.IsNotNullOrEmpty(license, "Profile.License is not specified");
      Ensure.IsTrue(File.Exists(license), "Profile.License points to missing file");

      var builder = profile.GetValidConnectionString();

      var instancesFolder = profile.InstancesFolder;
      Ensure.IsNotNullOrEmpty(instancesFolder, "Profile.InstancesFolder is not specified");
      Ensure.IsTrue(Directory.Exists(instancesFolder), "Profile.InstancesFolder points to missing folder");

      var rootPath = Path.Combine(instancesFolder, name);
      Ensure.IsTrue(!Directory.Exists(rootPath), "Folder already exists: {0}", rootPath);

      ProductManager.Initialize(repository);

      var distributive = ProductManager.FindProduct(ProductType.Standalone, product, version, revision);
      Ensure.IsNotNull(distributive, "product is not found");

      PipelineManager.Initialize(XmlDocumentEx.LoadXml(PipelinesConfig.Contents).DocumentElement);

      var sqlServerAccountName = SqlServerManager.Instance.GetSqlServerAccountName(builder);
      var webServerIdentity = Settings.CoreInstallWebServerIdentity.Value;
      var installArgs = new InstallArgs(name, name, distributive, rootPath, builder, sqlServerAccountName, webServerIdentity, new FileInfo(license), true, false, false, false, false, false, true, new Product[0]);
      var controller = new AggregatePipelineController();
      PipelineManager.StartPipeline("install", installArgs, controller, false);

      result.Success = !string.IsNullOrEmpty(controller.Message);
      result.Message = controller.Message;
      result.Data = controller.GetMessages().ToArray();
    }
  }
}