namespace SIM.Tool.Base.Pipelines
{
  using System.Data.SqlClient;

  using SIM.Instances;
  using SIM.Pipelines.Install;
  using SIM.Pipelines.Processors;
  using SIM.Pipelines.Reinstall;
  using SIM.Sitecore9Installer;
  using SIM.Tool.Base.Wizards;

  public class ReinstallWizardArgs : WizardArgs
  {
    public ReinstallWizardArgs(Instance instance, SqlConnectionStringBuilder connectionString, string license)
    {
      Instance = instance;
      InstanceName = instance.Name;
      ConnectionString = connectionString;
      License = license;
    }

    public Instance Instance { get; }

    public string InstanceName { get; }

    public SqlConnectionStringBuilder ConnectionString { get; }

    private string License { get; }

    public Tasker Tasker { get; set; }

    public override ProcessorArgs ToProcessorArgs()
    {
      if (int.Parse(this.Instance.Product.ShortVersion) >= 90)
      {
        return new Reinstall9Args(this.Tasker);
      }

      return new ReinstallArgs(this.Instance, this.ConnectionString, this.License, Settings.CoreInstallWebServerIdentity.Value, Settings.CoreInstallNotFoundTransfer.Value);
    }
  }
}