namespace SIM.IO.Mock
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using Sitecore.Diagnostics.Base;

  public class MockFileSystem : Dictionary<string, MockFileSystemEntry>, SIM.IO.IFileSystem
  {
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

    public IFile ParseFile(string path, string contents)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));
      Assert.ArgumentNotNullOrEmpty(contents, nameof(contents));

      var file = new MockFile(this, path, contents) { Exists = true };  

      return file;
    }
                
    public T Add<T>(string path, T file) where T : MockFileSystemEntry
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));
      Assert.ArgumentNotNull(file, nameof(file));

      base.Add(Path.GetFullPath(path), file);

      return file;
    }

    public bool Contains(string path)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));

      return ContainsKey(Path.GetFullPath(path));
    }
  }
}