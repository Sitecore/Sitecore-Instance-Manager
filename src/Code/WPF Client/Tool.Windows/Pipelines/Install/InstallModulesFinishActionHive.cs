#region Usings

using System;
using System.Collections.Generic;
using System.Xml;
using SIM.Products;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Wizards;

#endregion

namespace SIM.Tool.Windows.Pipelines.Install
{
  #region

  

  #endregion

  /// <summary>
  ///   TODO: Update summary.
  /// </summary>
  public class InstallModulesFinishActionHive : FinishActionHive
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="InstallModulesFinishActionHive"/> class.
    /// </summary>
    /// <param name="type">
    /// The type. 
    /// </param>
    public InstallModulesFinishActionHive(Type type) : base(type)
    {
    }

    #endregion

    #region Public methods

    /// <summary>
    /// The get finish actions.
    /// </summary>
    /// <param name="wizardArgs">
    /// The wizard args. 
    /// </param>
    /// <returns>
    /// </returns>
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