namespace SIM.Core.Commands
{
  using System.IO;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core.Common;
  using SIM.Instances;
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
      Assert.ArgumentNotNull(result, "result");

      var name = this.Name;
      Assert.ArgumentNotNullOrEmpty(name, "name");

      var product = this.Module;
      var version = this.Version;
      var revision = this.Revision;

      var profile = Profile.Read();
      var repository = profile.LocalRepository;
      Ensure.IsNotNullOrEmpty(repository, "Profile.LocalRepository is not specified");
      Ensure.IsTrue(Directory.Exists(repository), "Profile.LocalRepository points to missing location");

      var license = profile.License;
      Ensure.IsNotNullOrEmpty(license, "Profile.License is not specified");
      Ensure.IsTrue(File.Exists(license), "Profile.License points to missing file");

      var builder = profile.GetValidConnectionString();

      var instance = InstanceManager.GetInstance(name);
      Ensure.IsNotNull(instance, "instance is not found", name);

      ProductManager.Initialize(repository);

      var distributive = ProductManager.FindProduct(ProductType.Module, product, version, revision);
      Ensure.IsNotNull(distributive, "product is not found");

      PipelineManager.Initialize(XmlDocumentEx.LoadXml(PipelinesConfig.Contents).DocumentElement);
      
      var installArgs = new InstallModulesArgs(instance, new[] { distributive }, builder);
      var controller = new AggregatePipelineController();
      PipelineManager.StartPipeline("install", installArgs, controller, false);

      result.Success = !string.IsNullOrEmpty(controller.Message);
      result.Message = controller.Message;
      result.Data = controller.GetMessages().ToArray();
    }
  }
}