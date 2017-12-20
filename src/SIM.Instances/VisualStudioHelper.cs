namespace SIM.Instances
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  public static class VisualStudioHelper
  {
    #region Public methods

    public static IEnumerable<string> DistinctVisualStudioFiles(ICollection<string> filePaths, string rootPath)
    {
      var resultFilePaths = filePaths.ToList();
      foreach (var filePath in filePaths)
      {
        try
        {
          if (Path.GetExtension(filePath).EqualsIgnoreCase(".sln"))
          {
            foreach (var projectFilePath in GetSolutionProjectFiles(filePath))
            {
              var item = resultFilePaths.SingleOrDefault(x => x.EqualsIgnoreCase(projectFilePath));
              resultFilePaths.Remove(item);
            }
          }
        }
        catch (Exception ex)
        {
          Log.Error(ex, $"Visual Studio project distinction failed on {filePath} file");
        }
      }

      return resultFilePaths;
    }


    public static IEnumerable<string> GetSolutionProjectFiles(string filePath)
    {
      var directoryPath = Path.GetDirectoryName(filePath);
      var lines = FileSystem.FileSystem.Local.File.ReadAllLines(filePath);
      foreach (var line in lines)
      {
        if (line.StartsWith("Project"))
        {
          var commaPos = line.IndexOf(',');
          if (commaPos < 0)
          {
            Log.Warn($"File {filePath} seems to be corrupted, line: {line}");
            continue;
          }

          var str = line.Substring(commaPos + 1).TrimStart().TrimStart('"').TrimStart();
          var quotePos = str.IndexOf('"');
          if (quotePos < 0)
          {
            Log.Warn($"File {filePath} seems to be corrupted, line: {line}");
            continue;
          }

          var projectRelativePath = str.Substring(0, quotePos);
          if (Path.GetExtension(projectRelativePath).EqualsIgnoreCase(".csproj"))
          {
            var projectFilePath = Path.Combine(directoryPath, projectRelativePath);
            yield return projectFilePath;
          }
        }
      }
    }

    public static IEnumerable<string> GetVisualStudioSolutionFiles(string rootPath, string webRootPath, string searchPattern = null)
    {
      var files = GetVisualStudioSolutionFilesRaw(rootPath, webRootPath, searchPattern).ToArray();
      return DistinctVisualStudioFiles(files, rootPath);
    }

    [CanBeNull]
    public static IEnumerable<string> GetVisualStudioSolutionFilesRaw(string rootPath, string webRootPath, string searchPattern = null)
    {
      searchPattern = searchPattern.EmptyToNull() ?? "*";
      foreach (string dir in new[]
      {
        rootPath, webRootPath
      }.Distinct())
      {
        foreach (string ext in new[]
        {
          ".sln", ".csproj"
        })
        {
          var files = FileSystem.FileSystem.Local.Directory.GetFiles(dir, searchPattern + ext, SearchOption.TopDirectoryOnly);
          foreach (var file in files)
          {
            yield return file;
          }
        }
      }
    }

    #endregion
  }
}