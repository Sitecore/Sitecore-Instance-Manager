namespace SIM.Tool.Windows.UserControls.Resources
{
  public class ResourceCheckBox : DataObjectBase, IResourceCheckBox
  {
    private bool _IsChecked;

    public bool IsChecked
    {
      get
      {
        return _IsChecked;
      }
      set
      {
        _IsChecked = value;

        NotifyPropertyChanged("IsChecked");
      }
    }

    public string Value { get; }

    public ResourceCheckBox(string value)
    {
      Value = value;
    }

    public override string ToString()
    {
      return Value;
    }
  }
}