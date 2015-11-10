using System;
using System.IO;
using Ionic.Zip;
using Sitecore.Diagnostics.Base;
using Sitecore.Diagnostics.Base.Annotations;

namespace SIM.FileSystem
{
  public class UncZipProvider : ZipProvider
  {
    #region Fields

    [NotNull]
    private readonly FileSystem fileSystem;

    #endregion

    #region Constructors

    public UncZipProvider([NotNull] FileSystem fileSystem) : base(fileSystem)
    {
      Assert.ArgumentNotNull(fileSystem, "fileSystem");

      this.fileSystem = fileSystem;
    }

    #endregion

    #region Public methods

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
          this.fileSystem.File.Copy(path, shortPath);

          base.CheckZip(shortPath);
        }
      }
    }

    public override void CreateZip(string path, string zipFileName, string ignore = null, int compressionLevel = 0)
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
            this.fileSystem.Directory.Copy(path, shortPath, true);

            var shortZipPath = Path.Combine(tempZip.Path, Path.GetFileName(zipFileName));
            base.CreateZip(shortPath, shortZipPath, ignore);

            this.fileSystem.File.Copy(shortZipPath, zipFileName);
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
          this.fileSystem.File.Copy(path, shortPath);

          return base.GetFirstRootFolder(shortPath);
        }
      }
    }
    
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
            this.fileSystem.File.Copy(packagePath, shortPkgPath);

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
          this.fileSystem.File.Copy(path, shortPath);
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
          this.fileSystem.File.Copy(path, shortPath);

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
            this.fileSystem.File.Copy(pathToZip, shortPkgPath);

            var result = base.ZipUnpackFile(shortPkgPath, shortPath, fileName);
            var destFileName = Path.Combine(pathToUnpack, fileName);
            this.fileSystem.File.Move(result, destFileName);

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
            this.fileSystem.File.Copy(pathToZip, shortPkgPath);

            var result = base.ZipUnpackFolder(shortPkgPath, shortPath, folderName);
            var destFileName = Path.Combine(pathToUnpack, folderName);
            this.fileSystem.Directory.Move(result, destFileName);

            return destFileName;
          }
        }
      }
    }

    #endregion
  }
}