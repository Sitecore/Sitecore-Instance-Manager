namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Linq;
  using System.Windows;
  using SIM.Core.Common;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public class BrowseButton : IMainWindowButton
  {
    #region Fields

    [CanBeNull]
    protected string Browser { get; }

    [NotNull]
    protected string VirtualPath { get; }

    [NotNull]
    private readonly string[] _Params;

    #endregion

    #region Constructors

    public BrowseButton()
    {
      VirtualPath = string.Empty;
      Browser = null;
      _Params = new string[0];
    }

    public BrowseButton([CanBeNull] string param)
    {
      var arr = (param + ":").Split(':');
      VirtualPath = arr[0];
      Browser = arr[1];
      _Params = arr.Skip(2).ToArray();
    }

    #endregion

    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Analytics.TrackEvent("Browse");

      if (instance != null)
      {
        if (!InstanceHelperEx.PreheatInstance(instance, mainWindow))
        {
          return;
        }

        InstanceHelperEx.BrowseInstance(instance, mainWindow, VirtualPath, true, Browser, _Params);
      }
    }

    #endregion
  }
}