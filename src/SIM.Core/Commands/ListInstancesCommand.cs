namespace SIM.Core.Commands
{
  using System.Linq;
  using SIM.Core.Common;
  using SIM.Instances;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class ListCommand : AbstractCommand
  {
    [CanBeNull]
    public virtual string Filter { get; set; }

    public virtual bool Detailed { get; set; }

    public virtual bool Everywhere { get; set; }

    protected override void DoExecute(CommandResultBase result)
    {
      Assert.ArgumentNotNull(result, "result");

      var filter = this.Filter ?? string.Empty;
      var root = !this.Everywhere ? null : Profile.Read().InstancesFolder;

      InstanceManager.Initialize();

      object data;
      if (this.Detailed)
      {
        var instances = InstanceManager.Instances.Select(x => new { x.ID, x.Name, x.State, x.WebRootPath, x.DataFolderPath, x.RootPath, x.ProductFullName, Databases = x.AttachedDatabases.ToDictionary(y => y.Name, y => y.RealName) });
        if (!string.IsNullOrEmpty(filter))
        {
          instances = instances.Where(x => x.Name.ToLowerInvariant().Contains(filter.ToLowerInvariant()));
        }

        if (!string.IsNullOrEmpty(root))
        {
          instances = instances.Where(x => x.RootPath.ToLowerInvariant().Contains(root.ToLowerInvariant()));
        }

        data = instances.ToDictionary(x => x.Name, x => x);
      }
      else
      {
        data = InstanceManager.Instances.Select(x => new { x.ID, x.Name, x.State, x.WebRootPath });
      }

      result.Success = true;
      result.Message = "done";
      result.Data = data;
    }
  }
}