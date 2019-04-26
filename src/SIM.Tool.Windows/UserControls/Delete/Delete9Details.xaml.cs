namespace SIM.Tool.Windows.UserControls.Install
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Xml;
  using SIM.Adapters.SqlServer;
  using SIM.Adapters.WebServer;
  using SIM.Instances;
  using SIM.Products;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;
  using SIM.IO.Real;
  using SIM.Sitecore9Installer;

  [UsedImplicitly]
  public partial class Delete9Details : IWizardStep, IFlowControl
  {
    #region Fields

    [NotNull]
    private readonly ICollection<string> _AllFrameworkVersions = Environment.Is64BitOperatingSystem ? new[]
    {
      "v2.0", "v2.0 32bit", "v4.0", "v4.0 32bit"
    } : new[]
    {
      "v2.0", "v4.0"
    };

    private InstallWizardArgs _InstallParameters = null;

    #endregion

    #region Constructors

    public Delete9Details()
    {
      InitializeComponent();

    }

    #endregion

    #region Public properties

    public static bool InstallEverywhere
    {
      get
      {
        return WindowsSettings.AppInstallEverywhere.Value;
      }
    }

    #endregion

    #region Public Methods

    public bool OnMovingBack(WizardArgs wizardArg)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      var args = (Delete9WizardArgs)wizardArgs;
      var product = args.Product;
      Assert.IsNotNull(product, nameof(product));

      if (string.IsNullOrWhiteSpace(this.solrRoot.Text))
      {
        this.Alert("Solr URL cannot be empty. Please provide a value.", new object[] { });
        return false;
      }

      if (string.IsNullOrWhiteSpace(this.solrRoot.Text))
      {
        this.Alert("Solr root cannot be empty. Please provide a value.", new object[] { });
        return false;
      }

      args.SolrUrl = this.solrUrl.Text;
      args.SorlRoot = this.solrRoot.Text;      
      args.ScriptRoot = System.IO.Path.Combine(Directory.GetParent(args.Product.PackagePath).FullName, System.IO.Path.GetFileNameWithoutExtension(args.Product.PackagePath));
      if (!Directory.Exists(args.ScriptRoot))
      {
        Directory.CreateDirectory(args.ScriptRoot);
        WindowHelper.LongRunningTask(() => this.UnpackInstallationFiles(args), "Unpacking unstallation files.", wizardArgs.WizardWindow);
      }
      else
      {
        if (MessageBox.Show(string.Format("Path '{0}' already exists. Do you want overwrite it?", args.ScriptRoot), "Overwrite?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
          Directory.Delete(args.ScriptRoot, true);
          Directory.CreateDirectory(args.ScriptRoot);
          WindowHelper.LongRunningTask(()=>this.UnpackInstallationFiles(args), "Unpacking unstallation files.", wizardArgs.WizardWindow);
        }
       
      }
      
      Tasker tasker = new Tasker(args.ScriptRoot, Tasker.ResolveGlobalFile(args.Product).FullName,string.Empty,true);
      InstallParam sqlServer = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SqlServer");
      if (sqlServer != null)
      {
        sqlServer.Value = args.InstanceConnectionString.DataSource;
      }

      InstallParam sqlAdminUser= tasker.GlobalParams.FirstOrDefault(p => p.Name == "SqlAdminUser");
      if (sqlAdminUser != null)
      {
        sqlAdminUser.Value = args.InstanceConnectionString.UserID;
      }

      InstallParam sqlAdminPass = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SqlAdminPassword");
      if (sqlAdminPass != null)
      {
        sqlAdminPass.Value = args.InstanceConnectionString.Password;
      }

      InstallParam sqlDbPrefix = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SqlDbPrefix");
      if (sqlDbPrefix != null)
      {
        sqlDbPrefix.Value = args.InstanceName.SubstringEx(0,args.InstanceName.IndexOf('.'));
      }     

      InstallParam solrUrl = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SolrUrl");
      if (solrUrl != null)
      {
        solrUrl.Value = args.SolrUrl;
      }

      InstallParam solrRoot = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SolrRoot");
      if (solrRoot != null)
      {
        solrRoot.Value = args.SorlRoot;
      }

      InstallParam solrService = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SolrService");
      if (solrService != null)
      {
        solrService.Value = this.SolrService.Text;
      }

      args.ScriptsOnly = this.scriptsOnly.IsChecked ?? false;      
      args.Takser = tasker;
      return true;
    }

    public void UnpackInstallationFiles(Install9WizardArgs args)
    {
      RealZipFile zip = new RealZipFile(new RealFile(new RealFileSystem(), args.Product.PackagePath));
      zip.ExtractTo(new RealFolder(new RealFileSystem(), args.ScriptRoot));
      string configFilesZipPath = Directory.GetFiles(args.ScriptRoot, "*Configuration files*.zip").First();
      RealZipFile configFilesZip = new RealZipFile(new RealFile(new RealFileSystem(), configFilesZipPath));
      configFilesZip.ExtractTo(new RealFolder(new RealFileSystem(), args.ScriptRoot));
    }


    private static string GetWebRootPath(string rootPath)
    {
      var webRootPath = Path.Combine(rootPath, "Website");
      return webRootPath;
    }

    #endregion
    #region Protected methods

    protected void Alert([NotNull] string message, [NotNull] params object[] args)
    {
      Assert.ArgumentNotNull(message, nameof(message));
      Assert.ArgumentNotNull(args, nameof(args));

      WindowHelper.ShowMessage(message.FormatWith(args), "Conflict is found", MessageBoxButton.OK, MessageBoxImage.Stop);
    }

    #endregion

    #region Private methods
    

    private void PickLocationFolder([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      WindowHelper.PickFolder("Choose location folder", this.solrRoot, null);
    }       
    #endregion
    #region IWizardStep Members

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var args = (Delete9WizardArgs)wizardArgs;
      string webConfigPath = Path.Combine(args.Instance.WebRootPath, "web.config");
      XmlDocument doc = new XmlDocument();
      doc.Load(webConfigPath);
      string role = doc.SelectSingleElement("//appSettings/add[@key='role:define']").Attributes["value"].Value;
      string rev = args.Product.Revision;
      if (role.Equals("Standalone", StringComparison.InvariantCultureIgnoreCase))
      {
        rev = rev + " (WDP XP0 packages)";
      }
      else
      {
        rev = rev + " (WDP XP1 packages)";
      }

      args.Product = SIM.Products.ProductManager.FindProduct(ProductType.Standalone, args.Product.Name, args.Product.TwoVersion, rev);
      ProductName.Text = args.Product.DisplayName;
      _InstallParameters = args;
      this.InstanceName.Text = args.InstanceName;
      this.solrUrl.Text = args.Instance.Configuration.ConnectionStrings.FirstOrDefault(c => c.Name == "solr.search")?.Value;
     
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    #endregion
  }
}
