using System.Windows;
using System.Windows.Controls;

namespace SIM.Tool.Base.UserControls
{
  /// <summary>
  /// Interaction logic for OKCancelButtons.xaml
  /// </summary>
  public partial class OKCancelButtons : UserControl
  {
    public static readonly DependencyProperty IsOKEnabledProperty = DependencyProperty.Register("IsOKEnabled", typeof (bool), typeof (OKCancelButtons), new PropertyMetadata(default(bool)));

    public OKCancelButtons()
    {
      InitializeComponent();
    }

    public event RoutedEventHandler OnOK;

    public event RoutedEventHandler OnCancel;

    public DependencyProperty IsOKEnabled
    {
      get { return (bool) this.OKButton.GetValue(IsEnabledProperty); }
      set { this.OKButton.SetValue(IsOKEnabledProperty, value); }
    }

    private void OKClick(object sender, RoutedEventArgs e)
    {
      if (OnOK != null)
      {
        OnOK(sender, e);
      }
    }

    private void CancelClick(object sender, RoutedEventArgs e)
    {
      if (OnCancel != null)
      {
        OnCancel(sender, e);
      }
    }
  }
}
