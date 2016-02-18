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

    protected override void DoExecute(CommandResultBase<string[]> result)
    {
      Assert.ArgumentNotNull(result, "result");

      var name = this.Name;
      Assert.ArgumentNotNullOrEmpty(name, "name");

      var product = this.Module;
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

      var instance = InstanceManager.GetInstance(name);
      Assert.IsNotNull(instance, "InstanceManager.GetInstance({0}) is null", name);

      ProductManager.Initialize(repository);

      var distributive = ProductManager.FindProduct(ProductType.Module, product, version, revision);
      if (distributive == null)
      {
        result.Success = false;
        result.Message = "product not found";
        result.Data = null;

        return;
      }

      PipelineManager.Initialize();
      
      var installArgs = new InstallModulesArgs(instance, new[] { distributive }, builder);
      var controller = new AggregatePipelineController();
      PipelineManager.StartPipeline("install", installArgs, controller, false);

      result.Success = !string.IsNullOrEmpty(controller.Message);
      result.Message = controller.Message;
      result.Data = controller.GetMessages().ToArray();
    }
  }
}