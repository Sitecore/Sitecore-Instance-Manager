using Microsoft.Web.Administration;
using Newtonsoft.Json.Linq;
using SIM.SitecoreEnvironments;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using Sitecore.Diagnostics.Base;
using Sitecore.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using TaskDialogInterop;

namespace SIM.Tool.Windows.UserControls.Resources
{
  public partial class Search : IWizardStep, IFlowControl, ICustomButton
  {
    private Window owner;
    private string resourcesFileName;
    private List<string> resources;

    public Search()
    {
      InitializeComponent();
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      ResourcesListBox.ItemsSource = null;
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      ResourcesWizardArgs args = (ResourcesWizardArgs)wizardArgs;
      if (MessageBox.Show("Are you sure you want to delete the selected resources?", "Delete resoruces", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        return true;
      }

      return false;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      ResourcesWizardArgs args = (ResourcesWizardArgs)wizardArgs;
      owner = args.WizardWindow;
      resourcesFileName = $"resources-{args.InstanceName}";
      resources = new List<string>();
      Caption.Text = "Searching resources in progress.";

      TaskDialogResult result = WindowHelper.LongRunningTask(() => SearchResources(args.InstanceName, args.ConnectionString, args.SolrUrl), 
        "Searching resources", owner);
      if (result == null)
      {
        this.Caption.Text = "Searching resources aborted by user.";
      }
    }

    private void SearchResources(string searchTerm, string connectionString, string solrUrl)
    {
      Dispatcher.BeginInvoke(new Action(() =>
      {
        this.Caption.Text = "Results:";
        resources.Add("Root Folders:");
        resources.AddRange(GetRootFolders(searchTerm));
        resources.Add("\r\nIIS App Pools:");
        resources.AddRange(GetAppPools(searchTerm));
        resources.Add("\r\nIIS Sites:");
        resources.AddRange(GetSites(searchTerm));
        resources.Add("\r\nHosts File:");
        resources.AddRange(GetHostsFileEntries(searchTerm));
        resources.Add("\r\nSQL Databases:");
        resources.AddRange(GetDatabases(searchTerm, connectionString));
        resources.Add("\r\nSolr Cores:");
        resources.AddRange(GetSolrCores(searchTerm, solrUrl));
        resources.Add("\r\nWindows Services:");
        resources.AddRange(GetServices(searchTerm));
        resources.Add($"\r\nEnvironments defined in '{SitecoreEnvironmentHelper.FilePath}':");
        resources.AddRange(GetSitecoreEnvironments(searchTerm));
        resources.Add($"\r\nUninstall Params Folders:");
        resources.AddRange(GetUninstallParamsFolders(searchTerm));
        ResourcesListBox.ItemsSource = resources;
      }), DispatcherPriority.Background).Wait();
    }

    private IEnumerable<string> GetRootFolders(string searchTerm)
    {
      List<string> rootFolders = new List<string>();
      SearchFolders(ProfileManager.Profile.InstancesFolder, searchTerm, ref rootFolders);
      return rootFolders;
    }

    private void SearchFolders(string startPath, string searchTerm, ref List<string> rootFolders)
    {
      try
      {
        string[] directories = Directory.GetDirectories(startPath);

        foreach (string directory in directories)
        {
          string folderName = System.IO.Path.GetFileName(directory);
          if (folderName.IndexOf(searchTerm, StringComparison.InvariantCultureIgnoreCase) > -1)
          {
            rootFolders.Add(directory);
            SearchFolders(directory, searchTerm, ref rootFolders);
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, ex.Message);
        rootFolders.Add(ex.Message);
      }
    }

    private IEnumerable<string> GetAppPools(string searchTerm)
    {
      using (ServerManager serverManager = new ServerManager())
      {
        ApplicationPoolCollection appPools = serverManager.ApplicationPools;
        foreach (ApplicationPool appPool in appPools)
        {
          if (appPool.Name.IndexOf(searchTerm, StringComparison.InvariantCultureIgnoreCase) > -1)
          {
            yield return appPool.Name;
          }
        }
      }
    }

    private IEnumerable<string> GetSites(string searchTerm)
    {
      using (ServerManager serverManager = new ServerManager())
      {
        SiteCollection sites = serverManager.Sites;
        foreach (Site site in sites)
        {
          if (site.Name.IndexOf(searchTerm, StringComparison.InvariantCultureIgnoreCase) > -1)
          {
            yield return site.Name;
          }
        }
      }
    }

    private IEnumerable<string> GetHostsFileEntries(string searchTerm)
    {
      string pattern = $@".*{searchTerm}.*";
      string hostsFilePath = Environment.GetFolderPath(Environment.SpecialFolder.System);
      hostsFilePath = Path.Combine(hostsFilePath, @"drivers\etc\hosts");
      List<string> hostsLines = new List<string>();

      try
      {
        string[] lines = File.ReadAllLines(hostsFilePath);

        foreach (string line in lines)
        {
          if (Regex.IsMatch(line, pattern))
          {
            hostsLines.Add(line);
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, ex.Message);
        hostsLines.Add(ex.Message);
      }

      return hostsLines;
    }

    private IEnumerable<string> GetDatabases(string searchTerm, string connectionString)
    {
      List<string> databases = new List<string>();

      try
      {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
          connection.Open();
          SqlCommand cmd = connection.CreateCommand();
          cmd.CommandText = "SELECT [name] FROM master.dbo.sysdatabases WHERE [name] LIKE @name";
          cmd.Parameters.AddWithValue("@name", searchTerm + "_%");
          SqlDataReader reader = cmd.ExecuteReader();
          while (reader.Read())
          {
            databases.Add(reader["name"].ToString());
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, ex.Message);
        databases.Add(ex.Message);
      }

      return databases;
    }

    private IEnumerable<string> GetSolrCores(string searchTerm, string solrUrl)
    {
      List<string> solrCores = new List<string>();
      SolrStateResolver solrStateResolver = new SolrStateResolver();

      if (solrStateResolver.GetUrlState(solrUrl) == SolrState.CurrentState.Running)
      {
        string coresUrl = $"{solrUrl}/admin/cores?action=STATUS&wt=json";
        try
        {
          using (HttpClient client = new HttpClient())
          {
            HttpResponseMessage response = client.GetAsync(coresUrl).Result;
            if (response.IsSuccessStatusCode)
            {
              string json = response.Content.ReadAsStringAsync().Result;
              JObject coresData = JObject.Parse(json);

              foreach (JProperty coreProperty in coresData["status"])
              {
                if (coreProperty.Name.IndexOf(searchTerm, StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                  solrCores.Add(coreProperty.Name);
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          Log.Error(ex, ex.Message);
          solrCores.Add(ex.Message);
        }
      }
      else
      {
        solrCores.Add($"The '{solrUrl}' URL is not accessible.");
      }

      return solrCores;
    }

    private IEnumerable<string> GetServices(string searchTerm)
    {
      ServiceController[] services = ServiceController.GetServices();

      foreach (ServiceController service in services)
      {
        if (service.ServiceName.IndexOf(searchTerm, StringComparison.InvariantCultureIgnoreCase) > -1)
        {
          yield return service.ServiceName;
        }
      }
    }

    private IEnumerable<string> GetSitecoreEnvironments(string searchTerm)
    {
      foreach (SitecoreEnvironment sitecoreEnvironment in SitecoreEnvironmentHelper.GetSitecoreEnvironmentsBySearchTerm(searchTerm))
      {
        yield return sitecoreEnvironment.Name;
      }
    }

    private IEnumerable<string> GetUninstallParamsFolders(string searchTerm)
    {
      foreach (string uninstallParamsFolder in Directory.GetDirectories(ApplicationManager.UnInstallParamsFolder))
      {
        if (uninstallParamsFolder.IndexOf(searchTerm, StringComparison.InvariantCultureIgnoreCase) > -1)
        {
          yield return uninstallParamsFolder;
        }
      }
    }

    public string CustomButtonText => "Save results";

    public void CustomButtonClick()
    {
      try
      {
        System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
        saveFileDialog.FileName = resourcesFileName;
        saveFileDialog.Filter = "Text files (*.txt)|*.txt";
        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          string filePath = saveFileDialog.FileName;
          File.WriteAllLines(filePath, resources);
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, ex.Message);
      }
    }
  }
}