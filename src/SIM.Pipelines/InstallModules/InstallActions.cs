namespace SIM.Pipelines.InstallModules
{
  using System.Collections.Generic;
  using System.Linq;
  using SIM.Instances;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using SIM.Extensions;

  #region

  #endregion

  public class InstallActions : InstallModulesProcessor
  {
    #region Fields

    private readonly List<Product> _Done = new List<Product>();

    #endregion

    #region Methods

    protected override bool IsRequireProcessing(InstallModulesArgs args)
    {
      return !ProcessorDefinition.Param.EqualsIgnoreCase("archive") || (args._Modules != null && args._Modules.Any(m => m != null && m.IsArchive));
    }

    protected override void Process(InstallModulesArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      Instance instance = args.Instance;
      IEnumerable<Product> modules = args._Modules;
      var param = ProcessorDefinition.Param;
      ConfigurationActions.ExecuteActions(instance, modules.ToArray(), _Done, param, args.ConnectionString, Controller);
    }

    #endregion
  }
}