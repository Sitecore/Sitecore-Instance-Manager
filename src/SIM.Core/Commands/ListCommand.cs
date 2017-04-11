namespace SIM.Core.Commands
{
  using System.IO;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Core.Common;
  using SIM.Core.Models;
  using SIM.Instances;
  using SIM.IO;

  public class ListCommand : AbstractCommand<ListCommandResult>
  {
    public ListCommand([NotNull] IFileSystem fileSystem) : base(fileSystem)
    {
    }

    [CanBeNull]
    public virtual string Filter { get; set; }

    public virtual bool Detailed { get; set; }

    public virtual bool Everywhere { get; set; }

    protected override void DoExecute(CommandResult<ListCommandResult> result)
    {
      Assert.ArgumentNotNull(result, nameof(result));

      var filter = Filter ?? string.Empty;
      var root = !Everywhere ? null : Profile.Read(FileSystem).InstancesFolder;

      InstanceManager.Default.Initialize();

      var instances = InstanceManager.Default.Instances;
      if (!string.IsNullOrEmpty(filter))
      {
        instances = instances.Where(x => x.Name.ToLowerInvariant().Contains(filter.ToLowerInvariant()));
      }

      if (!string.IsNullOrEmpty(root))
      {
        instances = instances.Where(x => x.RootPath.ToLowerInvariant().Contains(root.ToLowerInvariant()));
      }

      ListCommandResult data;

      if (Detailed)
      {
        data = new ListCommandResult(instances.Select(x => new InstanceInfo(x.ID, x.Name, x.State.ToString(), x.WebRootPath)
        {
          DataFolder = SIM.Safe.Call(() => new DirectoryInfo(x.DataFolderPath)),
          RootFolder = SIM.Safe.Call(() => new DirectoryInfo(x.RootPath)),
          ProductName = SIM.Safe.Call(() => x.ProductFullName),
          Databases = SIM.Safe.Call(() => x.AttachedDatabases.ToDictionary(z => z.Name, z => z.RealName)),
          ProcessIds = SIM.Safe.Call(() => x.ProcessIds)
        }));
      }
      else
      {
        data = new ListCommandResult(instances.Select(x => x.Name));
      }

      foreach (var instance in data.Instances)
      {
        CreateEmptyFileInCurrentDirectory(instance);
      }

      result.Data = data;
    }

    private static void CreateEmptyFileInCurrentDirectory(string command)
    {
      try
      {
        File.Create(command).Close();
      }
      catch
      {
        Log.Warn($"Cannot create file: {command}");
      }
    }
  }
}