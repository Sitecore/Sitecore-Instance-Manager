namespace SIM.Core.Commands
{
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Core.Models.Configuration;
  using SIM.Instances;

  public class ConfigCommand : AbstractCommand<ConfigDatabaseInfo>
  {
    public virtual string Name { get; set; }

    public virtual string Database { get; set; }

    protected override void DoExecute(CommandResult<ConfigDatabaseInfo> result)
    {
      Assert.ArgumentNotNullOrEmpty(Name, "Name");

      var instance = InstanceManager.GetInstance(Name);
      Ensure.IsNotNull(instance, "The {0} instance is not found", Name);

      var config = instance.GetShowconfig();
      Assert.IsNotNull(config, "config");

      var database = config.SelectSingleElement($"/sitecore/databases/database[@id='{Database}']");
      Ensure.IsNotNull(database, "The {0} database cannot be found", Database);

      result.Success = true;
      result.Data = ConfigDatabaseInfo.FromXml(database);
    }
  }
}