#region Usings

using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using SIM.Base;

#endregion

namespace SIM.Tool.Base.Converters
{
  #region

  

  #endregion

  /// <summary>
  ///   The is not null or empty and directory exists.
  /// </summary>
  [ValueConversion(typeof(string), typeof(bool))]
  public class IsNotNullOrEmptyAndDirectoryExists : IValueConverter
  {
    #region Fields

    /// <summary>
    ///   The instance.
    /// </summary>
    public static readonly IsNotNullOrEmptyAndDirectoryExists Instance = new IsNotNullOrEmptyAndDirectoryExists();

    #endregion

    #region Implemented Interfaces

    #region IValueConverter

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">
    /// The value produced by the binding source. 
    /// </param>
    /// <param name="targetType">
    /// The type of the binding target property. 
    /// </param>
    /// <param name="parameter">
    /// The converter parameter to use. 
    /// </param>
    /// <param name="culture">
    /// The culture to use in the converter. 
    /// </param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used. 
    /// </returns>
    [NotNull]
    public object Convert([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      string str = value as string;
      Assert.IsNotNull(str, "Value must have string type");
      return !string.IsNullOrEmpty(str) && FileSystem.Local.Directory.Exists(str);
    }

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">
    /// The value that is produced by the binding target. 
    /// </param>
    /// <param name="targetType">
    /// The type to convert to. 
    /// </param>
    /// <param name="parameter">
    /// The converter parameter to use. 
    /// </param>
    /// <param name="culture">
    /// The culture to use in the converter. 
    /// </param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used. 
    /// </returns>
    [NotNull]
    public object ConvertBack([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      throw new InvalidOperationException("It's impossible to convert back");
    }

    #endregion

    #endregion
  }
}