using System.Windows;

namespace SIM.Tool.Windows.Dialogs
{
  public partial class DatabasesOperationsDialog : Window
  {
    #region Constructors

    public DatabasesOperationsDialog()
    {
      InitializeComponent();
    }

    public DatabasesOperationsDialog(string name)
    {
      InitializeComponent();
      dbName.Text = name;
    }

    public DatabasesOperationsDialog(string name, string path)
    {
      InitializeComponent();
      dbName.Text = name;
      dbPath.Text = path;
    }

    #endregion

    #region Private methods

    private void BtnOkClick(object sender, RoutedEventArgs e)
    {
      // OpenFileDialog
    }

    #endregion
  }
}