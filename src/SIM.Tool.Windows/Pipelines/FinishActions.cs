namespace SIM.Tool.Windows.Pipelines
{
  using System;
  using System.IO;
  using System.Linq;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Pipelines;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core;

  public static class FinishActions
  {
    #region Public methods

    public static void OpenSolution(InstallModulesWizardArgs args)
    {
      var webRootPath = args.Instance.WebRootPath;
      if (OpenSolution(args.Instance.RootPath))
      {
        return;
      }

      if (OpenSolution(webRootPath))
      {
        return;
      }

      throw new Exception("Cannot locate any Visual Studio project files");
    }

    public static void OpenSolution(InstallWizardArgs args)
    {
      if (OpenSolution(args.InstanceRootPath))
      {
        return;
      }

      if (OpenSolution(args.InstanceWebRootPath))
      {
        return;
      }

      throw new Exception("Cannot locate any Visual Studio project files");
    }

    [UsedImplicitly]
    public static void OpenToolbox(InstallWizardArgs args)
    {
      if (!InstanceHelperEx.PreheatInstance(args.Instance, args.WizardWindow))
      {
        return;
      }

      InstanceHelperEx.BrowseInstance(args.Instance, args.WizardWindow, "/sitecore/admin", true);
    }

    [UsedImplicitly]
    public static void OpenToolbox(InstallModulesWizardArgs args)
    {
      if (!InstanceHelperEx.PreheatInstance(args.Instance, args.WizardWindow))
      {
        return;
      }

      InstanceHelperEx.BrowseInstance(args.Instance, args.WizardWindow, "/sitecore/admin", true);
    }

    #endregion

    #region Private methods

    private static bool OpenSolution(string root)
    {
      var path = FileSystem.FileSystem.Local.Directory.GetFiles(root, "*.sln", SearchOption.TopDirectoryOnly).SingleOrDefault() ?? FileSystem.FileSystem.Local.Directory.GetFiles(root, "*.csproj", SearchOption.TopDirectoryOnly).SingleOrDefault();
      if (!string.IsNullOrEmpty(path) && FileSystem.FileSystem.Local.File.Exists(path))
      {
        CoreApp.RunApp(path);
        return true;
      }

      return false;
    }

    #endregion
  }
}