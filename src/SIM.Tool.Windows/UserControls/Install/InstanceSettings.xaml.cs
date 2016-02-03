namespace SIM.Tool.Windows.UserControls.Install
{
  using SIM.Pipelines.Install;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public partial class InstanceSettings : IWizardStep
  {
    public InstanceSettings()
    {
      this.InitializeComponent();
    }

    public void InitializeStep([NotNull] WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, "wizardArgs");

      var args = (InstallModulesWizardArgs)wizardArgs;
      this.Dictionaries.IsChecked = args.SkipDictionaries ?? !Settings.CoreInstallDictionaries.Value;
      this.RadControls.IsChecked = args.SkipRadControls ?? !Settings.CoreInstallRadControls.Value;
      this.ServerSideRedirect.IsChecked = args.ServerSideRedirect ?? Settings.CoreInstallNotFoundTransfer.Value;
      this.IncreaseExecutionTimeout.IsChecked = args.IncreaseExecutionTimeout ?? true;
    }

    public bool SaveChanges([NotNull] WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, "wizardArgs");

      var args = (InstallModulesWizardArgs)wizardArgs;
      args.SkipDictionaries = this.Dictionaries.IsChecked ?? true;
      args.SkipRadControls = this.RadControls.IsChecked ?? true;
      args.ServerSideRedirect = this.ServerSideRedirect.IsChecked;
      args.IncreaseExecutionTimeout = this.IncreaseExecutionTimeout.IsChecked;

      return true;
    }
  }
}