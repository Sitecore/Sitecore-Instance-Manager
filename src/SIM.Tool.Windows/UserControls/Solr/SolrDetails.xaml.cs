namespace SIM.Tool.Windows.UserControls.Install
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Wizards;
  using JetBrains.Annotations;
  using SIM.Sitecore9Installer;
  using SIM.Sitecore9Installer.Configuration;
  using SIM.Sitecore9Installer.Tasks;
  using SIM.Sitecore9Installer.Security;
  using System.Collections.Generic;

  [UsedImplicitly]
  public partial class SolrDetails : IWizardStep, IFlowControl
  {
    /// <summary>
    /// <para>Tasks for Solr installs</para>
    /// <para>The key corresponds to the minimum version supported for the task type.</para>
    /// <para>e.g. 9 will map to solr9</para>
    /// </summary>
    private readonly Dictionary<int, Tasker> taskers = new Dictionary<int, Tasker>();

    #region Constructors

    public SolrDetails()
    {
      InitializeComponent();

    }

    #endregion

    #region Public Methods

    public bool OnMovingBack(WizardArgs wizardArg)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      Tasker solrTasker = taskers[0];
      string solrVersion = solrVersionSelector.SelectedValue as string;
      if (Version.TryParse(solrVersion, out var v) && v.Major >= 10)
      {
        solrTasker = taskers[10];
        Task solrInstallTask = solrTasker.Tasks.Single(t => t.Name == "Solr");
        solrInstallTask.LocalParams.AddOrUpdateParam("SolrAdminUserName", "admin", InstallParamType.String);
        solrInstallTask.LocalParams.AddOrUpdateParam("SolrAdminPassword", SolrPasswordGenerator.Generate(), InstallParamType.String);
      }
      else if (v.Major >= 9)
      {
        solrTasker = taskers[9];
      }
      solrTasker.GlobalParams.First(p => p.Name == "SolrVersion").Value = solrVersion;
      solrTasker.GlobalParams.First(p => p.Name == "SolrDomain").Value = this.solrHostSelector.Text;

      if (!string.IsNullOrEmpty(this.solrPortSelector.Text))
      {
        solrTasker.GlobalParams.First(p => p.Name == "SolrPort").Value = this.solrPortSelector.Text;
      }

      if (!string.IsNullOrEmpty(this.solrFolderSelector.Text))
      {
        solrTasker.GlobalParams.First(p => p.Name == "SolrInstallRoot").Value = this.solrFolderSelector.Text;
      }

      string javaHomeVariable = Environment.GetEnvironmentVariable("JAVA_HOME");
      if (string.IsNullOrEmpty(javaHomeVariable))
      {
        MessageBox.Show("Required JAVA_HOME system variable was not found.\nIt seems Java Runtime Environment (JRE) has not been installed.", "Warning");
        return false;
      }
      else if (!Directory.Exists(Path.Combine(javaHomeVariable, "bin")) || Directory.GetFiles(Path.Combine(javaHomeVariable, "bin"), "java.exe").Length == 0)
      {
        MessageBox.Show($"The 'JAVA_HOME' system variable does not point to the proper JRE folder: {javaHomeVariable}", "Warning");
        return false;
      }

      solrTasker.GlobalParams.First(p => p.Name == "JavaHome").Value = javaHomeVariable;
      Install9WizardArgs args = (Install9WizardArgs)wizardArgs;
      args.Tasker = solrTasker;
      return true;
    }
    #endregion

    #region Private Methods
    private void SolrVersionSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      string solrVersion = this.solrVersionSelector.SelectedValue as string;
      this.solrHostSelector.Text = "solr" + solrVersion.Replace(".", string.Empty);
    }

    private void LocationBtn_Click(object sender, RoutedEventArgs e)
    {
      WindowHelper.PickFolder("Choose location folder", this.solrFolderSelector, null);
    }

    private void FillSolrVersions()
    {
      this.solrVersionSelector.DataContext = Configuration.Instance.SolrMap.Keys;
    }
    #endregion
    #region IWizardStep Members

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      string root = Path.Combine(Directory.GetCurrentDirectory(), "CustomSifConfig/Solr");
      if (!this.taskers.ContainsKey(0)) this.taskers.Add(0, new Tasker(root, "solr", null));
      if (!this.taskers.ContainsKey(9)) this.taskers.Add(9, new Tasker(root, "solr9", null));
      if (!this.taskers.ContainsKey(10)) this.taskers.Add(10, new Tasker(root, "solr10", null));

      this.FillSolrVersions();
      this.solrPortSelector.Text = taskers[0].Tasks.First(t => t.Name == "Solr").LocalParams.First(p => p.Name == "SolrPort").Value;
      this.solrFolderSelector.Text = taskers[0].Tasks.First(t => t.Name == "Solr").LocalParams.First(p => p.Name == "SolrInstallRoot").Value;
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    #endregion

  }
}
