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
      args.SkipDictionaries = this.Dictionaries.IsChecked ?? Throw("Dictionaries");
      args.SkipRadControls = this.RadControls.IsChecked ?? Throw("RadControls");
      args.ServerSideRedirect = this.ServerSideRedirect.IsChecked ?? Throw("ServerSideRedirect");
      args.IncreaseExecutionTimeout = this.IncreaseExecutionTimeout.IsChecked ?? Throw("IncreaseExecutionTimeout");

      return true;
    }

    private bool Throw(string message)
    {
      throw new InvalidOperationException(message);
    }
  }
}