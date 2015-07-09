namespace SIM.Tool.Base.Converters
{
  using System;
  using System.Globalization;
  using System.Windows.Data;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [ValueConversion(typeof(bool), typeof(bool))]
  public class InverseBooleanConverter : IValueConverter
  {
    #region Fields

    public static readonly InverseBooleanConverter Instance = new InverseBooleanConverter();

    #endregion

    #region Implemented Interfaces

    #region IValueConverter

    [NotNull]
    public object Convert([NotNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      Assert.ArgumentNotNull(value, "value");

      if (targetType != typeof(bool))
      {
        throw new InvalidOperationException("The target must be a boolean");
      }

      return !(bool)value;
    }

    [NotNull]
    public object ConvertBack([NotNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      Assert.ArgumentNotNull(value, "value");

      return this.Convert(value, targetType, parameter, culture);
    }

    #endregion

    #endregion
  }
}