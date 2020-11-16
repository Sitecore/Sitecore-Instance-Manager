using ContainerInstaller;
using SIM.Sitecore9Installer;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Windows.Dialogs;
using Sitecore.Diagnostics.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using TaskDialogInterop;

namespace SIM.Tool.Windows.UserControls.Install.Containers
{
  /// <summary>
  /// Interaction logic for Instance9SelectTasks.xaml
  /// </summary>
  public partial class SetEnvValues : IWizardStep, IFlowControl, ICustomButton
  {
    private Window owner;
    private EnvModel envModel;
    public SetEnvValues()
    {
      InitializeComponent();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      InstallContainerWizardArgs args = (InstallContainerWizardArgs)wizardArgs;
      this.owner = args.WizardWindow;
      this.envModel = args.EnvModel;
      this.envModel.SitecoreLicense = ProfileManager.Profile.License;
    }    

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      InstallContainerWizardArgs args = (InstallContainerWizardArgs)wizardArgs;

      
      return true;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    public string CustomButtonText { get=>"Advanced..."; }
    public void CustomButtonClick()
    {
      WindowHelper.ShowDialog<ContainerVariablesEditor>(this.envModel.ToList(), this.owner);
    }
  }
}
