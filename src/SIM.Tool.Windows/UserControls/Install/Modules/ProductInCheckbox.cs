namespace SIM.Tool.Windows.UserControls.Install.Modules
{
  using System.Linq;
  using SIM.Products;

  #region

  #endregion

  public class ProductInCheckbox : DataObjectBase
  {
    #region Fields

    private readonly string name;

    private readonly Product[] products;

    private bool isChecked;

    private Product value;

    #endregion

    #region Constructors

    public ProductInCheckbox(Product value)
    {
      this.Value = value;
      this.products = null;
    }

    public ProductInCheckbox(string name, Product[] products)
    {
      this.name = name;
      this.products = products;
      this.Value = products.First();
    }

    #endregion

    #region Properties

    public bool IsChecked
    {
      get
      {
        return this.isChecked;
      }

      set
      {
        this.isChecked = value;
        this.NotifyPropertyChanged("IsChecked");
      }
    }

    public string Name
    {
      get
      {
        return this.name ?? this.Value.Label ?? this.Value.Name;
      }
    }

    public Product[] Scope
    {
      get
      {
        return this.products;
      }
    }

    public Product Value
    {
      get
      {
        return this.value;
      }

      set
      {
        this.value = value;
        this.NotifyPropertyChanged("Value");
        this.NotifyPropertyChanged("ValueIndex");
      }
    }

    public int ValueIndex
    {
      get
      {
        var i = 0;
        foreach (var product in this.Scope)
        {
          if (product == this.Value)
          {
            return i;
          }

          i++;
        }

        return -1;
      }

      set
      {
        this.value = this.Scope[value];
        this.NotifyPropertyChanged("ValueIndex");
      }
    }

    #endregion

    #region Public methods

    public override string ToString()
    {
      return this.Value.Name;
    }

    #endregion
  }
}