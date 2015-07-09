using System.Windows;

namespace SIM.Tool.Windows.Dialogs
{
  public partial class DatabasesOperationsDialog : Window
  {
    #region Constructors

    public DatabasesOperationsDialog()
    {
      this.InitializeComponent();
    }

    public DatabasesOperationsDialog(string name)
    {
      this.InitializeComponent();
      this.dbName.Text = name;
    }

    public DatabasesOperationsDialog(string name, string path)
    {
      this.InitializeComponent();
      this.dbName.Text = name;
      this.dbPath.Text = path;
    }

    #endregion

    #region Private methods

    private void btnOk_Click(object sender, RoutedEventArgs e)
    {
      // OpenFileDialog
    }

    #endregion
  }
}