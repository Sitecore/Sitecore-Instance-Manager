namespace SIM.Core.Commands
{
  using System;
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

    protected override void DoExecute(CommandResultBase<string[]> result)
    {
      Assert.ArgumentNotNull(result, "result");

      var name = this.Name;
      Assert.ArgumentNotNullOrEmpty(name, "name");

      var product = this.Product;
      var version = this.Version;
      var revision = this.Revision;

      var profile = Profile.Read();
      var repository = profile.LocalRepository;
      Assert.IsNotNullOrEmpty(repository, "Profile.LocalRepository is null or empty");
      Assert.IsTrue(Directory.Exists(repository), "Profile.LocalRepository points to non-existing folder");

      var license = profile.License;
      Assert.IsNotNullOrEmpty(license, "Profile.License is null or empty");
      Assert.IsTrue(File.Exists(license), "Profile.License points to non-existing file");

      var builder = profile.GetValidConnectionString();

      var instancesFolder = profile.InstancesFolder;
      Assert.IsNotNullOrEmpty(instancesFolder, "Profile.InstancesFolder is null or empty");
      Assert.IsTrue(Directory.Exists(instancesFolder), "Profile.InstancesFolder points to non-existing folder");

      var rootPath = Path.Combine(instancesFolder, name);
      Assert.IsTrue(!Directory.Exists(rootPath), "The folder already exists: {0}", rootPath);

      ProductManager.Initialize(repository);

      var products = ProductManager.StandaloneProducts.Where(x => true);

      if (!string.IsNullOrEmpty(product))
      {
        products = products.Where(x => x.Name.Equals(product, StringComparison.OrdinalIgnoreCase));
      }

      if (!string.IsNullOrEmpty(version))
      {
        products = products.Where(x => x.Version == version);
      }
      else
      {
        products = products.OrderByDescending(x => x.Version);
      }

      if (!string.IsNullOrEmpty(revision))
      {
        products = products.Where(x => x.Revision == revision);
      }
      else
      {
        products = products.OrderByDescending(x => x.Revision);
      }

      var distributive = products.FirstOrDefault();
      if (distributive == null)
      {
        result.Success = false;
        result.Message = "product not found";
        result.Data = null;

        return;
      }

      PipelineManager.Initialize();

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