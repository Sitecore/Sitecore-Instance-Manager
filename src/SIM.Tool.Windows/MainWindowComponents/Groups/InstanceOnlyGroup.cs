using System.Linq;
using System.Windows;
using SIM.Instances;
using SIM.Products;
using SIM.Tool.Base.Plugins;

namespace SIM.Tool.Windows.MainWindowComponents.Groups
{
  public class InstanceOnlyGroup : IMainWindowGroup
  {
    #region Fields

    private string label;

    #endregion

    #region Public methods

    public virtual bool IsVisible(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        if (this.IsSitecoreMember(instance))
        {
          return ButtonsConfiguration.Instance.Sitecore9AndLaterMemberGroups.Contains(this.label);
        }
        if (this.IsSitecore9AndLater(instance))
        {
          return ButtonsConfiguration.Instance.Sitecore9AndLaterGroups.Contains(this.label);
        }
        if (this.IsSitecore8AndEarlier(instance))
        {
          return ButtonsConfiguration.Instance.Sitecore8AndEarlierGroups.Contains(this.label);
        }
      }

      return false;
    }

    #endregion

    #region Protected methods

    public InstanceOnlyGroup()
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

    #endregion
  }
}
