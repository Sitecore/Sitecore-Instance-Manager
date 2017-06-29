namespace SIM.Pipelines.Install
{
  using System.Net;
  using SIM.IO;
  using SIM.IO.Real;

  public static class InstallRolesCommandHelper
  {
    public static void Install(IFolder root, string version, string role)
    {
      InstallAssemblyTo(root.GetChildFolder("bin"));
      InstallIncludeFiles(version, root);
      UpdateWebConfig(root, role);
    }

    private static void UpdateWebConfig(IFolder websiteDir, string role)
    {
      var webConfigFile = websiteDir.GetChildFile("web.config");
      webConfigFile.ReplaceLine("<section name=\"sitecore\" .+/>", "<section name=\"sitecore\" type=\"Sitecore.Configuration.Roles.RoleConfigReader, Sitecore.Configuration.Roles\" />");
      webConfigFile.ReplaceLine("</appSettings>", $"  <add key=\"role:define\" value=\"{role}\" />\r\n  </appSettings>");
    }

    private static void InstallIncludeFiles(string version, IFolder websiteFolder)
    {
      var configsUrl = $"https://github.com/Sitecore/Sitecore-Configuration-Roles/archive/configuration/{version}.zip";

      using (var tmp = websiteFolder.FileSystem.ParseTempFile())
      {
        new WebClient().DownloadFile(configsUrl, tmp.FullName);

        var includeFolder = websiteFolder.GetChildFolder("App_Config\\Include");
        foreach (var child in includeFolder.GetChildren())
        {
          var dir = child as IFolder;
          if (dir?.Name == "zzz")
          {
            continue;
          }

          child.TryDelete();
        }

        using (var zip = new RealZipFile(tmp))
        {
          zip.ExtractTo(websiteFolder);
        }
      }

      var configRolesFolder = websiteFolder.GetChildFolder($"Sitecore-Configuration-Roles-configuration-{version}");
      var appConfigDir = configRolesFolder.GetChildFolder("App_Config");
      appConfigDir.MoveTo(websiteFolder);
      configRolesFolder.TryDelete();
    }

    private static void InstallAssemblyTo(IFolder binFolder)
    {
      var assemblyUrl = "https://github.com/Sitecore/Sitecore-Configuration-Roles/releases/download/1.3/Sitecore.Configuration.Roles.dll";
      var assemblyFile = binFolder.GetChildFile("Sitecore.Configuration.Roles.dll");

      new WebClient()
        .DownloadFile(assemblyUrl, assemblyFile.FullName);
    }
  }
}