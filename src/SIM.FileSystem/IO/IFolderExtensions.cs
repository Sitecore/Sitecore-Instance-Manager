using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SIM.IO
{
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;

  public static class FolderExtensions
  {
    [NotNull]
    public static IFile GetChildFile([NotNull] this IFolder folder, [NotNull] string fileName)
    {
      Assert.ArgumentNotNull(folder, nameof(folder));
      Assert.ArgumentNotNullOrEmpty(fileName, nameof(fileName));

      return folder.FileSystem.ParseFile(System.IO.Path.Combine(folder.FullName, fileName));
    }

    [NotNull]
    public static IFolder GetChildFolder([NotNull] this IFolder folder, [NotNull] string folderName)
    {
      Assert.ArgumentNotNull(folder, nameof(folder));
      Assert.ArgumentNotNullOrEmpty(folderName, nameof(folderName));

      return folder.FileSystem.ParseFolder(System.IO.Path.Combine(folder.FullName, folderName));
    }
    
    public static void ReplaceLine(this IFile file, string pattern, string replacement)
    {
      var regex = new Regex(pattern, RegexOptions.Compiled);
      var lines = new List<string>();
      using (var reader = new StreamReader(file.Open(OpenFileMode.Open, OpenFileAccess.Read, OpenFileShare.Read)))
      {
        while (reader.Peek() >= 0)
        {
          var line = reader.ReadLine();
          Assert.IsNotNull(line, nameof(line));

          lines.Add(regex.Replace(line, replacement));
        }
      }

      using (var writer = new StreamWriter(file.Open(OpenFileMode.Open, OpenFileAccess.Write, OpenFileShare.None)))
      {
        foreach (var line in lines)
        {
          writer.WriteLine(line);
        }
      }
    }
  }
}