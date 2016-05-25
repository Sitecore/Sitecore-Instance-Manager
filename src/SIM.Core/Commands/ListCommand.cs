namespace SIM.Core.Commands
{
  using System.Collections;
  using System.IO;
  using System.Linq;
  using SIM.Core.Common;
  using SIM.Core.Models;
  using SIM.Instances;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class ListCommand : AbstractCommand<ListCommandResult>
  {
    [CanBeNull]
    public virtual string Filter { get; set; }

    public virtual bool Detailed { get; set; }

    public virtual bool Everywhere { get; set; }

    protected override void DoExecute(CommandResult<ListCommandResult> result)
    {
      Assert.ArgumentNotNull(result, "result");

      var filter = this.Filter ?? string.Empty;
      var root = !this.Everywhere ? null : Profile.Read().InstancesFolder;

      InstanceManager.Initialize();

      var instances = InstanceManager.Instances;
      if (!string.IsNullOrEmpty(filter))
      {
        instances = instances.Where(x => x.Name.ToLowerInvariant().Contains(filter.ToLowerInvariant()));
      }

      if (!string.IsNullOrEmpty(root))
      {
        instances = instances.Where(x => x.RootPath.ToLowerInvariant().Contains(root.ToLowerInvariant()));
      }

      ListCommandResult data;

      if (this.Detailed)
      {
        data = new ListCommandResult(instances.Select(x => new InstanceInfo(x.ID, x.Name, x.State.ToString(), x.WebRootPath)
        {
          DataFolder = Null.Safe(() => new DirectoryInfo(x.DataFolderPath)), 
          RootFolder = Null.Safe(() => new DirectoryInfo(x.RootPath)),
          ProductName = Null.Safe(() => x.ProductFullName), 
          Databases = Null.Safe(() => x.AttachedDatabases.ToDictionary(z => z.Name, z => z.RealName)),
          ProcessIds = Null.Safe(() => x.ProcessIds)
        }));
      }
      else
      {
        data = new ListCommandResult(instances.Select(x => x.Name));
      }
      
      result.Data = data;
    }
  }
}