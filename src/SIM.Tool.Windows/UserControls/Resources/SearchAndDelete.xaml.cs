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
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using TaskDialogInterop;

namespace SIM.Tool.Windows.UserControls.Resources
{
  public partial class SearchAndDelete : IWizardStep, IFlowControl, ICustomButton
  {
    private Window owner;
    private string resourcesFileName;
    private string InstanceName;
    private string SqlConnectionString;
    private string SolrUrl;
    private string SolrRoot;
    private Dictionary<string, IEnumerable<string>> foundResources;
    private Dictionary<string, IEnumerable<string>> deletedResources;
    private List<string> ResourcesToDelete;
    private const string SearchingResourcesWindowTitle = "Searching resources";
    private const string CertificateDelimiter = " - ";
    private const string CertificatesCurrentUserMy = "Certificates (CurrentUser/My)";
    private const string CertificatesCurrentUserRoot = "Certificates (CurrentUser/Root)";
    private const string CertificatesLocalMachineMy = "Certificates (LocalMachine/My)";
    private const string CertificatesLocalMachineRoot = "Certificates (LocalMachine/Root)";
    private const string Sites = "IIS Sites";
    private const string AppPools = "IIS App Pools";
    private const string Hosts = "Lines in hosts";
    private const string WindowsServices = "Windows Services";
    private const string Databases = "SQL Databases";
    private const string SolrCores = "Solr Cores";
    private const string SolrCoresFolders = "Solr Cores Folders";
    private const string RootFolders = "Root Folders";
    private const string Environments = "Data in Environments.json";
    private const string UninstallParamsFolders = "Uninstall Params Folders";

