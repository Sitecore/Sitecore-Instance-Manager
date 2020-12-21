namespace SIM.Tool.Windows.Pipelines.Install
{
  #region

  using System;

  using SIM.Pipelines.Backup;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Windows.MainWindowComponents.Buttons;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Core;
  using SIM.Extensions;
  using SIM.Instances;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.UserControls.Backup;
  using SIM.Tool.Base.Profiles;

  #endregion

  [UsedImplicitly]
  public static class InstallActions
  {
    #region Public methods

    [UsedImplicitly]
    public static void BackupInstance(InstallModulesWizardArgs args)
    {
      var id = MainWindowHelper.GetListItemID(args.Instance.ID);
      Assert.IsTrue(id >= 0, "id ({0}) should be >= 0".FormatWith(id));
      WizardPipelineManager.Start("backup", args.WizardWindow, new BackupArgs(args.Instance, ProfileManager.GetConnectionString(), null, true, true), null, ignore => MainWindowHelper.MakeInstanceSelected(id), () => new BackupSettingsWizardArgs(args.Instance));
    }

    [UsedImplicitly]
    public static void BackupInstance(InstallWizardArgs args)
    {
      var id = MainWindowHelper.GetListItemID(args.Instance.ID);
      Assert.IsTrue(id >= 0, "id ({0}) should be >= 0".FormatWith(id));
      WizardPipelineManager.Start("backup", args.WizardWindow, new BackupArgs(args.Instance, ProfileManager.GetConnectionString(), null, true, true), null, ignore => MainWindowHelper.MakeInstanceSelected(id), () => new BackupSettingsWizardArgs(args.Instance));
    }

    [UsedImplicitly]
    public static void OpenBrowser(InstallWizardArgs args)
    {
      InstanceHelperEx.BrowseInstance(args.Instance, args.WizardWindow, String.Empty, true);
    }

    [UsedImplicitly]
    public static void OpenSitecoreClient(InstallWizardArgs args)
    {
      InstanceHelperEx.BrowseInstance(args.Instance, args.WizardWindow, "/sitecore", false);
    }

    [UsedImplicitly]
    public static void OpenVisualStudio(InstallWizardArgs args)
    {
      new OpenVisualStudioButton().OnClick(args.WizardWindow.Owner, args.Instance);
    }

    [UsedImplicitly]
    public static void OpenWebsiteFolder(InstallWizardArgs args)
    {
      CoreApp.OpenFolder(args.InstanceWebRootPath);
    }

    [UsedImplicitly]
    public static void LoginAdmin([NotNull] InstallWizardArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var instance = args.Instance;
      Assert.IsNotNull(instance, nameof(instance));

      InstanceHelperEx.OpenInBrowserAsAdmin(instance, MainWindow.Instance);
    }

    [UsedImplicitly]
    public static void PublishSite(InstallWizardArgs args)
    {
      MainWindowHelper.RefreshInstances();
      var instance = InstanceManager.Default.GetInstance(args.InstanceName);
      new PublishButton().OnClick(MainWindow.Instance, instance);
    }

    #endregion
  }
}