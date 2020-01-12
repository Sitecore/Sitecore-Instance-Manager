using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;

namespace SitecoreInstaller.Validation.FileSystem
{
    class ConfigValueReader
    {

        public ConfigValueReader()
        {
        
        }

        public static ConfigValueReader Instance { get { return new ConfigValueReader();} }

        public string GetXconnectThumbprint(string siteName)
        {
            string xConnectPath = FolderChecks.GetSitePath(siteName);
            string thumbprint = null;
            if (xConnectPath != null)
            {
                var appSettings = xConnectPath + "\\App_Config\\AppSettings.config";
                if (File.Exists(appSettings))
                {
                    XmlDocument doc = new ConfigXmlDocument();
                    using (var fileStream = File.OpenRead(appSettings))
                    {
                        using (XmlReader fileReader = XmlReader.Create(fileStream)) //File.OpenRead(appSettings))
                        {
                            while (fileReader.Read())
                            {
                                if (fileReader["name"] == "validateCertificateThumbprint")
                                {
                                    thumbprint = fileReader["value"];
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return thumbprint;
        }

        public Dictionary<string,string> GetSitecoreThumbprints(string siteName)
        {
            string sitecorePath = FolderChecks.GetSitePath(siteName);
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (sitecorePath != null)
            {
                var appConfig = sitecorePath + "\\App_Config\\ConnectionStrings.config";
                using (var fileStream = File.OpenRead(appConfig))
                {
                    using (XmlReader fileReader = XmlReader.Create(fileStream))
                    {
                        string name;
                        string connString;
                        while (fileReader.Read())
                        {
                            name = fileReader["name"];
                            if (name != null && name.Contains(".certificate"))
                            {
                                connString = fileReader["connectionString"];
                                if (!String.IsNullOrEmpty(connString))
                                {
                                    result.Add(name, connString.Split(new[] { "FindValue=" }, StringSplitOptions.RemoveEmptyEntries).Last());
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public Dictionary<string, string> GetAllThumbprints(string sitecoreSiteName, string xConnectSiteName)
        {

            Dictionary<string, string> result = GetSitecoreThumbprints(sitecoreSiteName);
            result.Add("xConnect", GetXconnectThumbprint(xConnectSiteName));
            return result;
        }

    }
}
