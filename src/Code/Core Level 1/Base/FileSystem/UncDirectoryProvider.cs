namespace SIM.Base
{
  using System.Collections.Generic;
  using System.IO;

  public class UncDirectoryProvider : DirectoryProvider
  {
    [NotNull]
    private readonly FileSystem fileSystem;

    public UncDirectoryProvider([NotNull] FileSystem fileSystem) : base(fileSystem)
    {
      Assert.ArgumentNotNull(fileSystem, "fileSystem");

      this.fileSystem = fileSystem;
    }

    #region Overrides of DirectoryProvider

    /// <summary>
    /// The assert folder exists.
    /// </summary>
    /// <param name="path">
    /// The path. 
    /// </param>
    /// <param name="message">
    /// The message. 
    /// </param>
    public override void AssertExists(string path, string message)
    {
      try
      {
        base.AssertExists(path, message);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        base.AssertExists(path, message);
      }
    }

    public override void Copy(string path, string newPath, bool recusive)
    {
      try
      {
        base.Copy(path, newPath, recusive);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);
        newPath = this.fileSystem.Path.ToUncPath(newPath);

        base.Copy(path, newPath, recusive);
      }
    }

    /// <summary>
    /// The create directory.
    /// </summary>
    /// <param name="path">
    /// The path. 
    /// </param>
    public override DirectoryInfo CreateDirectory(string path)
    {
      try
      {
        return base.CreateDirectory(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.CreateDirectory(path);
      }
    }

    /// <summary>
    /// The delete.
    /// </summary>
    /// <param name="path">
    /// The path. 
    /// </param>
    public override void Delete(string path)
    {
      try
      {
        base.Delete(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        base.Delete(path);
      }
    }

    public override void DeleteContents(string path)
    {
      try
      {
        base.DeleteContents(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        base.DeleteContents(path);
      }
    }

    /// <summary>
    /// The delete if exists.
    /// </summary>
    /// <param name="path">
    /// The path. 
    /// </param>
    public override void DeleteIfExists(string path, string ignore = null)
    {
      try
      {
        base.DeleteIfExists(path, ignore);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        base.DeleteIfExists(path, ignore);
      }
    }

    /// <summary>
    /// The ensure folder.
    /// </summary>
    /// <param name="path">
    /// The folder. 
    /// </param>
    /// <returns>
    /// The ensure folder. 
    /// </returns>
    public override string Ensure(string path)
    {
      try
      {
        return base.Ensure(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.Ensure(path);
      }
    }

    public override bool Exists(string path)
    {
      try
      {
        return base.Exists(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.Exists(path);
      }
    }

    /// <summary>
    /// The find common parent.
    /// </summary>
    /// <param name="path1">
    /// The path 1. 
    /// </param>
    /// <param name="path2">
    /// The path 2. 
    /// </param>
    /// <returns>
    /// The find common parent. 
    /// </returns>
    public override string FindCommonParent(string path1, string path2)
    {
      try
      {
        return base.FindCommonParent(path1, path2);
      }
      catch (PathTooLongException)
      {
        path1 = this.fileSystem.Path.ToUncPath(path1);
        path2 = this.fileSystem.Path.ToUncPath(path2);

        return base.FindCommonParent(path1, path2);
      }
    }

    /// <summary>
    /// The generate temp folder path.
    /// </summary>
    /// <param name="path">
    /// The folder. 
    /// </param>
    /// <returns>
    /// The generate temp folder path. 
    /// </returns>
    public override string GenerateTempFolderPath(string path)
    {
      try
      {
        return base.GenerateTempFolderPath(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.GenerateTempFolderPath(path);
      }
    }

    /// <summary>
    /// The get ancestors.
    /// </summary>
    /// <param name="path">
    /// The folder. 
    /// </param>
    /// <returns>
    /// </returns>
    public override IEnumerable<string> GetAncestors(string path)
    {
      try
      {
        return base.GetAncestors(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.GetAncestors(path);
      }
    }

    public override string[] GetDirectories(string path)
    {
      try
      {
        return base.GetDirectories(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.GetDirectories(path);
      }
    }

    public override string[] GetDirectories(string path, string pattern)
    {
      try
      {
        return base.GetDirectories(path, pattern);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.GetDirectories(path, pattern);
      }
    }

    public override string GetDirectoryRoot(string path)
    {
      try
      {
        return base.GetDirectoryRoot(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.GetDirectoryRoot(path);
      }
    }

    public override int GetDistance(string directory1, string directory2)
    {
      try
      {
        return base.GetDistance(directory1, directory2);
      }
      catch (PathTooLongException)
      {
        directory1 = this.fileSystem.Path.ToUncPath(directory1);
        directory2 = this.fileSystem.Path.ToUncPath(directory2);

        return base.GetDistance(directory1, directory2);
      }
    }

    public override void Move(string path, string newPath)
    {
      try
      {
        base.Move(path, newPath);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);
        newPath = this.fileSystem.Path.ToUncPath(newPath);

        base.Move(path, newPath);
      }
    }

    /// <summary>
    /// The try delete.
    /// </summary>
    /// <param name="path">
    /// The path. 
    /// </param>
    public override void TryDelete(string path)
    {
      try
      {
        base.TryDelete(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        base.TryDelete(path);
      }
    }

    /// <summary>
    /// Maps virtual path.
    /// </summary>
    /// <param name="virtualPath">
    /// The virtual path. 
    /// </param>
    /// <param name="rootPath">
    /// The root path. 
    /// </param>
    /// <returns>
    /// The mapped path. 
    /// </returns>
    public override string MapPath(string virtualPath, string rootPath)
    {
      try
      {
        return base.MapPath(virtualPath, rootPath);
      }
      catch (PathTooLongException)
      {
        rootPath = this.fileSystem.Path.ToUncPath(rootPath);

        return base.MapPath(virtualPath, rootPath);
      }
    }

    /// <summary>
    /// The move child.
    /// </summary>
    /// <param name="extracted">
    /// The extracted. 
    /// </param>
    /// <param name="childName">
    /// The child name. 
    /// </param>
    /// <param name="targetFolder">
    /// The target folder. 
    /// </param>
    public override void MoveChild(DirectoryInfo extracted, string childName, string targetFolder)
    {
      try
      {
        base.MoveChild(extracted, childName, targetFolder);
      }
      catch (PathTooLongException)
      {
        targetFolder = this.fileSystem.Path.ToUncPath(targetFolder);

        base.MoveChild(extracted, childName, targetFolder);
      }
    }

    /// <summary>
    /// The has drive letter.
    /// </summary>
    /// <param name="folder">
    /// The folder. 
    /// </param>
    /// <returns>
    /// The has drive letter. 
    /// </returns>
    public override bool HasDriveLetter(string folder)
    {
      try
      {
        return base.HasDriveLetter(folder);
      }
      catch (PathTooLongException)
      {
        folder = this.fileSystem.Path.ToUncPath(folder);

        return base.HasDriveLetter(folder);
      }
    }

    public override string GetPathRoot(string path)
    {
      try
      {
        return base.GetPathRoot(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.GetPathRoot(path);
      }
    }

    public override string[] GetFiles(string path, string filter, SearchOption searchMode)
    {
      try
      {
        return base.GetFiles(path, filter, searchMode);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.GetFiles(path, filter, searchMode);
      }
    }

    public override string[] GetFiles(string path, string filter)
    {
      try
      {
        return base.GetFiles(path, filter);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.GetFiles(path, filter);
      }
    }

    public override string[] GetFiles(string path)
    {
      try
      {
        return base.GetFiles(path);
      }
      catch (PathTooLongException)
      {
        path = this.fileSystem.Path.ToUncPath(path);

        return base.GetFiles(path);
      }
    }

    #endregion
  }
}