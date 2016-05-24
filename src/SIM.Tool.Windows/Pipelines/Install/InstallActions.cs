namespace SIM.Tool.Windows.Pipelines.Install
{
  #region

  using SIM.Pipelines.Backup;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Windows.MainWindowComponents;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core;
  using SIM.Tool.Base.Wizards;

  #endregion

  [UsedImplicitly]
  public static class InstallActions
  {
    #region Public methods

    [UsedImplicitly]
    public static void BackupInstance(InstallModulesWizardArgs args)
    {
      int id = MainWindowHelper.GetListItemID(args.Instance.ID);
      Assert.IsTrue(id >= 0, "id ({0}) should be >= 0".FormatWith(id));
      WizardPipelineManager.Start("backup", args.WizardWindow, new BackupArgs(args.Instance, null, true, true), null, () => MainWindowHelper.MakeInstanceSelected(id), args.Instance);
    }

    [UsedImplicitly]
    public static void BackupInstance(InstallWizardArgs args)
    {
      int id = MainWindowHelper.GetListItemID(args.Instance.ID);
      Assert.IsTrue(id >= 0, "id ({0}) should be >= 0".FormatWith(id));
      WizardPipelineManager.Start("backup", args.WizardWindow, new BackupArgs(args.Instance, null, true, true), null, () => MainWindowHelper.MakeInstanceSelected(id), args.Instance);
    }

    [UsedImplicitly]
    public static void OpenBrowser(InstallWizardArgs args)
    {
      InstanceHelperEx.BrowseInstance(args.Instance, args.WizardWindow, string.Empty, true);
    }

    [UsedImplicitly]
    public static void OpenBrowser(InstallModulesWizardArgs args)
    {
      InstanceHelperEx.BrowseInstance(args.Instance, args.WizardWindow, string.Empty, true);
    }

    [UsedImplicitly]
    public static void OpenSitecoreClient(InstallWizardArgs args)
    {
      InstanceHelperEx.BrowseInstance(args.Instance, args.WizardWindow, "/sitecore", false);
    }

    [UsedImplicitly]
    public static void OpenSitecoreClient(InstallModulesWizardArgs args)
    {
      InstanceHelperEx.BrowseInstance(args.Instance, args.WizardWindow, "/sitecore", false);
    }

    [UsedImplicitly]
    public static void OpenVisualStudio(InstallModulesWizardArgs args)
    {
      new OpenVisualStudioButton().OnClick(args.WizardWindow.Owner, args.Instance);
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
    public static void OpenWebsiteFolder(InstallModulesWizardArgs args)
    {
      CoreApp.OpenFolder(args.Instance.WebRootPath);
    }

    #endregion
  }
}