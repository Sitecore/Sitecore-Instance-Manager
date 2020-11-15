using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using SIM.Instances;
using SIM.Tool.Base.Plugins;
using Sitecore.Diagnostics.Base;
using JetBrains.Annotations;
using Sitecore.Diagnostics.Logging;

namespace SIM.Tool.Windows.CustomConverters
{
  public class CustomButtonVisibilityConverter : IValueConverter
  {
    #region Fields

    [NotNull]
    private IMainWindowButton Button { get; }

    #endregion

    #region Constructors

    public CustomButtonVisibilityConverter([NotNull] IMainWindowButton mainWindowButton)
    {
      Assert.ArgumentNotNull(mainWindowButton, nameof(mainWindowButton));

      this.Button = mainWindowButton;
    }

    #endregion

    #region Public methods

    [CanBeNull]
    public object Convert([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      using (new ProfileSection("Checking if button is visible", this))
      {
        ProfileSection.Argument("this.button", this.Button.GetType().FullName);
        return this.Button.IsVisible(MainWindow.Instance, value as Instance) ? Visibility.Visible : Visibility.Collapsed;
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