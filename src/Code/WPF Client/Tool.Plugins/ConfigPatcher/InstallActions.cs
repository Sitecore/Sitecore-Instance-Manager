namespace SIM.Tool.Plugins.ConfigPatcher
{
  using System.IO;
  using SIM.Instances;
  using SIM.Pipelines.InstallModules;
  using SIM.Base;
  using SIM.Products;

  using SIM.Tool.Base;

  public class InstallActions : IPackageInstallActions
  {
    public void Execute(Instance instance, Product module)
    {
      PatchConfigurationFiles(instance.WebRootPath);
    }

    /// <summary>
    /// Patches the configuration files.
    /// </summary>
    /// <param name="webRootPath">The web root path.</param>
    private static void PatchConfigurationFiles(string webRootPath)
    {
      var buildFolder = Path.Combine(webRootPath, "build");
      if (FileSystem.Local.Directory.Exists(buildFolder))
      {
        var patchFiles = FileSystem.Local.Directory.GetFiles(buildFolder, "*", SearchOption.AllDirectories);
        foreach (var patchFile in patchFiles)
        {
          var fileToPatch = Path.Combine(webRootPath, Path.GetDirectoryName(patchFile).Remove(0, buildFolder.Length + 1));
          var outputFile = Path.Combine(Path.GetDirectoryName(fileToPatch), Path.GetFileNameWithoutExtension(patchFile));
          Log.Info("Patching {0} file into {1} using {2} instructions".FormatWith(fileToPatch, outputFile, patchFile), typeof(InstallActions));
          string exeFileName = Path.Combine(ApplicationManager.PluginsFolder, "Config Patcher\\xmlpatch.exe");
          FileSystem.Local.File.AssertExists(exeFileName);
          var process = WindowHelper.RunApp(exeFileName, "/o", outputFile, fileToPatch, patchFile).IsNotNull("Cannot run the process");
          process.WaitForExit(10000);
          FileSystem.Local.Directory.DeleteIfExists(patchFile);
        }

        //FileSystem.DeleteEmptyFolder(buildFolder);
      }
    }
  }
}
