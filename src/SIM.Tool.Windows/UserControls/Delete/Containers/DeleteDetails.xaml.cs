using System;
using System.Text;
using SIM.Tool.Base.Wizards;
using JetBrains.Annotations;
using SIM.Tool.Base.Pipelines;
using SIM.SitecoreEnvironments;
using System.Collections.Generic;

namespace SIM.Tool.Windows.UserControls.Delete.Containers
{
  [UsedImplicitly]
  public partial class DeleteDetails : IWizardStep, IFlowControl
  {
    public DeleteDetails()
    {
      InitializeComponent();
    }

    public bool OnMovingBack(WizardArgs wizardArg)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      return true;
    }

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      DeleteContainersWizardArgs args = (DeleteContainersWizardArgs)wizardArgs;

      StringBuilder displayText = new StringBuilder();

      SitecoreEnvironment environment;
      if (!SitecoreEnvironmentHelper.TryGetEnvironmentById(args.EnvironmentId, out environment))
      {
        throw new InvalidOperationException($"Could not resolve environment by ID'{args.EnvironmentId}'");
      }

      this.ListHeader.Text = string.Format("Deleting '{0}':", environment.Name);

      List<SitecoreEnvironmentMember> members = environment.Members;

      foreach (var member in members)
      {
        displayText.AppendLine(string.Format(" -{0}", member.Name));
      }

      this.TextBlock.Text = displayText.ToString();
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }
  }
}