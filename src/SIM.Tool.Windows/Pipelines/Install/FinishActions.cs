namespace SIM.Tool.Windows.Pipelines.Reinstall
{
  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;

  using SIM.Core;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Windows.MainWindowComponents.Buttons;

  [UsedImplicitly]
  public static class FinishActions
  {
    [UsedImplicitly]
    public static void OpenBrowser(ReinstallWizardArgs args)
    {
      InstanceHelperEx.BrowseInstance(args.Instance, args.WizardWindow, string.Empty, true);
    }

    [UsedImplicitly]
    public static void OpenSitecoreClient(ReinstallWizardArgs args)
    {
      InstanceHelperEx.BrowseInstance(args.Instance, args.WizardWindow, "/sitecore", false);
    }

    [UsedImplicitly]
    public static void OpenVisualStudio(ReinstallWizardArgs args)
    {
      new OpenVisualStudioButton().OnClick(args.WizardWindow.Owner, args.Instance);
    }

    [UsedImplicitly]
    public static void OpenWebsiteFolder(ReinstallWizardArgs args)
    {
      CoreApp.OpenFolder(args.Instance.WebRootPath);
    }

    [UsedImplicitly]
    public static void LoginAdmin([NotNull] ReinstallWizardArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var instance = args.Instance;
      Assert.IsNotNull(instance, nameof(instance));

      InstanceHelperEx.OpenInBrowserAsAdmin(instance, MainWindow.Instance);
    }
  }
}