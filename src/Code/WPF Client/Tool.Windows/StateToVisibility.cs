namespace SIM.Tool.Windows
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;
  using SIM.Base;
  using SIM.Instances;

  public class StateToVisibility : IValueConverter
  {
    [CanBeNull]
    public object Convert([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      return (value ?? "").ToString() == (string)parameter ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new System.NotImplementedException();
    }
  }
}