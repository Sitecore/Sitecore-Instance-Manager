namespace SIM.Core.Commands
{
  using System;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base.Exceptions;
  using SIM.Adapters.SqlServer;
  using SIM.Core.Common;
  using SIM.Extensions;
  using SIM.IO;
  using SIM.Pipelines;
  using SIM.Pipelines.Install;
  using SIM.Products;

  public class InstallCommand : AbstractCommand<string[]>
  {
    [CanBeNull]
    private IProfile _Profile;

    [CanBeNull]
    public virtual string Name { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string DistributionPackagePath { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string Product { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string Version { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string Revision { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string ConnectionString { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string SqlPrefix { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual bool AttachDatabases { get; [UsedImplicitly] set; } = AttachDatabasesDefault;

    public const bool AttachDatabasesDefault = true;

    [CanBeNull]
    public virtual bool SkipUnnecessaryFiles { get; [UsedImplicitly] set; } = SkipUnnecessaryFilesDefault;

    protected IProfile Profile => _Profile ?? (_Profile = Common.Profile.Read(FileSystem));

    public const bool SkipUnnecessaryFilesDefault = false;

    protected override void DoExecute(CommandResult<string[]> result)
    {
      Assert.ArgumentNotNull(result, nameof(result));

      var name = Name;
      Assert.ArgumentNotNullOrEmpty(name, nameof(name));

      var hostNames = new[] {name};
      var sqlPrefix = SqlPrefix ?? name;    
      var attachDatabases = AttachDatabases;
      var skipUnnecessaryFiles = SkipUnnecessaryFiles;

      var repository = Profile.LocalRepository;
      Ensure.IsNotNullOrEmpty(repository, "Profile.LocalRepository is not specified");
      Ensure.IsTrue(FileSystem.ParseFolder(repository).Exists, "Profile.LocalRepository points to missing folder");

      var license = Profile.License;
      Ensure.IsNotNullOrEmpty(license, "Profile.License is not specified");
      Ensure.IsTrue(FileSystem.ParseFile(license).Exists, "Profile.License points to missing file");

      var builder = Profile.GetValidConnectionString();

      var instancesFolder = FileSystem.ParseFolder(Profile.InstancesFolder.IsNotNullOrEmpty(nameof(Profile.InstancesFolder)));
      Ensure.IsTrue(instancesFolder.Exists, $"{nameof(Profile.InstancesFolder)} points to missing folder");

      var root = instancesFolder.GetChildFolder(name);
      Ensure.IsTrue(!root.Exists, $"Folder already exists: {root.FullName}");

      ProductManager.Initialize(repository);
      
      var distributive = GetDistributive();

      PipelineManager.Initialize(XmlDocumentEx.LoadXml(PipelinesConfig.Contents).DocumentElement);

      var sqlServerAccountName = SqlServerManager.Instance.GetSqlServerAccountName(builder);
      var webServerIdentity = Settings.CoreInstallWebServerIdentity.Value;
      var installArgs = new InstallArgs(name, hostNames, sqlPrefix, attachDatabases, distributive, root, builder, sqlServerAccountName, webServerIdentity, FileSystem.ParseFile(license), true, false, false, !skipUnnecessaryFiles, !skipUnnecessaryFiles, false, false, false, "", new Product[0]);
      var controller = new AggregatePipelineController();
      PipelineManager.StartPipeline("install", installArgs, controller, false);

      result.Success = !string.IsNullOrEmpty(controller.Message);
      result.Message = controller.Message;
      result.Data = controller.GetMessages().ToArray();
    }

    [NotNull]
    private Product GetDistributive()
    {
      var packagePath = DistributionPackagePath;
      if (!string.IsNullOrEmpty(packagePath))
      {
        var file = FileSystem.ParseFile(packagePath);
        Assert.ArgumentCondition(file.Exists, nameof(DistributionPackagePath), "The file does not exist");

        var distributive = Products.Product.Parse(file.FullName);
        Ensure.IsNotNull(distributive, "product is not found");

        return distributive;
      }
      else
      {
        var product = Product;
        var version = Version;
        var revision = Revision;

        var distributive = ProductManager.FindProduct(ProductType.Standalone, product, version, revision);
        Ensure.IsNotNull(distributive, "product is not found");

        return distributive;
      }
    }

    public InstallCommand([NotNull] IFileSystem fileSystem) : base(fileSystem)
    {
    }
  }
}