namespace SIM.Tool.Windows
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;
  using Sitecore.Diagnostics.Base.Annotations;

  public class StateToVisibility : IValueConverter
  {
    #region Public methods

    [CanBeNull]
    public object Convert([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      return (value ?? string.Empty).ToString() == (string)parameter ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new System.NotImplementedException();
    }

    #endregion
  }
}