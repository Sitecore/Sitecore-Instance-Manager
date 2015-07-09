namespace SIM.Tool.Base.Converters
{
  using System;
  using System.Globalization;
  using System.Windows.Data;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [ValueConversion(typeof(object), typeof(bool))]
  public class IsNotNullConverter : IValueConverter
  {
    #region Fields

    public static readonly IsNotNullConverter Instance = new IsNotNullConverter();

    #endregion

    #region Implemented Interfaces

    #region IValueConverter

    [NotNull]
    public object Convert([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      return value != null;
    }

    [NotNull]
    public object ConvertBack([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      return this.Convert(value, targetType, parameter, culture);
    }

    #endregion

    #endregion
  }
}