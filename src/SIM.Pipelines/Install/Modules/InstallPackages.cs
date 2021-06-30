namespace SIM.Pipelines.Install.Modules
{
  using System.Collections.Generic;
  using System.Linq;
  using SIM.Pipelines.Agent;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Pipelines.InstallModules;

  #region

  #endregion

  [UsedImplicitly]
  public class InstallPackages : InstallModulesProcessor
  {
    #region Fields

    private readonly List<Product> _Done = new List<Product>();

    #endregion

    #region Methods

    protected override void Process([NotNull] InstallModulesArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      Assert.IsNotNull(args.Instance, "Instance");

      foreach (var module in args._Modules.Where(m => m.IsPackage))
      {
        if (_Done.Contains(module))
        {
          continue;
        }

        AgentHelper.InstallPackage(args.Instance, module, args.Cookies, args.Headers);

        _Done.Add(module);
      }
    }

    #endregion
  }
}