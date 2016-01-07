namespace SIM.Commands
{
  using System.Linq;
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
      filter = filter.ToLowerInvariant();

      InstanceManager.Initialize();
      var instances = InstanceManager.Instances;
      var data = instances.ToDictionary(x => x.Name, x => new { x.ID, x.RootPath, x.WebRootPath, x.DataFolderPath, x.ProductFullName });

      result.Success = true;
      result.Data = string.IsNullOrEmpty(filter) ? data : data.Where(x => x.Key.ToLowerInvariant().Contains(filter));
    }
  }
}