namespace SIM.Tool.Windows.UserControls.Install
{
  using System;
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
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));

      var args = (InstallWizardArgs)wizardArgs;
      this.PreHeat.IsChecked = args.PreHeat;
      this.Dictionaries.IsChecked = args.SkipDictionaries;
      this.RadControls.IsChecked = args.SkipRadControls;
      this.ServerSideRedirect.IsChecked = args.ServerSideRedirect;
      this.IncreaseExecutionTimeout.IsChecked = args.IncreaseExecutionTimeout;
    }

    public bool SaveChanges([NotNull] WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));

      var args = (InstallWizardArgs)wizardArgs;
      args.PreHeat = this.PreHeat.IsChecked ?? Throw("PreHeat");
      InstallWizardArgs.SaveLastTimeOption(nameof(args.PreHeat), args.PreHeat);

      args.SkipDictionaries = this.Dictionaries.IsChecked ?? Throw("Dictionaries");
      InstallWizardArgs.SaveLastTimeOption(nameof(args.SkipDictionaries), args.SkipDictionaries);

      args.SkipRadControls = this.RadControls.IsChecked ?? Throw("RadControls");
      InstallWizardArgs.SaveLastTimeOption(nameof(args.SkipDictionaries), args.SkipDictionaries);

      args.ServerSideRedirect = this.ServerSideRedirect.IsChecked ?? Throw("ServerSideRedirect");
      InstallWizardArgs.SaveLastTimeOption(nameof(args.ServerSideRedirect), args.ServerSideRedirect);

      args.IncreaseExecutionTimeout = this.IncreaseExecutionTimeout.IsChecked ?? Throw("IncreaseExecutionTimeout");
      InstallWizardArgs.SaveLastTimeOption(nameof(args.IncreaseExecutionTimeout), args.IncreaseExecutionTimeout);

      return true;
    }

    private bool Throw(string message)
    {
      throw new InvalidOperationException(message);
    }
  }
}