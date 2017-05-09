using System.IO;
using System.Net;
using JetBrains.Annotations;
using Sitecore.Diagnostics.Base;
using SIM.IO;
using SIM.IO.Real;
using SIM.Products;

namespace SIM.Pipelines.Install
{
  [UsedImplicitly]
  public class InstallRoles : InstallProcessor
  {
    [NotNull]
    private IFileSystem FileSystem { get; } = new RealFileSystem();

    #region Methods

    protected override void Process(InstallArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var role = args.InstallRoles;
      if (string.IsNullOrEmpty(role))
      {
        return;
      }

      var websiteDir = FileSystem.ParseFolder(args.WebRootPath);      
      var product = args.Product;
      var version = $"{product.Version}.{product.Update}";
      Process(websiteDir, version, role);
    }

    private void Process(IFolder root, string version, string role)
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

    private void InstallIncludeFiles(string version, IFolder websiteFolder)
    {
      var configsUrl = $"https://github.com/Sitecore/Sitecore-Configuration-Roles/archive/configuration/{version}.zip";
      var tempFolder = FileSystem.ParseFolder(Path.GetTempFileName() + ".dir");
      try
      {
        tempFolder.Create();

        using (var tmp = FileSystem.ParseZipFile(Path.GetTempFileName()))
        {
          new WebClient().DownloadFile(configsUrl, tmp.FullName);

          try
          {
            tmp.ExtractTo(tempFolder);
          }
          finally
          {
            tmp.TryDelete();
          }
        }

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

        var appConfigDir = tempFolder.GetChildFolder($"Sitecore-Configuration-Roles-configuration-{version}\\App_Config");
        appConfigDir.MoveTo(websiteFolder);
      }
      finally
      {
        tempFolder.TryDelete();
      }
    }

    private static void InstallAssemblyTo(IFolder binFolder)
    {
      var assemblyUrl = "https://github.com/Sitecore/Sitecore-Configuration-Roles/releases/download/1.2/Sitecore.Configuration.Roles.dll";
      var assemblyFile = binFolder.GetChildFile("Sitecore.Configuration.Roles.dll");

      new WebClient()
        .DownloadFile(assemblyUrl, assemblyFile.FullName);
    }

    #endregion
  }
}