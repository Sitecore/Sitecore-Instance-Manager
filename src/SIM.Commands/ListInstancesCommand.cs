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

    protected override void DoExecute(CommandResult result)
    {
      Assert.ArgumentNotNull(result, "result");

      var filter = this.Filter ?? string.Empty;
      filter = filter.ToLowerInvariant();

      InstanceManager.Initialize();
      var instances = InstanceManager.Instances;
      var names = instances.Select(x => x.DisplayName);

      result.Success = true;
      result.Data = string.IsNullOrEmpty(filter) ? names : names.Where(x => x.ToLowerInvariant().Contains(filter));
    }
  }
}