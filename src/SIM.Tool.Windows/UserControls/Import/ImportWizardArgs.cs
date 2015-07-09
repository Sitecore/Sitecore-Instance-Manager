namespace SIM.Tool.Windows.UserControls.Import
{
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.IO;
  using System.Xml;
  using SIM.Pipelines.Import;
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;

  public class ImportWizardArgs : WizardArgs
  {
    #region Fields

    public List<BindingsItem> bindings = new List<BindingsItem>();
    public string pathToExportedInstance = string.Empty;
    public string pathToLicenseFile = string.Empty;
    public string rootPath = string.Empty;
    public string siteName = string.Empty;
    public bool updateLicense = false;
    public string virtualDirectoryPhysicalPath = string.Empty;
    private SqlConnectionStringBuilder connectionString = null;
    private string temporaryPathToUnpack = string.Empty;

    #endregion

    #region Constructors

    public ImportWizardArgs(string pathToExportedInstance)
    {
      this.pathToExportedInstance = pathToExportedInstance;
      this.connectionString = ProfileManager.GetConnectionString();
      this.temporaryPathToUnpack = FileSystem.FileSystem.Local.Directory.RegisterTempFolder(FileSystem.FileSystem.Local.Directory.Ensure(Path.GetTempFileName() + "dir"));
      string websiteSettingsFilePath = FileSystem.FileSystem.Local.Zip.ZipUnpackFile(pathToExportedInstance, this.temporaryPathToUnpack, ImportArgs.websiteSettingsFileName);
      XmlDocumentEx websiteSettings = new XmlDocumentEx();
      websiteSettings.Load(websiteSettingsFilePath);
      this.siteName = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site", "name");
      this.virtualDirectoryPhysicalPath = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site/application/virtualDirectory", "physicalPath");
      this.rootPath = FileSystem.FileSystem.Local.Directory.GetParent(this.virtualDirectoryPhysicalPath).FullName;
      this.bindings = this.GetBindings(websiteSettingsFilePath);
    }

    #endregion



    #region Public methods

    public override ProcessorArgs ToProcessorArgs()
    {
      Dictionary<string, int> bindingsForArgs = new Dictionary<string, int>();

      foreach (var binding in this.bindings)
      {
        if (binding.IsChecked)
        {
          bindingsForArgs.Add(binding.hostName, binding.port);
        }
      }

      return new ImportArgs(this.pathToExportedInstance, this.siteName, this.temporaryPathToUnpack, this.rootPath, this.connectionString, this.updateLicense, this.pathToLicenseFile, bindingsForArgs);
    }

    #endregion

    #region Private methods

    private List<BindingsItem> GetBindings(string websiteSettingsFilePath)
    {
      // Dict {host}/{port}
      List<BindingsItem> result = new List<BindingsItem>();
      XmlDocumentEx websiteSettings = new XmlDocumentEx();
      websiteSettings.Load(websiteSettingsFilePath);
      XmlElement bindingsElement = websiteSettings.SelectSingleElement("/appcmd/SITE/site/bindings");
      if (bindingsElement != null)
      {
        foreach (XmlElement bindingElement in bindingsElement.ChildNodes)
        {
          result.Add(new BindingsItem(bindingElement.Attributes["bindingInformation"].Value.Split(':')[2], int.Parse(bindingElement.Attributes["bindingInformation"].Value.Split(':')[1])));
        }

        return result;
      }

      return null;
    }

    #endregion
  }

  public class BindingsItem
  {
    #region Fields

    private bool isChecked = true;

    #endregion

    #region Constructors

    public BindingsItem(bool IsChecked, string Hostname, int Port)
    {
      this.isChecked = IsChecked;
      this.hostName = Hostname;
      this.port = Port;
    }

    public BindingsItem(string Hostname, int Port)
    {
      this.hostName = Hostname;
      this.port = Port;
    }

    #endregion

    #region Public properties

    public bool IsChecked
    {
      get
      {
        return this.isChecked;
      }

      set
      {
        this.isChecked = value;
      }
    }

    public string hostName { get; set; }
    public int port { get; set; }

    #endregion


  }

}