    public SearchAndDelete()
    {
      InitializeComponent();
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      ClearResourcesViews();
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      if (!foundResources.Keys.Any())
      {
        WindowHelper.ShowMessage($"No resources to delete were found using the '{InstanceName}' search term.",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);
        return false;
      }

      if (WindowHelper.ShowMessage("The following types of resources are going to be deleted:\n" +
        $"{string.Join("\n", foundResources.Keys)}\n\n" +
        "Do you want to proceed?",
        messageBoxImage: MessageBoxImage.Question,
        messageBoxButton: MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        Dictionary<string, IEnumerable<string>> resourcesToDelete = foundResources;
        foreach (var resource in resourcesToDelete)
        {
          WindowHelper.LongRunningTask(() => DeleteResourcesBasedOnType(resource.Key, resource.Value), $"Deleting {resource.Key}", owner);
        }

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
      InstanceName = args.InstanceName;
      SqlConnectionString = args.ConnectionString;
      SolrUrl = args.SolrUrl;
      SolrRoot = args.SolrRoot;
      resourcesFileName = $"resources-{args.InstanceName}";
      foundResources = new Dictionary<string, IEnumerable<string>>();
      deletedResources = new Dictionary<string, IEnumerable<string>>();

      SaveToFile.Visibility = Visibility.Hidden;
      ResourcesListBox.Visibility = Visibility.Hidden;
      CaptionColumnDefinition.Width = new GridLength(200);
      Caption.Text = $"{SearchingResourcesWindowTitle} in progress.";

      TaskDialogResult result = WindowHelper.LongRunningTask(() => SearchResources(InstanceName, SqlConnectionString, SolrUrl, SolrRoot),
        SearchingResourcesWindowTitle, owner);
      if (result == null)
      {
        Caption.Text = $"{SearchingResourcesWindowTitle} aborted by user.";
      }
    }

    private void SearchResources(string searchTerm, string connectionString, string solrUrl, string solrRoot)
    {
      Dispatcher.BeginInvoke(new Action(() =>
      {
        InitializeResources(CertificatesCurrentUserMy, GetCertificates(searchTerm, StoreName.My, StoreLocation.CurrentUser));
        InitializeResources(CertificatesCurrentUserRoot, GetCertificates(searchTerm, StoreName.Root, StoreLocation.CurrentUser));
        InitializeResources(CertificatesLocalMachineMy, GetCertificates(searchTerm, StoreName.My, StoreLocation.LocalMachine));
        InitializeResources(CertificatesLocalMachineRoot, GetCertificates(searchTerm, StoreName.Root, StoreLocation.LocalMachine));
        InitializeResources(Sites, GetSites(searchTerm));
        InitializeResources(AppPools, GetAppPools(searchTerm));
        InitializeResources(Hosts, GetHostsFileEntries(searchTerm));
        InitializeResources(WindowsServices, GetServices(searchTerm));
        InitializeResources(Databases, GetDatabases(searchTerm, connectionString));
        InitializeResources(SolrCores, GetSolrCores(searchTerm, solrUrl));
        InitializeResources(SolrCoresFolders, GetSolrCoresFolders(searchTerm, solrRoot));
        InitializeResources(RootFolders, GetRootFolders(searchTerm));
        InitializeResources(Environments, GetSitecoreEnvironments(searchTerm));
        InitializeResources(UninstallParamsFolders, GetUninstallParamsFolders(searchTerm));

        foreach (KeyValuePair<string, IEnumerable<string>> resource in foundResources)
        {
          ResourcesComboBox.Items.Add(resource.Key);
        }

        if (ResourcesComboBox.Items.Count > 0)
        {
          SetResourcesFoundViews();
        }
        else
        {
          SetResourcesNotFoundViews();
        }
      }), DispatcherPriority.Background).Wait();
    }

    private void InitializeResources(string resourceType, IEnumerable<string> resources)
    {
      if (resources != null && resources.Any())
      {
        foundResources.Add(resourceType, resources);
      }
    }

    private IEnumerable<string> GetCertificates(string searchTerm, StoreName storeName, StoreLocation storeLocation)
    {
      List<string> foundCertificates = new List<string>();

      X509Store store = new X509Store(storeName, storeLocation);

      try
      {
        store.Open(OpenFlags.ReadOnly);

        X509Certificate2Collection certificates = store.Certificates.Find(X509FindType.FindBySubjectName, searchTerm, false);

        foreach (X509Certificate2 certificate in certificates)
        {
          foundCertificates.Add(certificate.Subject + CertificateDelimiter + certificate.Thumbprint);
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, ex.Message);
        WindowHelper.ShowMessage($"Unable to get certificates from the '{storeLocation}/{storeName}' store due to the following error:\n{ex.Message}",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);
      }
      finally
      {
        store.Close();
      }

      return foundCertificates;
    }

    private IEnumerable<string> DeleteCertificates(IEnumerable<string> certificates, StoreName storeName, StoreLocation storeLocation)
    {
      List<string> deletedCertificates = new List<string>();

      X509Store store = new X509Store(storeName, storeLocation);

      foreach (string certificate in certificates)
      {
        int thumbprintPosition = certificate.IndexOf(CertificateDelimiter);

        if (thumbprintPosition != -1)
        {
          string thumbprint = certificate.Substring(thumbprintPosition + CertificateDelimiter.Length);

          if (!string.IsNullOrEmpty(thumbprint))
          {
            try
            {
              store.Open(OpenFlags.ReadWrite);

              X509Certificate2Collection certificatesToDelete = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

              foreach (X509Certificate2 certificateToDelete in certificatesToDelete)
              {
                store.Remove(certificateToDelete);
                deletedCertificates.Add(certificate);
              }
            }
            catch (Exception ex)
            {
              Log.Error(ex, ex.Message);
              WindowHelper.ShowMessage($"Unable to delete the '{certificate}' certificate from the '{storeLocation}/{storeName}' store due to the following error:\n{ex.Message}",
                messageBoxImage: MessageBoxImage.Warning,
                messageBoxButton: MessageBoxButton.OK);
            }
            finally
            {
              store.Close();
            }
          }
        }
      }

      return deletedCertificates;
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

    private IEnumerable<string> DeleteSites(IEnumerable<string> sites)
    {
      List<string> deletedSites = new List<string>();

      using (ServerManager serverManager = new ServerManager())
      {
        foreach (string site in sites)
        {
          try
          {
            Site siteToDelete = serverManager.Sites[site];
            serverManager.Sites.Remove(siteToDelete);
            serverManager.CommitChanges();
            deletedSites.Add(site);
          }
          catch (Exception ex)
          {
            Log.Error(ex, ex.Message);
            WindowHelper.ShowMessage($"Unable to delete the '{site}' site due to the following error:\n{ex.Message}",
              messageBoxImage: MessageBoxImage.Warning,
              messageBoxButton: MessageBoxButton.OK);
          }
        }
      }

      return deletedSites;
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

    private IEnumerable<string> DeleteAppPools(IEnumerable<string> appPools)
    {
      List<string> deletedAppPools = new List<string>();

      using (ServerManager serverManager = new ServerManager())
      {
        foreach (string appPool in appPools)
        {
          try
          {
            ApplicationPool appPoolToDelete = serverManager.ApplicationPools[appPool];
            serverManager.ApplicationPools.Remove(appPoolToDelete);
            serverManager.CommitChanges();
            deletedAppPools.Add(appPool);
          }
          catch (Exception ex)
          {
            Log.Error(ex, ex.Message);
            WindowHelper.ShowMessage($"Unable to delete the '{appPool}' App Pool due to the following error:\n{ex.Message}",
              messageBoxImage: MessageBoxImage.Warning,
              messageBoxButton: MessageBoxButton.OK);
          }
        }
      }

      return deletedAppPools;
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
        WindowHelper.ShowMessage($"Unable to get info from the hosts file due to the following error:\n{ex.Message}",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);
      }

      return hostsLines;
    }

    private IEnumerable<string> DeleteHostsFileEntries(IEnumerable<string> entries)
    {
      string hostsFilePath = Environment.GetFolderPath(Environment.SpecialFolder.System);
      hostsFilePath = Path.Combine(hostsFilePath, @"drivers\etc\hosts");
      List<string> deletedHostsLines = new List<string>();

      try
      {
        string[] lines = File.ReadAllLines(hostsFilePath);
        StringBuilder updatedContent = new StringBuilder();

        foreach (string line in lines)
        {
          bool removeEntry = false;
          foreach (string entry in entries)
          {
            if (line.Contains(entry))
            {
              deletedHostsLines.Add(entry);
              removeEntry = true;
              break;
            }
          }

          if (!removeEntry)
          {
            updatedContent.AppendLine(line);
          }
        }

        File.WriteAllText(hostsFilePath, updatedContent.ToString());
      }
      catch (Exception ex)
      {
        Log.Error(ex, ex.Message);
        WindowHelper.ShowMessage($"The following error occurred while deleting lines in the hosts file:\n{ex.Message}",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);
      }

      return deletedHostsLines;
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

    private IEnumerable<string> DeleteServices(IEnumerable<string> services)
    {
      List<string> deletedServices = new List<string>();

      foreach (string service in services)
      {
        try
        {
          ServiceController serviceController = new ServiceController(service);

          if (serviceController.Status == ServiceControllerStatus.Running)
          {
            serviceController.Stop();
            serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
          }

          ServiceInstaller serviceInstaller = new ServiceInstaller();
          serviceInstaller.Context = new System.Configuration.Install.InstallContext();
          serviceInstaller.ServiceName = serviceController.ServiceName;
          serviceInstaller.Uninstall(null);

          deletedServices.Add(service);
        }
        catch (Exception ex)
        {
          Log.Error(ex, ex.Message);
          WindowHelper.ShowMessage($"The following error occurred while deleting the '{service}' service:\n{ex.Message}",
            messageBoxImage: MessageBoxImage.Warning,
            messageBoxButton: MessageBoxButton.OK);
        }
      }

      return deletedServices;
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
        WindowHelper.ShowMessage($"Unable to get info about databases due to the following error:\n{ex.Message}",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);
      }

      return databases;
    }

    private IEnumerable<string> DeleteDatabases(IEnumerable<string> databases, string connectionString)
    {
      List<string> deletedDatabases = new List<string>();

      try
      {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
          connection.Open();

          foreach (string database in databases)
          {
            string dropDatabaseCommand = $"DROP DATABASE [{database}]";

            using (SqlCommand command = new SqlCommand(dropDatabaseCommand, connection))
            {
              command.ExecuteNonQuery();
            }

            deletedDatabases.Add(database);
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, ex.Message);
        WindowHelper.ShowMessage($"The following error occurred while deleting databases:\n{ex.Message}",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);
      }

      return deletedDatabases;
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
          WindowHelper.ShowMessage($"Unable to get info about Solr cores due to the following error:\n{ex.Message}",
            messageBoxImage: MessageBoxImage.Warning,
            messageBoxButton: MessageBoxButton.OK);
        }
      }
      else
      {
        WindowHelper.ShowMessage($"Unable to get info about Solr cores because the '{solrUrl}' URL is not accessible.",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);
      }

      return solrCores;
    }

    private IEnumerable<string> DeleteSolrCores(IEnumerable<string> solrCores, string solrUrl)
    {
      List<string> deletedSolrCores = new List<string>();
      SolrStateResolver solrStateResolver = new SolrStateResolver();

      if (solrStateResolver.GetUrlState(solrUrl) == SolrState.CurrentState.Running)
      {
        try
        {
          using (HttpClient client = new HttpClient())
          {
            foreach(string solrCore in solrCores)
            {
              string unloadCoreUrl = $"{solrUrl}/admin/cores?action=unload&core={solrCore}";
              HttpResponseMessage response = client.PostAsync(unloadCoreUrl, null).Result;

              if (response.IsSuccessStatusCode)
              {
                deletedSolrCores.Add(solrCore);
              }
              else
              {
                WindowHelper.ShowMessage($"Unable to delete the '{solrCore}' Solr core due to the following error:\n{response.StatusCode} - {response.ReasonPhrase}",
                  messageBoxImage: MessageBoxImage.Warning,
                  messageBoxButton: MessageBoxButton.OK);
              }
            }
          }
        }
        catch (Exception ex)
        {
          Log.Error(ex, ex.Message);
          WindowHelper.ShowMessage($"The following error occurred while deleting Solr cores:\n{ex.Message}",
            messageBoxImage: MessageBoxImage.Warning,
            messageBoxButton: MessageBoxButton.OK);
        }
      }
      else
      {
        WindowHelper.ShowMessage($"Unable to delete the following Solr cores because the '{solrUrl}' URL is not accessible:\n{string.Join("\n", solrCores)}",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);
      }

      return deletedSolrCores;
    }

    private IEnumerable<string> GetSolrCoresFolders(string searchTerm, string solrRoot)
    {
      if (!string.IsNullOrEmpty(solrRoot))
      {
        solrRoot = Path.Combine(solrRoot, @"server\solr");
        if (Directory.Exists(solrRoot))
        {
          foreach (string solrCoresFolder in Directory.GetDirectories(solrRoot))
          {
            if (solrCoresFolder.IndexOf(searchTerm, StringComparison.InvariantCultureIgnoreCase) > -1)
            {
              yield return solrCoresFolder;
            }
          }
        }
        else
        {
          WindowHelper.ShowMessage($"Unable to get info about Solr cores folders because the '{solrRoot}' directory does not exist.",
            messageBoxImage: MessageBoxImage.Warning,
            messageBoxButton: MessageBoxButton.OK);
        }
      }
      else
      {
        WindowHelper.ShowMessage($"Unable to get info about Solr cores folders because the Solr root is invalid.",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);
      }
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
          string folderName = Path.GetFileName(directory);
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
        WindowHelper.ShowMessage($"Unable to get info about root folders due to the following error:\n{ex.Message}",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);
      }
    }

