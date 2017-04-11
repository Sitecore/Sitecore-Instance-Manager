namespace SIM.Pipelines.Install.Modules
{
  using System.Collections.Generic;
  using System.Linq;
  using SIM.Pipelines.Agent;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class PerformPostStepActions : InstallProcessor
  {
    #region Fields

    private readonly List<Product> _Done = new List<Product>();

    #endregion

    #region Methods

    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      Assert.IsNotNull(args.Instance, "Instance");

      AgentHelper.ResetStatus(args.Instance);

      foreach (var module in args._Modules.Where(m => m.IsPackage))
      {
        if (this._Done.Contains(module))
        {
          continue;
        }

        AgentHelper.PerformPostStepAction(args.Instance, module);

        this._Done.Add(module);
      }
    }

    #endregion
  }
}