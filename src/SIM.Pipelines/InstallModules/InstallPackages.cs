namespace SIM.Pipelines.InstallModules
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
  public class InstallPackages : InstallModulesProcessor
  {
    #region Methods

    #region Fields

    private readonly List<Product> _Done = new List<Product>();

    #endregion

    #region Protected methods

    protected override void Process([NotNull] InstallModulesArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      foreach (var module in args._Modules.Where(m => m.IsPackage))
      {
        if (_Done.Contains(module))
        {
          continue;
        }

        AgentHelper.InstallPackage(args.Instance, module);

        _Done.Add(module);
      }
    }

    #endregion

    #endregion
  }
}