    private IEnumerable<string> DeleteFolders(IEnumerable<string> folders)
    {
      List<string> deletedFolders = new List<string>();

      foreach (string folder in folders)
      {
        try
        {
          if (Directory.Exists(folder))
          {
            Directory.Delete(folder, true);
          }
          deletedFolders.Add(folder);
        }
        catch (Exception ex)
        {
          Log.Error(ex, ex.Message);
          WindowHelper.ShowMessage($"Unable to delete the '{folder}' folder due to the following error:\n{ex.Message}",
            messageBoxImage: MessageBoxImage.Warning,
            messageBoxButton: MessageBoxButton.OK);
        }
      }

      return deletedFolders;
    }

    private IEnumerable<string> GetSitecoreEnvironments(string searchTerm)
    {
      List<string> environments = new List<string>();

      foreach (SitecoreEnvironment sitecoreEnvironment in SitecoreEnvironmentHelper.GetSitecoreEnvironmentsBySearchTerm(searchTerm))
      {
        environments.Add(sitecoreEnvironment.Name);
      }

      return environments;
    }

    private IEnumerable<string> DeleteSitecoreEnvironments(IEnumerable<string> environments)
    {
      List<string> deletedEnvironments = new List<string>();

      foreach (string environment in environments)
      {
        SitecoreEnvironment environmentToDelete = SitecoreEnvironmentHelper.GetEnvironmentByName(environment);
        if (environmentToDelete != null)
        {
          SitecoreEnvironmentHelper.SitecoreEnvironments.Remove(environmentToDelete);
          SitecoreEnvironmentHelper.SaveSitecoreEnvironmentData(SitecoreEnvironmentHelper.SitecoreEnvironments);
          deletedEnvironments.Add(environment);
        }
        else
        {
          WindowHelper.ShowMessage($"Unable to find the '{environment}' environment in Environments.json for deletion.",
            messageBoxImage: MessageBoxImage.Warning,
            messageBoxButton: MessageBoxButton.OK);
        }
      }

      return deletedEnvironments;
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

    private void ResourcesComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (ResourcesComboBox.Items.Count > 0 && ResourcesComboBox.SelectedValue != null)
      {
        string selectedValue = ResourcesComboBox.SelectedValue.ToString();
        if (foundResources.ContainsKey(selectedValue))
        {
          ResourcesListBox.ItemsSource = foundResources[selectedValue];
          ResourcesToDelete = new List<string>();
        }
      }
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
      var resource = ((System.Windows.Controls.CheckBox)sender).Content.ToString();
      if (!string.IsNullOrEmpty(resource))
      {
        ResourcesToDelete.Add(resource);
      }
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
      var resource = ((System.Windows.Controls.CheckBox)sender).Content.ToString();
      if (!string.IsNullOrEmpty(resource))
      {
        ResourcesToDelete.Remove(resource);
      }
    }

