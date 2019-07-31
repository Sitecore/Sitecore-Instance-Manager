﻿namespace SIM.Tool.Windows.UserControls.Setup
{
  using System;
  using System.Data.SqlClient;
  using System.Windows;
  using System.Windows.Controls;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.Dialogs;
  using SIM.Tool.Windows.Pipelines.Setup;
  using JetBrains.Annotations;

  public partial class ConnectionString : IWizardStep, IFlowControl
  {
    #region Constructors

    public ConnectionString()
    {
      InitializeComponent();
    }

    #endregion

    #region Private methods

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      ConnectionStringTextBox.Text = args.ConnectionString;
    }

    bool IFlowControl.OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
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
        WindowHelper.HandleError("Failed to process move next", false, ex);
        return false;
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("Failed to process move next", true, ex);
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

    private void PickConnectionString([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      var result = WindowHelper.ShowDialog(new ConnectionStringDialog()
      {
        DataContext = ConnectionStringTextBox.Text
      }, (Window)((Grid)((TabControl)((TabItem)Parent).Parent).Parent).Parent);
      ConnectionStringTextBox.Text = (string)result;
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      var args = (SetupWizardArgs)wizardArgs;
      args.ConnectionString = ConnectionStringTextBox.Text;
      return true;
    }

    #endregion
  }
}