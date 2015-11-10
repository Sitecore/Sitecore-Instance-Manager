namespace SIM.Pipelines.InstallModules
{
  using System.Collections.Generic;
  using System.Linq;
  using SIM.Instances;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;

  #region

  #endregion

  public class InstallActions : InstallModulesProcessor
  {
    #region Fields

    private readonly List<Product> done = new List<Product>();

    #endregion

    #region Methods

    protected override bool IsRequireProcessing(InstallModulesArgs args)
    {
      return !this.ProcessorDefinition.Param.EqualsIgnoreCase("archive") || (args.Modules != null && args.Modules.Any(m => m != null && m.IsArchive));
    }

    protected override void Process(InstallModulesArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Instance instance = args.Instance;
      IEnumerable<Product> modules = args.Modules;
      string param = this.ProcessorDefinition.Param;
      ConfigurationActions.ExecuteActions(instance, modules.ToArray(), this.done, param, args.ConnectionString, this.Controller);
    }

    #endregion
  }
}