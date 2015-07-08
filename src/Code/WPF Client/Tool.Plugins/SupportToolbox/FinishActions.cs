#region Usings

using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;

#endregion

namespace SIM.Tool.Plugins.SupportToolbox
{
  using SIM.Base;

  public static class FinishActions
  {
    #region Public methods

    /// <summary>
    /// Opens solution.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    [UsedImplicitly]
    public static void OpenToolbox(InstallWizardArgs args)
    {
      if (!InstanceHelperEx.PreheatInstance(args.Instance, args.WizardWindow))
      {
        return;
      }

      InstanceHelperEx.BrowseInstance(args.Instance, args.WizardWindow, "/sitecore/admin", true);
    }

    /// <summary>
    /// Opens solution.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    [UsedImplicitly]
    public static void OpenToolbox(InstallModulesWizardArgs args)
    {
      if (!InstanceHelperEx.PreheatInstance(args.Instance, args.WizardWindow))
      {
        return;
      }

      InstanceHelperEx.BrowseInstance(args.Instance, args.WizardWindow, "/sitecore/admin", true);
    }

    #endregion
  }
}