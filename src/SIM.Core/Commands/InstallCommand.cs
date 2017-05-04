namespace SIM.Core.Commands
{
  using System.IO;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Adapters.SqlServer;
  using SIM.Core.Common;
  using SIM.IO;
  using SIM.Pipelines;
  using SIM.Pipelines.Install;
  using SIM.Products;

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

    [CanBeNull]
    public virtual string SqlPrefix { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual bool? AttachDatabases { get; [UsedImplicitly] set; }

    public const bool AttachDatabasesDefault = true;

    [CanBeNull]
    public virtual bool? SkipUnnecessaryFiles { get; [UsedImplicitly] set; }

    public const bool SkipUnnecessaryFilesDefault = false;

    protected override void DoExecute(CommandResult<string[]> result)
    {
      Assert.ArgumentNotNull(result, nameof(result));

      var name = Name;
      Assert.ArgumentNotNullOrEmpty(name, nameof(name));

      var hostNames = new[] {name};
      var sqlPrefix = SqlPrefix ?? name;
      var product = Product;
      var version = Version;
      var revision = Revision;
      var attachDatabases = AttachDatabases ?? AttachDatabasesDefault;
      var skipUnnecessaryFiles = SkipUnnecessaryFiles ?? SkipUnnecessaryFilesDefault;

      var profile = Profile.Read(FileSystem);
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
      var installArgs = new InstallArgs(name, hostNames, sqlPrefix, attachDatabases, distributive, rootPath, builder, sqlServerAccountName, webServerIdentity, new FileInfo(license), true, false, false, !skipUnnecessaryFiles, !skipUnnecessaryFiles, false, false, false, "", new Product[0]);
      var controller = new AggregatePipelineController();
      PipelineManager.StartPipeline("install", installArgs, controller, false);

      result.Success = !string.IsNullOrEmpty(controller.Message);
      result.Message = controller.Message;
      result.Data = controller.GetMessages().ToArray();
    }

    public InstallCommand([NotNull] IFileSystem fileSystem) : base(fileSystem)
    {
    }
  }
}