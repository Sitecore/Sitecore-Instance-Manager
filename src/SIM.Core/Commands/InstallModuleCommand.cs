namespace SIM.Core.Commands
{
  using System.IO;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.IO;
  using SIM.Pipelines;
  using SIM.Pipelines.InstallModules;
  using SIM.Products;

  public class InstallModuleCommand : AbstractCommand<string[]>
  {
    [CanBeNull]
    public virtual string Name { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string Module { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string Version { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string Revision { get; [UsedImplicitly] set; }

    protected override void DoExecute(CommandResult<string[]> result)
    {
      Assert.ArgumentNotNull(result, nameof(result));

      var name = Name;
      Assert.ArgumentNotNullOrEmpty(name, nameof(name));

      var product = Module;
      var version = Version;
      var revision = Revision;

      var profile = Profile.Read(FileSystem);
      var repository = profile.LocalRepository;
      Ensure.IsNotNullOrEmpty(repository, "Profile.LocalRepository is not specified");
      Ensure.IsTrue(Directory.Exists(repository), "Profile.LocalRepository points to missing location");

      var license = profile.License;
      Ensure.IsNotNullOrEmpty(license, "Profile.License is not specified");
      Ensure.IsTrue(File.Exists(license), "Profile.License points to missing file");

      var connectionString = profile.GetValidConnectionString();

      var instance = InstanceManager.Default.GetInstance(name);
      Ensure.IsNotNull(instance, "The {0} instance is not found", name);

      ProductManager.Initialize(repository);

      var distributive = ProductManager.FindProduct(ProductType.Module, product, version, revision);
      Ensure.IsNotNull(distributive, "product is not found");

      PipelineManager.Initialize(XmlDocumentEx.LoadXml(PipelinesConfig.Contents).DocumentElement);
      
      var args = new InstallModulesArgs(instance, new[] { distributive }, connectionString);
      var controller = new AggregatePipelineController();
      PipelineManager.StartPipeline("installmodules", args, controller, false);

      result.Success = !string.IsNullOrEmpty(controller.Message);
      result.Message = controller.Message;
      result.Data = controller.GetMessages().ToArray();
    }

    public InstallModuleCommand([NotNull] IFileSystem fileSystem) : base(fileSystem)
    {
    }
  }
}