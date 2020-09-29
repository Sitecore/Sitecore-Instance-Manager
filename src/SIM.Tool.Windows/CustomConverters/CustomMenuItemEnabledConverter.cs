using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Tool.Base.Plugins;
using Sitecore.Diagnostics.Base;
using Sitecore.Diagnostics.Logging;

namespace SIM.Tool.Windows.CustomConverters
{
  public class CustomMenuItemEnabledConverter : IValueConverter
  {
    #region Fields

    [NotNull]
    private IMainWindowButton MenuItem { get; }

    #endregion

    #region Constructors

    public CustomMenuItemEnabledConverter([NotNull] IMainWindowButton mainWindowMenuItem)
    {
      Assert.ArgumentNotNull(mainWindowMenuItem, nameof(mainWindowMenuItem));

      MenuItem = mainWindowMenuItem;
    }

    #endregion

    #region Public methods

    [CanBeNull]
    public object Convert([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      using (new ProfileSection("Checking if menu item is enabled", this))
      {
        ProfileSection.Argument("this.menuitem", MenuItem.GetType().FullName);
        return MenuItem.IsEnabled(MainWindow.Instance, value as Instance);
      }
    }

    [CanBeNull]
    public object ConvertBack([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }
}
