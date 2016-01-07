namespace SIM.Commands.Commands
{
  using System.Linq;
  using SIM.Commands.Common;
  using SIM.Instances;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class ListInstancesCommand : AbstractCommand
  {
    [CanBeNull]
    public virtual string Filter { get; set; }

    public virtual bool Everywhere { get; set; }

    protected override void DoExecute(CommandResultBase result)
    {
      Assert.ArgumentNotNull(result, "result");

      var filter = this.Filter ?? string.Empty;
      var root = !this.Everywhere ? null : Profile.Read().InstancesFolder;

      InstanceManager.Initialize();

      var instances = InstanceManager.Instances.Select(x => new { x.ID, x.Name, x.RootPath, x.WebRootPath, x.DataFolderPath, x.ProductFullName });
      if (!string.IsNullOrEmpty(filter))
      {
        instances = instances.Where(x => x.Name.ToLowerInvariant().Contains(filter.ToLowerInvariant()));
      }

      if (!string.IsNullOrEmpty(root))
      {
        instances = instances.Where(x => x.RootPath.ToLowerInvariant().Contains(root.ToLowerInvariant()));
      }
      
      result.Success = true;
      result.Data = instances.ToDictionary(x => x.Name, x => x);
    }
  }
}