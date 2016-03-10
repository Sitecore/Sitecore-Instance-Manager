namespace SIM.Tool.Windows.Pipelines.Install
{
  using System;
  using System.Collections.Generic;
  using System.Xml;
  using SIM.Products;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Wizards;

  #region

  #endregion

  public class InstallModulesFinishActionHive : FinishActionHive
  {
    #region Constructors

    public InstallModulesFinishActionHive(Type type) : base(type)
    {
    }

    #endregion

    #region Public methods

    public override IEnumerable<FinishAction> GetFinishActions(WizardArgs wizardArgs)
    {
      InstallModulesWizardArgs installModulesWizardArgs = (InstallModulesWizardArgs)wizardArgs;
      List<Product> modules = installModulesWizardArgs.Modules;
      foreach (Product module in modules)
      {
        XmlDocument manifest = module.Manifest;
        if (manifest != null && manifest != Product.EmptyManifest)
        {
          XmlElement finish = (XmlElement)manifest.SelectSingleNode("manifest/*/finish");
          if (finish == null)
          {
            continue;
          }

          foreach (FinishAction action in WizardPipelineManager.GetFinishActions(finish, this.WizardArgumentsType))
          {
            yield return new FinishAction(action.Text, action.Method);
          }
        }
      }
    }

    #endregion
  }
}