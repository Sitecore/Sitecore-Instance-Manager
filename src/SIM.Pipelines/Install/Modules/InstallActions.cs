namespace SIM.Pipelines.Install.Modules
{
  using System.Collections.Generic;
  using System.Linq;
  using SIM.Instances;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using SIM.Extensions;

  #region

  #endregion

  public class InstallActions : InstallProcessor
  {
    #region Fields

    private readonly List<Product> _Done = new List<Product>();

    #endregion

    #region Methods

    protected override bool IsRequireProcessing(InstallArgs args)
    {
      return !this.ProcessorDefinition.Param.EqualsIgnoreCase("archive") || (args._Modules != null && args._Modules.Any(m => m != null && m.IsArchive));
    }

    protected override void Process(InstallArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));
      Assert.IsNotNull(args.Instance, "Instance");

      Instance instance = args.Instance;
      IEnumerable<Product> modules = args._Modules;
      var param = this.ProcessorDefinition.Param;
      ConfigurationActions.ExecuteActions(instance, modules.ToArray(), this._Done, param, args.ConnectionString, this.Controller);
    }

    #endregion
  }
}