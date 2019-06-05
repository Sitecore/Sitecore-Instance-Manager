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
      args.ScriptsOnly = this.scriptsOnly.IsChecked ?? false;

      args.ScriptRoot = args.Takser.GlobalParams.FirstOrDefault(p => p.Name == "FilesRoot").Value;
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
          WindowHelper.LongRunningTask(() => this.UnpackInstallationFiles(args), "Unpacking unstallation files.", wizardArgs.WizardWindow);
        }
      }

      args.Takser.GlobalParams.FirstOrDefault(p => p.Name == "FilesRoot").Value = args.ScriptRoot;      
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
    

         
    #endregion
    #region IWizardStep Members

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var args = (Delete9WizardArgs)wizardArgs;
      args.Takser = new Tasker(args.UnInstallPath);
      this.Solrs.DataContext = ProfileManager.Profile.Solrs;
      string solrRoot = args.Takser.GlobalParams.FirstOrDefault(p => p.Name == "SolrRoot")?.Value;
      string solrUrl= args.Takser.GlobalParams.FirstOrDefault(p => p.Name == "SolrUrl")?.Value;
      string solrService = args.Takser.GlobalParams.FirstOrDefault(p => p.Name == "SolrService")?.Value;
      SolrDefinition solr = ProfileManager.Profile.Solrs.FirstOrDefault(s => s.Root == solrRoot && s.Url == solrUrl && s.Service == solrService);
      this.Solrs.SelectedItem = solr;
      this.InstanceName.Text = args.Takser.GlobalParams.First(p => p.Name == "SqlDbPrefix").Value;
      args.Takser.UnInstall = true;

    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    #endregion
  }
}
