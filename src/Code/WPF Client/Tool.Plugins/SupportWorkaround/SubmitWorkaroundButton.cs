namespace SIM.Tool.Plugins.SupportWorkaround
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Windows;
  using Microsoft.Win32;
  using SIM.Base;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;

  public class SubmitWorkaroundButton : IMainWindowButton
  {
    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return EnvironmentHelper.IsSitecoreMachine;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      try
      {
        const string ChooseFile = "Choose Visual Studio Project file (*.csproj)";
        const string CreateProject = "Generate Visual Studio Project from template";
        const string Cancel = "Cancel";

        if (instance != null)
        {
          // check if there is any project file
          var files = VsHelper.GetWorkaroundProjectFiles(instance);
          if(files.Length > 1)
          {
            foreach (var file in files)
            {
              SubmitContribution(file, mainWindow);
            }
          }
          else if(files.Length == 0)
          {
            var options = new[]
            {
              ChooseFile,
              CreateProject,
              Cancel
            };

            var result = WindowHelper.AskForSelection("No local patch projects", null, "No Sitecore.Support.###### projects found in the Website folder, you have 3 options: ", options, mainWindow, ChooseFile);
            if (result == null || result == Cancel)
            {
              return;
            }

            if (result == ChooseFile)
            {
              var dialog = new OpenFileDialog()
              {
                Filter = "Visual Studio Project (*.csproj)|*.csproj|Visual Studio Solution (*.sln)|*.sln",
                CheckFileExists = true
              };

              var fileResult = dialog.ShowDialog(mainWindow);
              if (fileResult == false)
              {
                return;
              }

              var projectFilePath = dialog.FileName;
              if (string.IsNullOrEmpty(projectFilePath) || File.Exists(projectFilePath))
              {
                return;
              }

              SubmitContribution(projectFilePath, mainWindow);
            }
          }
        }
        else
        {
          var options = new[]
            {
              ChooseFile,
              Cancel
            };

            var result = WindowHelper.AskForSelection("No Sitecore instances selected", null, "No Sitecore instances selected so you have 2 options: ", options, mainWindow, ChooseFile);
            if (result == null || result == Cancel)
            {
              return;
            }

          if (result == ChooseFile)
          {
            var dialog = new OpenFileDialog()
            {
              Filter = "Visual Studio Project (*.csproj)|*.csproj|Visual Studio Solution (*.sln)|*.sln",
              CheckFileExists = true
            };

            var fileResult = dialog.ShowDialog(mainWindow);
            if (fileResult == false)
            {
              return;
            }

            var projectFilePath = dialog.FileName;
            if (string.IsNullOrEmpty(projectFilePath) || !File.Exists(projectFilePath))
            {
              return;
            }

            SubmitContribution(projectFilePath, mainWindow);
          }
        }
      }
      catch (OperationCanceledException)
      {
      }
    }

    private static void SubmitContribution([NotNull] string projectFilePath, [NotNull] Window mainWindow)
    {
      Assert.ArgumentNotNull(projectFilePath, "projectFilePath");
      Assert.ArgumentNotNull(mainWindow, "mainWindow");
      const string Repository = "\\\\pssbuild1dk1.dk.sitecore.net\\Patches\\Contributions";
      var name = Path.GetFileNameWithoutExtension(projectFilePath);
      Action task = delegate
      {
        var referencedFiles = VsHelper.FindFilesToCommit(projectFilePath);
        var targetfolder = Repository + "\\" + name + " - " + DateTime.Now.ToString("yy-MM-dd hh-mm-ss");
        FileSystem.Local.Directory.Ensure(targetfolder);
        CopyFiles(referencedFiles, Path.GetDirectoryName(projectFilePath), targetfolder);
      };

      WindowHelper.LongRunningTask(task, "Submitting patch " + name, mainWindow, "Copying the " + name + " source code to \n" + Repository, null, false, true);
    }

    internal static void CopyFiles(IEnumerable<string> referencedFiles, string rootFolder, string targetFolder)
    {
      foreach (var file in referencedFiles)
      {
        var virtualPath = file.Substring(rootFolder.Length).TrimStart('\\');
        FileSystem.Local.File.AssertExists(file, "File \"{0}\" referred in project does not exist".FormatWith(virtualPath),
          false);
        var newFile = Path.Combine(targetFolder, virtualPath);
        FileSystem.Local.Directory.Ensure(Path.GetDirectoryName(newFile));
        FileSystem.Local.File.Copy(file, newFile);
      }
    }
  }
}