﻿namespace SIM.Tests.Instances
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using SIM.Adapters.SqlServer;
  using SIM.Extensions;
  using SIM.Instances;

  [TestClass]
  public class InstanceTest
  {
    #region Public methods
    
    [TestMethod]
    public void DataAndWebsiteOnSameLevelTest()
    {
      {
        var name = GetName();
        var root = GetRootFolder(name);
        var instance = new FakeInstance(GetRelativeFolder(root, "Website"), GetRelativeFolder(root, "Data"), GetDatabases(name, GetRelativeFolder(root, "Databases")));
        RootPathTest("#1 regular (root/website, root/data, root/databases)", root, instance);
      }
    }

    [TestMethod]
    public void DataInsideWebsiteTest()
    {
      {
        var name = GetName();
        var root = GetRootFolder(name);

        var instance = new FakeInstance(GetRelativeFolder(root, "Website"), GetRelativeFolder(root, "Website\\Data"), GetDatabases(name, GetRelativeFolder(root, "Databases")));
        RootPathTest("#2 data inside (root/website, root/website/data, root/databases)", root, instance);
      }
    }

    [TestMethod]
    public void DataIsWebsiteTest()
    {
      {
        var name = GetName();
        var root = GetRootFolder(name);

        var instance = new FakeInstance(GetRelativeFolder(root, "Website"), GetRelativeFolder(root, "Website"), GetDatabases(name, GetRelativeFolder(root, "Databases")));
        RootPathTest("#3 data is website (root/website, root/website, root/databases)", root, instance);
      }
    }

    [TestMethod]
    public void DataAndWebsiteAreRootTest()
    {
      {
        var name = GetName();
        var root = GetRootFolder(name);

        var instance = new FakeInstance(root, root, GetDatabases(name, root), name);
        RootPathTest("#4 all are root (root, root, root)", root, instance);
      }
    }

    [TestMethod]
    public void DataOnAnotherDriveTest()
    {
      {
        var name = GetName();
        var root = GetRootFolder(name);
        var rootDrive = FileSystem.FileSystem.Local.Directory.GetDirectoryRoot(root.FullName);
        var drive = Environment.GetLogicalDrives().FirstOrDefault(d => !d.EqualsIgnoreCase(rootDrive));

        //This test can be executed only if there are more than 1 drive on the machine.
        //If there is just 1 drive, the test is skipped.
        if (drive == null)
          return;

        var instance = new FakeInstance(GetRelativeFolder(root, "Website"), new DirectoryInfo(drive + "data"), GetDatabases(name, GetRelativeFolder(root, "Databases")));
        RootPathTest($"#5 data on another drive (root/website, {drive}data, root/databases)", root, instance);
      }
    }

    [TestMethod]
    public void DataOutsideRootTest()
    {
      {
        var name = GetName();
        var root = GetRootFolder(name);
        var instance = new FakeInstance(GetRelativeFolder(root, "Website"), GetRelativeFolder(root.Parent, "data"), GetDatabases(name, GetRelativeFolder(root, "Databases")));
        RootPathTest("#6 data is too far, but databases are fine (root/website, root/../data, root/databases)", root, instance);
      }
    }

    [TestMethod]
    public void DatabasesOnAnotherDriveTest()
    {
      {
        var name = GetName();
        var root = GetRootFolder(name);
        var rootDrive = FileSystem.FileSystem.Local.Directory.GetDirectoryRoot(root.FullName);
        var drive = Environment.GetLogicalDrives().FirstOrDefault(d => !d.EqualsIgnoreCase(rootDrive));

        //This test can be executed only if there are more than 1 drive on the machine.
        //If there is just 1 drive, the test is skipped.
        if (drive == null)
          return;

        var instance = new FakeInstance(GetRelativeFolder(root, "Website"), GetRelativeFolder(root, "Data"), GetDatabases(name, new DirectoryInfo(drive + "databases")));
        RootPathTest($"#7 databases on another drive (root/website, root/data, {drive}databases)", root, instance);
      }
    }

    [TestMethod]
    public void DatabasesOnAnotherDriveDataOtsideRootTest()
    {
      {
        var name = GetName();
        var root = GetRootFolder(name);
        var rootDrive = FileSystem.FileSystem.Local.Directory.GetDirectoryRoot(root.FullName);
        var drive = Environment.GetLogicalDrives().FirstOrDefault(d => !d.EqualsIgnoreCase(rootDrive));

        //This test can be executed only if there are more than 1 drive on the machine.
        //If there is just 1 drive, the test is skipped.
        if (drive == null)
          return;

        var instance = new FakeInstance(GetRelativeFolder(root, "Website"), GetRelativeFolder(root.Parent, "Data"), GetDatabases(name, new DirectoryInfo(drive + "databases")));
        RootPathTest($"#8 databases on another drive, data too far (root/website, root/data, {drive}databases)", root, instance);
      }
    }

    [TestMethod]
    public void NoDatabasesDataOutsideRootTest()
    {
      {
        var name = GetName();
        var root = GetRootFolder(name);
        var instance = new FakeInstance(GetRelativeFolder(root, "Website"), GetRelativeFolder(root.Parent, "Data"), new Database[0]);
        RootPathTest($"#9 no databases, data too far (root/website, root/data, no databases)", root, instance);
      }
    }

    [TestMethod]
    public void NoDatabasesDataInsideWebsiteTest()
    {
      {
        var name = GetName();
        var root = GetRootFolder(name);
        var website = GetRelativeFolder(root, "Website");
        var instance = new FakeInstance(website, GetRelativeFolder(website, "Data"), new Database[0]);
        RootPathTest($"#10 no databases, data is inside (root/website, root/website/data, no databases)", website, instance);
      }
    }

    #endregion

    #region Private methods

    private static Database[] GetDatabases(string name, DirectoryInfo databasesFolder)
    {
      return new Database[]
      {
        new FakeDatabase("core", name + "Sitecore_Core", databasesFolder, "Sitecore.Core.mdf"), 
        new FakeDatabase("master", name + "Sitecore_Master", databasesFolder, "Sitecore.Master.mdf"), 
        new FakeDatabase("web", name + "Sitecore_Web", databasesFolder, "Sitecore.Web.mdf")
      };
    }

    private static DirectoryInfo GetRootFolder(string name)
    {
      var tmp = Path.GetTempPath();
      var root = FileSystem.FileSystem.Local.Directory.CreateDirectory(Path.Combine(tmp, name));
      return root;
    }

    private void AreEqual(string path1, DirectoryInfo path2, string desc)
    {
      Assert.AreEqual(path1.TrimEnd('\\', '/'), path2.FullName.TrimEnd('\\', '/'), true, desc);
    }

    private string GetName()
    {
      return Guid.NewGuid().ToString();
    }

    private DirectoryInfo GetRelativeFolder(DirectoryInfo root, string website)
    {
      var path = Path.Combine(root.FullName, website);
      if (FileSystem.FileSystem.Local.Directory.Exists(path))
      {
        return new DirectoryInfo(path);
      }

      return FileSystem.FileSystem.Local.Directory.CreateDirectory(path);
    }

    private void RootPathTest(string desc, DirectoryInfo root, Instance instance, Type exception = null)
    {
      try
      {
        string rootPath = instance.RootPath;
        if (exception != null)
        {
          Assert.Fail("No {0} exception for {1}".FormatWith(exception.Name, desc));
        }

        AreEqual(rootPath, root, desc);
      }
      catch (Exception ex)
      {
        if (exception == null)
        {
          throw new Exception(desc, ex);
        }

        if (ex.GetType() != exception)
        {
          throw new Exception("Exception is {0} instead of expected {1} for {2}".FormatWith(ex.GetType().Name, exception.Name, desc), ex);
        }
      }
      finally
      {
        root.Delete(true);
      }
    }

    #endregion
  }

  public class FakeDatabase : Database
  {
    #region Fields

    private string fileName { get; }

    #endregion

    #region Constructors

    public FakeDatabase(string name, string realName, DirectoryInfo databasesFolder, string fileName)
    {
      Name = name;
      RealName = realName;
      this.fileName = Path.Combine(databasesFolder.FullName, fileName);
    }

    #endregion

    #region Public properties

    public override string FileName
    {
      get
      {
        return fileName;
      }
    }

    #endregion
  }

  public class FakeInstance : Instance
  {
    #region Fields

    private string dataFolderPath { get; }
    private readonly Database[] _GetAttachedDatabases;
    private string webRootPath { get; }

    #endregion

    #region Constructors

    public FakeInstance(string webRootPath, string dataFolderPath, Database[] databases)
      : base(0)
    {
      this.webRootPath = webRootPath;
      this.dataFolderPath = dataFolderPath;
      _GetAttachedDatabases = databases;
    }

    public FakeInstance(DirectoryInfo webroot, DirectoryInfo dataFolder, 
      Database[] databases, string displayName = null)
      : this(webroot.FullName, dataFolder.FullName, databases)
    {
      DisplayName = $"FakeInstance-{displayName ?? webroot.Parent.Name}";
    }

    #endregion

    #region Public properties

    public override sealed string WebRootPath
    {
      get
      {
        return webRootPath;
      }
    }

    public sealed override string DisplayName { get; }

    #endregion

    #region Protected methods

    protected override ICollection<Database> GetAttachedDatabases()
    {
      return _GetAttachedDatabases;
    }

    protected override string GetDataFolderPath()
    {
      return dataFolderPath;
    }

    #endregion
  }
}