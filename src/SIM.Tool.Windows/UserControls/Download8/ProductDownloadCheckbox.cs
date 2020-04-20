using System;

namespace SIM.Tool.Windows.UserControls.Download8
{
  public class ProductDownloadCheckbox : DataObjectBase, IProductDownloadCheckBox
  {
    private Uri _Value;
    private bool _IsChecked;
    protected string Label { get; private set; }
    protected string Revision { get; private set; }
    protected string Version { get; private set; }

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

    public bool IsEnabled { get; }

    public string Name { get; }

    public Uri Value
    {
      get
      {
        return _Value;
      }
      set
      {
        _Value = value;

        NotifyPropertyChanged("Value");
      }
    }

    public ProductDownloadCheckbox(
      bool isEnabled,
      string name,
      Uri value,
      string label,
      string revision,
      string version
      )
    {
      IsEnabled = isEnabled;
      Name = name;
      Value = value;
      Label = label;
      Revision = revision;
      Version = version;
    }

    public override string ToString()
    {
      return $"{Name} {Version} rev. {Revision}" +
        $"{(string.IsNullOrEmpty(Label) ? string.Empty : $" ({Label})")}" +
        $"{(IsEnabled ? string.Empty : " - you already have it")}";
    }
  }
}