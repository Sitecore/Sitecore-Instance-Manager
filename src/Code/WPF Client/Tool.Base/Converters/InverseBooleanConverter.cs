#region Usings

using System;
using System.Globalization;
using System.Windows.Data;
using SIM.Base;

#endregion

namespace SIM.Tool.Base.Converters
{
  #region

  

  #endregion

  /// <summary>
  ///   The inverse boolean converter.
  /// </summary>
  [ValueConversion(typeof(bool), typeof(bool))]
  public class InverseBooleanConverter : IValueConverter
  {
    #region Fields

    /// <summary>
    ///   The instance.
    /// </summary>
    public static readonly InverseBooleanConverter Instance = new InverseBooleanConverter();

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
    /// <exception cref="InvalidOperationException">
    /// The target must be a boolean
    /// </exception>
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
    public object ConvertBack([NotNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      Assert.ArgumentNotNull(value, "value");

      return this.Convert(value, targetType, parameter, culture);
    }

    #endregion

    #endregion
  }
}