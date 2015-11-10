namespace SIM.Pipelines.Install.Modules
{
  using System.Collections.Generic;
  using System.Linq;
  using SIM.Pipelines.Agent;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class PerformPostStepActions : InstallProcessor
  {
    #region Fields

    private readonly List<Product> done = new List<Product>();

    #endregion

    #region Methods

    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Assert.IsNotNull(args.Instance, "Instance");

      AgentHelper.ResetStatus(args.Instance);

      foreach (Product module in args.Modules.Where(m => m.IsPackage))
      {
        if (this.done.Contains(module))
        {
          continue;
        }

        AgentHelper.PerformPostStepAction(args.Instance, module);

        this.done.Add(module);
      }
    }

    #endregion
  }
}