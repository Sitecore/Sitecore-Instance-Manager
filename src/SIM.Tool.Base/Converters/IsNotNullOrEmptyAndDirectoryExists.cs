namespace SIM.Tool.Base.Converters
{
  using System;
  using System.Globalization;
  using System.Windows.Data;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  [ValueConversion(typeof(string), typeof(bool))]
  public class IsNotNullOrEmptyAndDirectoryExists : IValueConverter
  {
    #region Fields

    public static readonly IsNotNullOrEmptyAndDirectoryExists Instance = new IsNotNullOrEmptyAndDirectoryExists();

    #endregion

    #region Implemented Interfaces

    #region IValueConverter

    [NotNull]
    public object Convert([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      string str = value as string;
      Assert.IsNotNull(str, "Value must have string type");
      return !string.IsNullOrEmpty(str) && FileSystem.FileSystem.Local.Directory.Exists(str);
    }

    [NotNull]
    public object ConvertBack([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      throw new InvalidOperationException("It's impossible to convert back");
    }

    #endregion

    #endregion
  }
}