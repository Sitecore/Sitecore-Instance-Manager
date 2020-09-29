using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Tool.Base.Plugins;
using Sitecore.Diagnostics.Base;
using Sitecore.Diagnostics.Logging;

namespace SIM.Tool.Windows.CustomConverters
{
  public class CustomMenuItemVisibilityConverter : IValueConverter
  {
    #region Fields

    [NotNull]
    private IMainWindowButton MenuItem { get; }

    #endregion

    #region Constructors

    public CustomMenuItemVisibilityConverter([NotNull] IMainWindowButton mainWindowMenuItem)
    {
      Assert.ArgumentNotNull(mainWindowMenuItem, nameof(mainWindowMenuItem));

      MenuItem = mainWindowMenuItem;
    }

    #endregion

    #region Public methods

    [CanBeNull]
    public object Convert([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      using (new ProfileSection("Checking if menu item is visible", this))
      {
        ProfileSection.Argument("this.menuitem", MenuItem.GetType().FullName);
        return MenuItem.IsVisible(MainWindow.Instance, value as Instance) ? Visibility.Visible : Visibility.Collapsed;
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
