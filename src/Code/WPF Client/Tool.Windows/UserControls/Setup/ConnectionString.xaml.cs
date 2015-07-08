using SIM.Base;
using SIM.Tool.Base;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Windows.Dialogs;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using SIM.Tool.Windows.Pipelines.Setup;

namespace SIM.Tool.Windows.UserControls.Setup
{
  /// <summary>
  /// Interaction logic for ConnectionString.xaml
  /// </summary>
  public partial class ConnectionString : IWizardStep, IFlowControl
  {
    public ConnectionString()
    {
      InitializeComponent();
    }

    private void PickConnectionString([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      var result = WindowHelper.ShowDialog(new ConnectionStringDialog() { DataContext = this.ConnectionStringTextBox.Text }, (Window)(((Grid)((TabControl)((TabItem)this.Parent).Parent).Parent)).Parent);
      this.ConnectionStringTextBox.Text = (string) result;
    }

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      this.ConnectionStringTextBox.Text = args.ConnectionString;
    }

    bool IFlowControl.OnMovingNext(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      SqlConnection connection = null;
      try
      {
        connection = SIM.Adapters.SqlServer.SqlServerManager.Instance.OpenConnection(new SqlConnectionStringBuilder(args.ConnectionString), true);
        connection.Close();
        return true;
      }
      catch (SqlException ex)
      {
        WindowHelper.HandleError(ex.Message, false, ex, this);
        return false;
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError(ex.Message, true, ex, this);
        return false;
      }
      finally
      {
        if (connection != null)
        {
          connection.Close();
        }
      }
    }

    bool IFlowControl.OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      args.ConnectionString = this.ConnectionStringTextBox.Text;
      return true;
    }
  }
}
