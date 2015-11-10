namespace SIM.Tool.Base.Converters
{
  using System;
  using System.Globalization;
  using System.Windows.Data;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  public class Product : IValueConverter
  {
    #region Fields

    public static Product DefaultInstanceName = new Product(p => p.DefaultInstanceName);

    private readonly Func<Products.Product, string> target;

    #endregion

    #region Constructors

    public Product([NotNull] Func<Products.Product, string> target)
    {
      Assert.ArgumentNotNull(target, "target");

      this.target = target;
    }

    #endregion

    #region Implemented Interfaces

    #region IValueConverter

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

    [CanBeNull]
    public object ConvertBack([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      return null;
    }

    #endregion

    #endregion
  }
}