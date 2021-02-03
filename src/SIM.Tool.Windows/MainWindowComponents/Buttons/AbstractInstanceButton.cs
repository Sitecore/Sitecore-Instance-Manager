using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Tool.Base.Plugins;
using Sitecore.Diagnostics.Base;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  public abstract class InstanceOnlyButton : IMainWindowButton
  {
    #region Fields

    private readonly string _label;

    #endregion

    #region Public methods

    public virtual bool IsEnabled(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(mainWindow, nameof(mainWindow));

      return instance != null;
    }

    public virtual bool IsVisible(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        switch (instance.Type)
        {
          case Instance.InstanceType.SitecoreMember:
          {
            return ButtonsConfiguration.Instance.Sitecore9AndLaterMemberButtons.Contains(this._label);
          }
          case Instance.InstanceType.Sitecore9AndLater:
          {
            return ButtonsConfiguration.Instance.Sitecore9AndLaterButtons.Contains(this._label);
          }
          case Instance.InstanceType.Sitecore8AndEarlier:
          {
            return ButtonsConfiguration.Instance.Sitecore8AndEarlierButtons.Contains(this._label);
          }
          case Instance.InstanceType.SitecoreContainer:
          {
            return ButtonsConfiguration.Instance.SitecoreContainersButtons.Contains(this._label);
          }
        }
      }

      return false;
    }

    public abstract void OnClick([CanBeNull] Window mainWindow, Instance instance);

    #endregion

    #region Protected methods

    protected InstanceOnlyButton()
    {
      this._label = this.GetType().Name;
    }

    #endregion
  }
}