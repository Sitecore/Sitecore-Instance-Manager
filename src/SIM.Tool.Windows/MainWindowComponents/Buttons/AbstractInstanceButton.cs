using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Products;
using SIM.Tool.Base.Plugins;
using Sitecore.Diagnostics.Base;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  public abstract class InstanceOnlyButton : IMainWindowButton
  {
    #region Fields

    private string label;

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
        if (this.IsSitecoreMember(instance))
        {
          return ButtonsConfiguration.Instance.Sitecore9AndLaterMemberButtons.Contains(this.label);
        }
        if (this.IsSitecore9AndLater(instance))
        {
          return ButtonsConfiguration.Instance.Sitecore9AndLaterButtons.Contains(this.label);
        }
        if (this.IsSitecore8AndEarlier(instance))
        {
          return ButtonsConfiguration.Instance.Sitecore8AndEarlierButtons.Contains(this.label);
        }
      }

      return false;
    }

    public virtual void OnClick([CanBeNull] Window mainWindow, Instance instance)
    {
    }

    #endregion

    #region Protected methods

    protected InstanceOnlyButton()
    {
      this.label = this.GetType().Name;
    }

    protected bool IsSitecoreMember(Instance selectedInstance)
    {
      if (selectedInstance.Product == Product.Undefined || selectedInstance.Product.Release == null)
      {
        return true;
      }

      return false;
    }

    protected bool IsSitecore9AndLater(Instance selectedInstance)
    {
      if (selectedInstance.Product.Release.Version.MajorMinorInt >= 90)
      {
        return true;
      }

      return false;
    }

    protected bool IsSitecore8AndEarlier(Instance selectedInstance)
    {
      if (selectedInstance.Product.Release.Version.MajorMinorInt < 90)
      {
        return true;
      }

      return false;
    }

    protected bool IsSitecore90AndEarlier(Instance selectedInstance)
    {
      if (selectedInstance.Product.Release.Version.MajorMinorInt <= 90)
      {
        return true;
      }

      return false;
    }

    #endregion
  }
}