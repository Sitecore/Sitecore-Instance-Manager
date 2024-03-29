﻿using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Windows.Dialogs;
using Sitecore.Diagnostics.Base;
using Sitecore.Diagnostics.Logging;
using System.Windows;

namespace SIM.Tool.Windows.UserControls.Resources
{
  public partial class Details : IWizardStep, IFlowControl
  {
    private Window owner;

    public Details()
    {
      InitializeComponent();
    }

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      ResourcesWizardArgs args = (ResourcesWizardArgs)wizardArgs;
      owner = args.WizardWindow;
      using (new ProfileSection("Initializing Solrs", this))
      {
        Solrs.DataContext = ProfileManager.Profile.Solrs;
      }
      ConnectionStringTextBox.Text = ProfileManager.Profile.ConnectionString;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingBack(WizardArgs wizardArg)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      ResourcesWizardArgs args = (ResourcesWizardArgs)wizardArgs;

      if (string.IsNullOrEmpty(InstanceNameTextBox.Text))
      {
        WindowHelper.ShowMessage("Please provide site name.",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);
        return false;
      }
      args.InstanceName = InstanceNameTextBox.Text;

      if (ConnectionStringCheckBox.IsChecked == true)
      {
        if (string.IsNullOrEmpty(ConnectionStringTextBox.Text))
        {
          WindowHelper.ShowMessage("Please provide connection string.",
            messageBoxImage: MessageBoxImage.Warning,
            messageBoxButton: MessageBoxButton.OK);
          return false;
        }
        args.ConnectionString = ConnectionStringTextBox.Text;
      }
      else
      {
        args.ConnectionString = ProfileManager.Profile.ConnectionString;
      }

      SolrDefinition solr = Solrs.SelectedItem as SolrDefinition;
      if (solr == null)
      {
        WindowHelper.ShowMessage("Please provide Solr.",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);
        return false;
      }

      if (string.IsNullOrEmpty(solr.Url))
      {
        WindowHelper.ShowMessage($"Solr URL is not specified for the '{solr.Name}' service.",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);
        return false;
      }
      args.SolrUrl = solr.Url;

      if (string.IsNullOrEmpty(solr.Root))
      {
        WindowHelper.ShowMessage($"Solr root is not specified for the '{solr.Name}' service.",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);
        return false;
      }
      args.SolrRoot = solr.Root;

      return true;
    }

    private void ConnectionStringCheckBox_Checked(object sender, RoutedEventArgs e)
    {
      ConnectionStringRowDefinition.Height = new GridLength(28);
    }

    private void ConnectionStringCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
      ConnectionStringRowDefinition.Height = new GridLength(0);
    }

    private void AddSolr_Click(object sender, RoutedEventArgs e)
    {
      SolrDefinition solr = WindowHelper.ShowDialog<AddSolrDialog>(ProfileManager.Profile.Solrs, owner) as SolrDefinition;
      if (solr != null)
      {
        if (!ProfileManager.Profile.Solrs.Contains(solr))
        {
          ProfileManager.Profile.Solrs.Add(solr);
          ProfileManager.SaveChanges(ProfileManager.Profile);
        }

        Solrs.DataContext = null;
        Solrs.DataContext = ProfileManager.Profile.Solrs;
        Solrs.SelectedItem = solr;
      }
    }
  }
}