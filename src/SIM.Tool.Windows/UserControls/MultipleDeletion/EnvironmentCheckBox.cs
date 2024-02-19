namespace SIM.Tool.Windows.UserControls.MultipleDeletion
{
  public class EnvironmentCheckBox : DataObjectBase, IEnvironmentCheckBox
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

    public string Name { get; }

    public EnvironmentCheckBox(string name)
    {
      Name = name;
    }

    public override string ToString()
    {
      return Name;
    }
  }
}