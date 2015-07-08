#region Usings

using SIM.Base;
using SIM.Pipelines.Backup;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Wizards;

#endregion

namespace SIM.Tool.Windows.Pipelines.Install
{
  #region
  


  using SIM.Instances;
  using SIM.Tool.Windows.MainWindowComponents;

  #endregion

  /// <summary>
  ///   Install wizard finish steps
  /// </summary>
  [UsedImplicitly]
  public static class InstallActions
  {
    #region Public methods

    /// <summary>
    /// Opens browser.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    [UsedImplicitly]
    public static void OpenBrowser(InstallWizardArgs args)
    {
      InstanceHelperEx.BrowseInstance(args.Instance, args.WizardWindow, "", true);
    }

    /// <summary>
    /// Opens browser.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    [UsedImplicitly]
    public static void OpenBrowser(InstallModulesWizardArgs args)
    {
      InstanceHelperEx.BrowseInstance(args.Instance, args.WizardWindow, "", true);
    }

    /// <summary>
    /// Opens browser.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    [UsedImplicitly]
    public static void OpenSitecoreClient(InstallWizardArgs args)
    {
      InstanceHelperEx.BrowseInstance(args.Instance, args.WizardWindow, "/sitecore", false);
    }

    /// <summary>
    /// Opens browser.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    [UsedImplicitly]
    public static void OpenSitecoreClient(InstallModulesWizardArgs args)
    {
      InstanceHelperEx.BrowseInstance(args.Instance, args.WizardWindow, "/sitecore", false);
    }

    /// <summary>
    /// Opens website folder.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    [UsedImplicitly]
    public static void OpenWebsiteFolder(InstallWizardArgs args)
    {
      WindowHelper.OpenFolder(args.InstanceWebRootPath);
    }

    /// <summary>
    /// Opens website folder.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    [UsedImplicitly]
    public static void OpenWebsiteFolder(InstallModulesWizardArgs args)
    {
      WindowHelper.OpenFolder(args.Instance.WebRootPath);
    }

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
    public static void OpenVisualStudio(InstallModulesWizardArgs args)
    {
      new OpenVisualStudioButton().OnClick(args.WizardWindow.Owner, args.Instance);
    }

    [UsedImplicitly]
    public static void OpenVisualStudio(InstallWizardArgs args)
    {
      new OpenVisualStudioButton().OnClick(args.WizardWindow.Owner, args.Instance);
    }
    

    #endregion
  }
}