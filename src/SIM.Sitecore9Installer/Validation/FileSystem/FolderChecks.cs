using System;
using System.IO;
using Microsoft.Win32;

namespace SitecoreInstaller.Validation.FileSystem
{
    public class FolderChecks
    {
        public static bool ValidatePath(string siteName)
        {
            var sitePath = GetSitePath(siteName);
            if (sitePath != null)
            {
                return Directory.Exists(sitePath);
            }
            return false;
        }

        public static string GetSitePath(string siteName)
        {
            var defaultPath = "C:\\inetpub\\wwwroot";
            var registryKeyWWWPath =
                Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\INetStp", "PathWWWRoot", defaultPath);
            if (registryKeyWWWPath != null && !registryKeyWWWPath.Equals(string.Empty))
            {
                return String.Concat(registryKeyWWWPath, '\\', siteName);
            }

            return null;
        }

        //TODO: maybe remove
        public static string GetSitePath(string siteName, string defaultPath, out bool defaultPathUsed)
        {
            defaultPathUsed = false;
            return null;
        }
    }
}
