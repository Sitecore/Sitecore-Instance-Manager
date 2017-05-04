namespace SIM.IO.Mock
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using Sitecore.Diagnostics.Base;

  public class MockFileSystem : SIM.IO.IFileSystem
  {
    internal Dictionary<string, MockFile> Files { get; } = new Dictionary<string, MockFile>();

    internal Dictionary<string, MockFolder> Folders { get; } = new Dictionary<string, MockFolder>();

    public IFolder ParseFolder(string path)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));

      return new MockFolder(this, path);
    }

    public IFile ParseFile(string path)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));

      return ParseFile(path, string.Empty);
    }

    public IZipFile ParseZipFile(string path)
    {
      throw new NotImplementedException();
    }

    public IFile ParseFile(string path, string contents)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));
      Assert.ArgumentNotNullOrEmpty(contents, nameof(contents));

      var file = new MockFile(this, path, contents) { Exists = true };

      return file;
    }

    public T Add<T>(string path, T entry) where T : MockFileSystemEntry
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));
      Assert.ArgumentNotNull(entry, nameof(entry));

      var file = entry as MockFile;
      var folder = entry as MockFolder;
      if (file != null)
      {
        Files.Add(Path.GetFullPath(path), file);
      }
      else if (folder != null)
      {
        Folders.Add(Path.GetFullPath(path), folder);
      }
      else
      {
        throw new NotSupportedException($"{typeof(T).Name} is not supported");
      }

      return entry;
    }
  }
}