    private void SaveToFile_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
        saveFileDialog.FileName = resourcesFileName;
        saveFileDialog.Filter = "Text files (*.txt)|*.txt";
        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          string filePath = saveFileDialog.FileName;
          List<string> contents = new List<string>();
          foreach (KeyValuePair<string, IEnumerable<string>> resource in foundResources)
          {
            if (resource.Value != null && resource.Value.ToList().Count > 0)
            {
              contents.Add(resource.Key + ":");
              contents.AddRange(resource.Value);
              contents.Add(string.Empty);
            }
          }
          File.WriteAllLines(filePath, contents);
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, ex.Message);
        WindowHelper.ShowMessage($"Unable to save info to file due to the following error:\n{ex.Message}",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);
      }
    }

    public string CustomButtonText => "Delete selected";

    public void CustomButtonClick()
    {
      if (!ResourcesToDelete.Any())
      {
        WindowHelper.ShowMessage("You haven't selected any of resources to delete.",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK);
        return;
      }

      string resourceType = ResourcesComboBox.SelectedValue.ToString();

      if (WindowHelper.ShowMessage($"Are you sure you want to delete the selected resources related to '{resourceType}'?",
        messageBoxImage: MessageBoxImage.Question,
        messageBoxButton: MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        WindowHelper.LongRunningTask(() => UpdateResourcesLists(resourceType, DeleteResourcesBasedOnType(resourceType, ResourcesToDelete)),
          $"Deleting {resourceType}", owner);
        UpdateResourcesViews(resourceType);
      }
    }

    private IEnumerable<string> DeleteResourcesBasedOnType(string resourceType, IEnumerable<string> resources)
    {
      switch (resourceType)
      {
        case CertificatesCurrentUserMy:
          return DeleteCertificates(resources, StoreName.My, StoreLocation.CurrentUser);
        case CertificatesCurrentUserRoot:
          return DeleteCertificates(resources, StoreName.Root, StoreLocation.CurrentUser);
        case CertificatesLocalMachineMy:
          return DeleteCertificates(resources, StoreName.My, StoreLocation.LocalMachine);
        case CertificatesLocalMachineRoot:
          return DeleteCertificates(resources, StoreName.Root, StoreLocation.LocalMachine);
        case Sites:
          return DeleteSites(resources);
        case AppPools:
          return DeleteAppPools(resources);
        case Hosts:
          return DeleteHostsFileEntries(resources);
        case WindowsServices:
          return DeleteServices(resources);
        case Databases:
          return DeleteDatabases(resources, SqlConnectionString);
        case SolrCores:
          return DeleteSolrCores(resources, SolrUrl);
        case SolrCoresFolders:
        case RootFolders:
        case UninstallParamsFolders:
          return DeleteFolders(resources);
        case Environments:
          return DeleteSitecoreEnvironments(resources);
        default:
          WindowHelper.ShowMessage("No resources were deleted.",
            messageBoxImage: MessageBoxImage.Warning,
            messageBoxButton: MessageBoxButton.OK);
          return null;
      }
    }

    private void UpdateResourcesLists(string resourceType, IEnumerable<string> resources)
    {
      if (resources != null && resources.Any())
      {
        if (deletedResources.ContainsKey(resourceType))
        {
          List<string> tempDeletedResources = deletedResources[resourceType].ToList();
          tempDeletedResources.AddRange(resources);
          deletedResources[resourceType] = tempDeletedResources;
        }
        else
        {
          deletedResources.Add(resourceType, resources);
        }

        IEnumerable<string> tempFoundResources = foundResources[resourceType].ToList().Except(resources);
        if (tempFoundResources.Any())
        {
          foundResources[resourceType] = tempFoundResources;
        }
        else
        {
          foundResources.Remove(resourceType);
        }

        ResourcesToDelete = new List<string>();
      }
    }

    private void UpdateResourcesViews(string resourceType)
    {
      if (foundResources.ContainsKey(resourceType))
      {
        ResourcesListBox.ItemsSource = foundResources[resourceType];
      }
      else if (ResourcesComboBox.Items.Contains(resourceType))
      {
        ResourcesComboBox.Items.Remove(resourceType);
        if (ResourcesComboBox.Items.Count < 1)
        {
          ClearResourcesViews();
          SetResourcesNotFoundViews();
        }
        else
        {
          ResourcesComboBox.SelectedIndex = 0;
        }
      }
    }

    private void ClearResourcesViews()
    {
      ResourcesListBox.ItemsSource = null;
      ResourcesListBox.Visibility = Visibility.Hidden;
      ResourcesComboBox.Items.Clear();
      ResourcesComboBox.Visibility = Visibility.Hidden;
      SaveToFile.Visibility = Visibility.Hidden;
    }

    private void SetResourcesFoundViews()
    {
      ResourcesToDelete = new List<string>();
      CaptionColumnDefinition.Width = new GridLength(100);
      Caption.Text = "Found resources:";
      ResourcesComboBox.Visibility = Visibility.Visible;
      ResourcesComboBox.SelectedIndex = 0;
      ResourcesListBox.Visibility = Visibility.Visible;
      SaveToFile.Visibility = Visibility.Visible;
    }

    private void SetResourcesNotFoundViews()
    {
      ResourcesToDelete = new List<string>();
      CaptionColumnDefinition.Width = new GridLength(200);
      Caption.Text = $"No resources were found.";
    }
  }
}