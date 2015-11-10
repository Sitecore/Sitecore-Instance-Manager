namespace SIM.Pipelines.InstallModules
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
  public class InstallPackages : InstallModulesProcessor
  {
    #region Methods

    #region Fields

    private readonly List<Product> done = new List<Product>();

    #endregion

    #region Protected methods

    protected override void Process([NotNull] InstallModulesArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      foreach (Product module in args.Modules.Where(m => m.IsPackage))
      {
        if (this.done.Contains(module))
        {
          continue;
        }

        AgentHelper.InstallPackage(args.Instance, module);

        this.done.Add(module);
      }
    }

    #endregion

    #endregion
  }
}