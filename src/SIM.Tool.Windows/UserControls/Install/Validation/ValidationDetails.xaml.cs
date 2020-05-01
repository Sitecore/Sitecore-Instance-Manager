using SIM.Sitecore9Installer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Tool.Windows.UserControls.Install.Validation
{
  /// <summary>
  /// Interaction logic for Install9ParametersEditor.xaml
  /// </summary>
  public partial class ValidationDetails : Window
  {
    public ValidationDetails()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      
    }

    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      ScrollViewer scrollviewer = sender as ScrollViewer;
      if (e.Delta > 0)
        scrollviewer.LineLeft();
      else
        scrollviewer.LineRight();
      e.Handled = true;
    }    

    private void Btn_Close_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
