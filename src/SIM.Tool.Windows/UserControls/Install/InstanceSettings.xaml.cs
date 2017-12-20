namespace SIM.Tool.Windows.UserControls.Install
{
  using System;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  public partial class InstanceSettings : IWizardStep
  {
    public InstanceSettings()
    {
      InitializeComponent();
    }

    public void InitializeStep([NotNull] WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));

      var args = (InstallWizardArgs)wizardArgs;
      PreHeat.IsChecked = args.PreHeat;
      Dictionaries.IsChecked = args.SkipDictionaries;
      RadControls.IsChecked = args.SkipRadControls;
      ServerSideRedirect.IsChecked = args.ServerSideRedirect;
      IncreaseExecutionTimeout.IsChecked = args.IncreaseExecutionTimeout;
    }

    public bool SaveChanges([NotNull] WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));

      var args = (InstallWizardArgs)wizardArgs;
      args.PreHeat = PreHeat.IsChecked ?? Throw("PreHeat");
      InstallWizardArgs.SaveLastTimeOption(nameof(args.PreHeat), args.PreHeat);

      args.SkipDictionaries = Dictionaries.IsChecked ?? Throw("Dictionaries");
      InstallWizardArgs.SaveLastTimeOption(nameof(args.SkipDictionaries), args.SkipDictionaries);

      args.SkipRadControls = RadControls.IsChecked ?? Throw("RadControls");
      InstallWizardArgs.SaveLastTimeOption(nameof(args.SkipRadControls), args.SkipRadControls);

      args.ServerSideRedirect = ServerSideRedirect.IsChecked ?? Throw("ServerSideRedirect");
      InstallWizardArgs.SaveLastTimeOption(nameof(args.ServerSideRedirect), args.ServerSideRedirect);

      args.IncreaseExecutionTimeout = IncreaseExecutionTimeout.IsChecked ?? Throw("IncreaseExecutionTimeout");
      InstallWizardArgs.SaveLastTimeOption(nameof(args.IncreaseExecutionTimeout), args.IncreaseExecutionTimeout);

      return true;
    }

    private bool Throw(string message)
    {
      throw new InvalidOperationException(message);
    }
  }
}