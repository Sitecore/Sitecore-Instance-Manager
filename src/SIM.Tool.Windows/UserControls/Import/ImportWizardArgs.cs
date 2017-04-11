namespace SIM.Tool.Windows.UserControls.Import
{
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.IO;
  using System.Xml;
  using SIM.Extensions;
  using SIM.Pipelines.Import;
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;

  public class ImportWizardArgs : WizardArgs
  {
    #region Fields

    public List<BindingsItem> _Bindings = new List<BindingsItem>();
    public string _PathToExportedInstance = string.Empty;
    public string _PathToLicenseFile = string.Empty;
    public string _RootPath = string.Empty;
    public string _SiteName = string.Empty;
    public bool _UpdateLicense = false;
    public string _VirtualDirectoryPhysicalPath = string.Empty;
    private SqlConnectionStringBuilder _ConnectionString = null;
    private string _TemporaryPathToUnpack = string.Empty;

    #endregion

    #region Constructors

    public ImportWizardArgs(string pathToExportedInstance)
    {
      this._PathToExportedInstance = pathToExportedInstance;
      this._ConnectionString = ProfileManager.GetConnectionString();
      this._TemporaryPathToUnpack = FileSystem.FileSystem.Local.Directory.RegisterTempFolder(FileSystem.FileSystem.Local.Directory.Ensure(Path.GetTempFileName() + "dir"));
      var websiteSettingsFilePath = FileSystem.FileSystem.Local.Zip.ZipUnpackFile(pathToExportedInstance, this._TemporaryPathToUnpack, ImportArgs.WebsiteSettingsFileName);
      XmlDocumentEx websiteSettings = new XmlDocumentEx();
      websiteSettings.Load(websiteSettingsFilePath);
      this._SiteName = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site", "name");
      this._VirtualDirectoryPhysicalPath = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site/application/virtualDirectory", "physicalPath");
      this._RootPath = FileSystem.FileSystem.Local.Directory.GetParent(this._VirtualDirectoryPhysicalPath).FullName;
      this._Bindings = this.GetBindings(websiteSettingsFilePath);
    }

    #endregion



    #region Public methods

    public override ProcessorArgs ToProcessorArgs()
    {
      Dictionary<string, int> bindingsForArgs = new Dictionary<string, int>();

      foreach (var binding in this._Bindings)
      {
        if (binding.IsChecked)
        {
          bindingsForArgs.Add(binding.HostName, binding.Port);
        }
      }

      return new ImportArgs(this._PathToExportedInstance, this._SiteName, this._TemporaryPathToUnpack, this._RootPath, this._ConnectionString, this._UpdateLicense, this._PathToLicenseFile, bindingsForArgs);
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

    private bool _IsChecked = true;

    #endregion

    #region Constructors

    public BindingsItem(bool IsChecked, string hostname, int Port)
    {
      this._IsChecked = IsChecked;
      this.HostName = hostname;
      this.Port = Port;
    }

    public BindingsItem(string hostname, int Port)
    {
      this.HostName = hostname;
      this.Port = Port;
    }

    #endregion

    #region Public properties

    public bool IsChecked
    {
      get
      {
        return this._IsChecked;
      }

      set
      {
        this._IsChecked = value;
      }
    }

    public string HostName { get; set; }
    public int Port { get; set; }

    #endregion


  }

}