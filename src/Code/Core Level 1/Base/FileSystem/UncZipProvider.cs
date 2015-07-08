namespace SIM.Base
{
  using System;
  using System.IO;
  using Ionic.Zip;

  public class UncZipProvider : ZipProvider
  {
    [NotNull]
    private readonly FileSystem fileSystem;

    public UncZipProvider([NotNull] FileSystem fileSystem) : base(fileSystem)
    {
      Assert.ArgumentNotNull(fileSystem, "fileSystem");

      this.fileSystem = fileSystem;
    }

    /// <summary>
    /// The check zip.
    /// </summary>
    /// <param name="path">
    /// The package Path. 
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The \"{0}\" archive isn't a Sitecore installation package.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The \"{0}\" installation package seems to be corrupted.
    /// </exception>
    public override void CheckZip(string path)
    {
      try
      {
          base.CheckZip(path);
      }
      catch (ZipException ex)
      {
        if (!(ex.InnerException is PathTooLongException))
        {
          throw;
        }
        using (var temp = FileSystem.Local.Directory.GetTempFolder(path))
        {
          var shortPath = Path.Combine(temp.Path, Path.GetFileName(path));
          fileSystem.File.Copy(path, shortPath);

          base.CheckZip(shortPath);
        }
      }
    }

    public override void CreateZip(string path, string zipFileName, string ignore = null)
    {
      try
      {
        base.CreateZip(path, zipFileName, ignore);
      }
      catch (ZipException ex)
      {
        if (!(ex.InnerException is PathTooLongException))
        {
          throw;
        }
        using (var tempPath = FileSystem.Local.Directory.GetTempFolder(path))
        {
          using (var tempZip = FileSystem.Local.Directory.GetTempFolder(zipFileName))
          {
            var shortPath = Path.Combine(tempPath.Path, Path.GetFileName(path));
            fileSystem.Directory.Copy(path, shortPath, true);

            var shortZipPath = Path.Combine(tempZip.Path, Path.GetFileName(zipFileName));
            base.CreateZip(shortPath, shortZipPath, ignore);

            fileSystem.File.Copy(shortZipPath, zipFileName);
          }
        }
      }
    }

    public override string GetFirstRootFolder(string path)
    {
      try
      {
          return base.GetFirstRootFolder(path);
      }
      catch (ZipException ex)
      {
        if (!(ex.InnerException is PathTooLongException))
        {
          throw;
        }
        using (var temp = FileSystem.Local.Directory.GetTempFolder(path))
        {
          var shortPath = Path.Combine(temp.Path, Path.GetFileName(path));
          fileSystem.File.Copy(path, shortPath);
          
          return base.GetFirstRootFolder(shortPath);
        }
      }
    }

    public override void UnpackZip(string packagePath, string path, Action<long> incrementProgress, string ignore1 = null, string ignore2 = null)
    {
      try
      {
        base.UnpackZip(packagePath, path, incrementProgress, ignore1, ignore2);
      }
      catch (ZipException ex)
      {
        if (!(ex.InnerException is PathTooLongException))
        {
          throw;
        }
        using (var tempPath = FileSystem.Local.Directory.GetTempFolder(path))
        {
          using (var tempPkg = FileSystem.Local.Directory.GetTempFolder(packagePath))
          {
            var shortPath = Path.Combine(tempPath.Path, Path.GetFileName(path));
            
            var shortPkgPath = Path.Combine(tempPkg.Path, Path.GetFileName(packagePath));
            fileSystem.File.Copy(packagePath, shortPkgPath);

            base.UnpackZip(shortPkgPath, shortPath, incrementProgress, ignore1, ignore2);
            this.fileSystem.Directory.Copy(shortPath, path, true);
          }
        }
      }
    }

    /// <summary>
    /// The unpack zip.
    /// </summary>
    /// <param name="packagePath">
    /// The package Path. 
    /// </param>
    /// <param name="path">
    /// The path. 
    /// </param>
    /// <param name="entriesPattern">
    /// The entries Pattern. 
    /// </param>
    /// <param name="stepsCount">
    /// The steps Count. 
    /// </param>
    /// <param name="incrementProgress">
    /// The increment Progress. 
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The \"{0}\" installation package seems to be corrupted.
    /// </exception>
    public override void UnpackZip(string packagePath, string path, string entriesPattern = null, int stepsCount = 1, Action incrementProgress = null, bool skipErrors = false)
    {
      try
      {
      base.UnpackZip(packagePath, path, entriesPattern, stepsCount, incrementProgress, skipErrors);
      }
      catch (ZipException ex)
      {
        if (!(ex.InnerException is PathTooLongException))
        {
          throw;
        }
        using (var tempPath = FileSystem.Local.Directory.GetTempFolder(path))
        {
          using (var tempPkg = FileSystem.Local.Directory.GetTempFolder(packagePath))
          {
            var shortPath = Path.Combine(tempPath.Path, Path.GetFileName(path));
            
            var shortPkgPath = Path.Combine(tempPkg.Path, Path.GetFileName(packagePath));
            fileSystem.File.Copy(packagePath, shortPkgPath);

            base.UnpackZip(shortPkgPath, shortPath, entriesPattern, stepsCount, incrementProgress, skipErrors);
            this.fileSystem.Directory.Copy(shortPath, path, true);
          }
        }
      }
    }

    public override void UnpackZipWithActualWebRootName(string packagePath, string path, string webRootName, string entriesPattern = null, int stepsCount = 1, Action incrementProgress = null)
    {
      try
      {
        base.UnpackZipWithActualWebRootName(packagePath, path, webRootName, entriesPattern, stepsCount, incrementProgress);
      }
      catch (ZipException ex)
      {
        if (!(ex.InnerException is PathTooLongException))
        {
          throw;
        }

        throw new NotImplementedException("Not implemented yet");
      }
    }

    public override bool ZipContainsFile(string path, string innerFileName)
    {
      try
      {
        return base.ZipContainsFile(path, innerFileName);
      }
      catch (ZipException ex)
      {
        if (!(ex.InnerException is PathTooLongException))
        {
          throw;
        }
        using (var temp = FileSystem.Local.Directory.GetTempFolder(path))
        {
          var shortPath = Path.Combine(temp.Path, Path.GetFileName(path));
          fileSystem.File.Copy(path, shortPath);
          return base.ZipContainsFile(shortPath, innerFileName);
        }
      }
    }

    public override bool ZipContainsSingleFile(string path, string innerFileName)
    {
      try
      {
        return base.ZipContainsSingleFile(path, innerFileName);
      }
      catch (ZipException ex)
      {
        if (!(ex.InnerException is PathTooLongException))
        {
          throw;
        }
        using (var temp = FileSystem.Local.Directory.GetTempFolder(path))
        {
          var shortPath = Path.Combine(temp.Path, Path.GetFileName(path));
          fileSystem.File.Copy(path, shortPath);

        return base.ZipContainsSingleFile(path, innerFileName);
        }
      }
    }

    public override string ZipUnpackFile(string pathToZip, string pathToUnpack, string fileName)
    {
      try
      {
        return base.ZipUnpackFile(pathToZip, pathToUnpack, fileName);
      }
      catch (ZipException ex)
      {
        if (!(ex.InnerException is PathTooLongException))
        {
          throw;
        }
        using (var tempPath = FileSystem.Local.Directory.GetTempFolder(pathToUnpack))
        {
          using (var tempPkg = FileSystem.Local.Directory.GetTempFolder(pathToZip))
          {
            var shortPath = Path.Combine(tempPath.Path, Path.GetFileName(pathToUnpack));

            var shortPkgPath = Path.Combine(tempPkg.Path, Path.GetFileName(pathToZip));
            fileSystem.File.Copy(pathToZip, shortPkgPath);
            
            var result = base.ZipUnpackFile(shortPkgPath, shortPath, fileName);
            var destFileName = Path.Combine(pathToUnpack, fileName);
            fileSystem.File.Move(result, destFileName);

            return destFileName;
          }
        }
      }
    }

    public override string ZipUnpackFolder(string pathToZip, string pathToUnpack, string folderName)
    {
      try
      {
        return base.ZipUnpackFolder(pathToZip, pathToUnpack, folderName);
      }
      catch (ZipException ex)
      {
        if (!(ex.InnerException is PathTooLongException))
        {
          throw;
        }
        using (var tempPath = FileSystem.Local.Directory.GetTempFolder(pathToUnpack))
        {
          using (var tempPkg = FileSystem.Local.Directory.GetTempFolder(pathToZip))
          {
            var shortPath = Path.Combine(tempPath.Path, Path.GetFileName(pathToUnpack));

            var shortPkgPath = Path.Combine(tempPkg.Path, Path.GetFileName(pathToZip));
            fileSystem.File.Copy(pathToZip, shortPkgPath);

            var result = base.ZipUnpackFolder(shortPkgPath, shortPath, folderName);
            var destFileName = Path.Combine(pathToUnpack, folderName);
            fileSystem.Directory.Move(result, destFileName);

            return destFileName;
          }
        }
      }
    }
  }
}