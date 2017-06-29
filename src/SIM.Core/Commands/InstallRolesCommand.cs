namespace SIM.Core.Commands
{
  using System.Net;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.IO;
  using SIM.IO.Real;
  using SIM.Pipelines.Install;

  public class InstallRolesCommand : AbstractInstanceActionCommand
  {
    [CanBeNull]
    public virtual string Roles { get; [UsedImplicitly] set; }
    
    public InstallRolesCommand([NotNull] IFileSystem fileSystem) 
    : base(fileSystem)
    {
    }

    protected override void DoExecute(Instance instance, CommandResult result)
    {
      Assert.ArgumentNotNullOrEmpty(Roles);

      var product = instance.Product;
      var version = $"{product.Version}.{product.Update}";

      InstallRolesCommandHelper.Install(FileSystem.ParseFolder(instance.WebRootPath), version, Roles);

      result.Success = true;
      result.Message = $"Installed and configured as {Roles}";      
    }
  }
}