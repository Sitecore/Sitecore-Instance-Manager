#region Usings

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SIM.Base;
using SIM.Tool.Base.Pipelines;

#endregion

namespace SIM.Tool.Windows.Pipelines
{
  using SIM.Tool.Base;

  public static class FinishActions
  {
    /// <summary>
    /// Opens solution.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    public static void OpenSolution(InstallModulesWizardArgs args)
    {
      var webRootPath = args.Instance.WebRootPath;
      if (OpenSolution(args.Instance.RootPath)) return;
      if (OpenSolution(webRootPath)) return;
      throw new Exception("Cannot locate any Visual Studio project files");
    }

    /// <summary>
    /// Opens solution.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    public static void OpenSolution(InstallWizardArgs args)
    {
      if (OpenSolution(args.InstanceRootPath)) return;
      if (OpenSolution(args.InstanceWebRootPath)) return;
      throw new Exception("Cannot locate any Visual Studio project files");
    }

    private static bool OpenSolution(string root)
    {
      var path = FileSystem.Local.Directory.GetFiles(root, "*.sln", SearchOption.TopDirectoryOnly).SingleOrDefault() ?? FileSystem.Local.Directory.GetFiles(root, "*.csproj", SearchOption.TopDirectoryOnly).SingleOrDefault();
      if (!string.IsNullOrEmpty(path) && FileSystem.Local.File.Exists(path))
      {
        WindowHelper.RunApp(path);
        return true;
      }

      return false;
    }
  }
}
