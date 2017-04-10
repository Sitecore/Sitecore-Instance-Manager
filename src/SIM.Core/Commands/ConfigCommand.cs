namespace SIM.Core.Commands
{
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Core.Models.Configuration;
  using SIM.Extensions;
  using SIM.Instances;
  using SIM.IO;

  public class ConfigCommand : AbstractCommand<ConfigDatabaseInfo>
  {
    public ConfigCommand([NotNull] IFileSystem fileSystem) : base(fileSystem)
    {                                                         
    }

    public virtual string Name { get; set; }

    public virtual string Database { get; set; }

    protected override void DoExecute(CommandResult<ConfigDatabaseInfo> result)
    {
      Assert.ArgumentNotNullOrEmpty(Name, nameof(Name));

      var instance = InstanceManager.Default.GetInstance(Name);
      Ensure.IsNotNull(instance, "The {0} instance is not found", Name);

      var config = instance.GetShowconfig();
      Assert.IsNotNull(config, nameof(config));

      var database = config.SelectSingleElement($"/sitecore/databases/database[@id='{Database}']");
      Ensure.IsNotNull(database, "The {0} database cannot be found", Database);

      result.Success = true;
      result.Data = ConfigDatabaseInfo.FromXml(database);
    }
  }
}