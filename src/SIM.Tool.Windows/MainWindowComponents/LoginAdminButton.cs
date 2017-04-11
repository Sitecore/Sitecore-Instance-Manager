namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public class LoginAdminButton : IMainWindowButton
  {
    #region Fields

    protected string Browser { get; }
    protected string VirtualPath { get; }
    protected readonly string[] _Params;

    #endregion

    #region Constructors

    public LoginAdminButton()
    {
      VirtualPath = string.Empty;
      Browser = string.Empty;
      _Params = new string[0];
    }

    public LoginAdminButton([NotNull] string param)
    {
      Assert.ArgumentNotNull(param, nameof(param));

      var par = Parameters.Parse(param);
      VirtualPath = par[0];
      Browser = par[1];
      _Params = par.Skip(2);
    }

    #endregion

    #region Public methods

    [UsedImplicitly]
    public static void FinishAction([NotNull] InstallWizardArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var instance = args.Instance;
      Assert.IsNotNull(instance, nameof(instance));

      InstanceHelperEx.OpenInBrowserAsAdmin(instance, MainWindow._Instance);
    }

    [UsedImplicitly]
    public static void FinishAction([NotNull] InstallModulesWizardArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var instance = args.Instance;
      Assert.IsNotNull(instance, nameof(instance));

      InstanceHelperEx.OpenInBrowserAsAdmin(instance, MainWindow._Instance);
    }

    public bool IsEnabled([CanBeNull] Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, nameof(mainWindow));
      Assert.IsNotNull(instance, nameof(instance));

      Analytics.TrackEvent("LogInAdmin");

      InstanceHelperEx.OpenInBrowserAsAdmin(instance, mainWindow, VirtualPath, Browser, _Params);
    }

    #endregion
  }
}