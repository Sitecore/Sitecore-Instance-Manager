using System.Collections.Generic;
using System.Linq;

namespace SIM.IO.Real
{
  using System;
  using System.IO;
  using JetBrains.Annotations;

  public class RealFolder : FileSystemEntry, IFolder
  {
    [CanBeNull]
    private IFolder _Parent;

    public RealFolder([NotNull] SIM.IO.IFileSystem fileSystem, [NotNull] string path) : base(fileSystem, path)
    {
      DirectoryInfo = new DirectoryInfo(FullName);
    }

    [NotNull]
    public DirectoryInfo DirectoryInfo { get; }

    public override bool Exists => DirectoryInfo.Exists;

    public override IFolder Parent => _Parent ?? (_Parent = FileSystem.ParseFolder(DirectoryInfo.Parent));

    public IReadOnlyList<IFileSystemEntry> GetChildren()
    {
      return DirectoryInfo
        .GetFileSystemInfos()
        .Select(x => x is FileInfo ? (IFileSystemEntry)FileSystem.ParseFile(x.FullName) : FileSystem.ParseFolder(x.FullName))
        .ToArray();
    }

    public IReadOnlyList<IFile> GetFiles()
    {
      return DirectoryInfo
        .GetFiles()
        .Select(x => FileSystem.ParseFile(x.FullName))
        .ToArray();
    }

    public IReadOnlyList<IFolder> GetFolders()
    {
      return DirectoryInfo
        .GetDirectories()
        .Select(x => FileSystem.ParseFolder(x.FullName))
        .ToArray();
    }

    public IFolder MoveTo(IFolder parent)
    {
      // http://stackoverflow.com/questions/2553008/directory-move-doesnt-work-file-already-exist
      var stack = new Stack<KeyValuePair<string, string>>();
      var newFullName = Path.Combine(parent.FullName, Name);
      stack.Push(new KeyValuePair<string, string>(FullName, newFullName));

      while (stack.Count > 0)
      {
        var folders = stack.Pop();
        Directory.CreateDirectory(folders.Value);
        foreach (var file in Directory.GetFiles(folders.Key))
        {
          var targetFile = Path.Combine(folders.Value, Path.GetFileName(file));
          if (File.Exists(targetFile))
          {
            File.Delete(targetFile);
          }

          File.Move(file, targetFile);
        }

        foreach (var folder in Directory.GetDirectories(folders.Key))
        {
          stack.Push(new KeyValuePair<string, string>(folder, Path.Combine(folders.Value, Path.GetFileName(folder))));
        }
      }

      TryDelete();

      return FileSystem.ParseFolder(newFullName);
    }

    public override void Create()
    {
      DirectoryInfo.Create();
    }

    public bool Equals(IFolder other)
    {
      return FullName.Equals(other?.FullName, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj)
    {
      return Equals((RealFolder)obj);
    }

    protected bool Equals(RealFolder other)
    {
      return DirectoryInfo.Equals(other.DirectoryInfo);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (base.GetHashCode() * 397) ^ DirectoryInfo.GetHashCode();
      }
    }

    public override void TryDelete()
    {
      try
      {
        DirectoryInfo.Delete(true);
      }
      catch
      {
        // we don't care if we cannot delete the file whatever error happens
      }
    }
  }
}