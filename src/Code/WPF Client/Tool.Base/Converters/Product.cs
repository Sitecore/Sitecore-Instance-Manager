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
  ///   The product.
  /// </summary>
  public class Product : IValueConverter
  {
    #region Fields

    /// <summary>
    ///   The default instance name.
    /// </summary>
    public static Product DefaultInstanceName = new Product(p => p.DefaultInstanceName);

    /// <summary>
    ///   The target.
    /// </summary>
    private readonly Func<Products.Product, string> target;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Product"/> class.
    /// </summary>
    /// <param name="target">
    /// The target. 
    /// </param>
    public Product([NotNull] Func<Products.Product, string> target)
    {
      Assert.ArgumentNotNull(target, "target");

      this.target = target;
    }

    #endregion

    #region Implemented Interfaces

    #region IValueConverter

    /// <summary>
    /// The convert.
    /// </summary>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <param name="targetType">
    /// The target Type. 
    /// </param>
    /// <param name="parameter">
    /// The parameter. 
    /// </param>
    /// <param name="culture">
    /// The culture. 
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The target must be a boolean
    /// </exception>
    /// <returns>
    /// The convert. 
    /// </returns>
    [CanBeNull]
    public object Convert([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      if (value == null)
      {
        return null;
      }

      if (value as string == string.Empty)
      {
        return string.Empty;
      }

      Products.Product val = value as Products.Product;
      Assert.IsNotNull(val, "The value must be a Product variable");

      return this.target(val);
    }

    /// <summary>
    /// The convert back.
    /// </summary>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <param name="targetType">
    /// The target type. 
    /// </param>
    /// <param name="parameter">
    /// The parameter. 
    /// </param>
    /// <param name="culture">
    /// The culture. 
    /// </param>
    /// <returns>
    /// The convert back. 
    /// </returns>
    [CanBeNull]
    public object ConvertBack([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      return null;
    }

    #endregion

    #endregion
  }
}