using System.Collections.Generic;
using SIM.Pipelines.Processors;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using SIM.Pipelines.Import;
using System.Data.SqlClient;
using SIM.Base;
using System.IO;
using System.Xml;

namespace SIM.Tool.Windows.UserControls.Import
{
    public class ImportWizardArgs : WizardArgs
    {

        public string pathToExportedInstance = "";
        public string siteName = "";
        public string rootPath = "";
        public string virtualDirectoryPhysicalPath = "";
        string temporaryPathToUnpack = "";
        public bool updateLicense = false;
        public string pathToLicenseFile = "";
        public List<BindingsItem> bindings = new List<BindingsItem>();
        SqlConnectionStringBuilder connectionString = null;

        public ImportWizardArgs(string pathToExportedInstance)
        {
            this.pathToExportedInstance = pathToExportedInstance;
            connectionString = ProfileManager.GetConnectionString();
            //
            temporaryPathToUnpack = FileSystem.Local.Directory.RegisterTempFolder(FileSystem.Local.Directory.Ensure(Path.GetTempFileName() + "dir"));
            string websiteSettingsFilePath = FileSystem.Local.Zip.ZipUnpackFile(pathToExportedInstance, temporaryPathToUnpack, ImportArgs.websiteSettingsFileName);
            XmlDocumentEx websiteSettings = new XmlDocumentEx();
            websiteSettings.Load(websiteSettingsFilePath);
            siteName = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site", "name");
            virtualDirectoryPhysicalPath = websiteSettings.GetElementAttributeValue("/appcmd/SITE/site/application/virtualDirectory", "physicalPath");
            rootPath = FileSystem.Local.Directory.GetParent(virtualDirectoryPhysicalPath).FullName;
            this.bindings = GetBindings(websiteSettingsFilePath);
        }
        //
        List<BindingsItem> GetBindings(string websiteSettingsFilePath)//Dict {host}/{port}
        {
            List<BindingsItem> result = new List<BindingsItem>();
            XmlDocumentEx websiteSettings = new XmlDocumentEx();
            websiteSettings.Load(websiteSettingsFilePath);
            //
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
        //
        public override ProcessorArgs ToProcessorArgs()
        {
            Dictionary<string, int> bindingsForArgs = new Dictionary<string, int>();

            foreach(var binding in bindings)
            {
                if (binding.IsChecked)
                {
                    bindingsForArgs.Add(binding.hostName, binding.port);
                }
            }

            return new ImportArgs(pathToExportedInstance, siteName, temporaryPathToUnpack, rootPath, connectionString, updateLicense, pathToLicenseFile, bindingsForArgs);
        }

    }
    //
    public class BindingsItem
    {
        bool isChecked = true;
        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; }
        }
        public string hostName { get; set; }
        public int port { get; set; }
        //
        public BindingsItem(bool IsChecked, string Hostname, int Port)
        {
            this.isChecked = IsChecked;
            this.hostName = Hostname;
            this.port = Port;
        }
        //
        public BindingsItem(string Hostname, int Port)
        {
            this.hostName = Hostname;
            this.port = Port;
        }
        //
    }
    //
}
