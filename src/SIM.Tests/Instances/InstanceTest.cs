namespace SIM.Tests.Instances
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using SIM.Adapters.SqlServer;
  using SIM.Instances;

  [TestClass]
  public class InstanceTest
  {
    #region Public methods
    
    [TestMethod]
    public void RootPathTest1()
    {
      {
        var name = this.GetName();
        var root = GetRootFolder(name);
        var instance = new FakeInstance(this.GetRelativeFolder(root, "Website"), this.GetRelativeFolder(root, "Data"), GetDatabases(name, this.GetRelativeFolder(root, "Databases")));
        this.RootPathTest("#1 regular (root/website, root/data, root/databases)", root, instance);
      }
    }

    [TestMethod]
    public void RootPathTest2()
    {
      {
        var name = this.GetName();
        var root = GetRootFolder(name);

        var instance = new FakeInstance(this.GetRelativeFolder(root, "Website"), this.GetRelativeFolder(root, "Website\\Data"), GetDatabases(name, this.GetRelativeFolder(root, "Databases")));
        this.RootPathTest("#2 data inside (root/website, root/website/data, root/databases)", root, instance);
      }
    }

    [TestMethod]
    public void RootPathTest3()
    {
      {
        var name = this.GetName();
        var root = GetRootFolder(name);

        var instance = new FakeInstance(this.GetRelativeFolder(root, "Website"), this.GetRelativeFolder(root, "Website"), GetDatabases(name, this.GetRelativeFolder(root, "Databases")));
        this.RootPathTest("#3 data is website (root/website, root/website, root/databases)", root, instance);
      }
    }

    [TestMethod]
    public void RootPathTest4()
    {
      {
        var name = this.GetName();
        var root = GetRootFolder(name);

        var instance = new FakeInstance(root, root, GetDatabases(name, root));
        this.RootPathTest("#4 all are root (root, root, root)", root, instance);
      }
    }

    [TestMethod]
    public void RootPathTest5()
    {
      {
        var name = this.GetName();
        var root = GetRootFolder(name);
        var rootDrive = FileSystem.FileSystem.Local.Directory.GetDirectoryRoot(root.FullName);
        var drive = Environment.GetLogicalDrives().First(d => !d.EqualsIgnoreCase(rootDrive));
        var instance = new FakeInstance(this.GetRelativeFolder(root, "Website"), new DirectoryInfo(drive + "data"), GetDatabases(name, this.GetRelativeFolder(root, "Databases")));
        this.RootPathTest("#5 data on another drive (root/website, " + drive + "data, root/databases)", root, instance);
      }
    }

    [TestMethod]
    public void RootPathTest6()
    {
      {
        var name = this.GetName();
        var root = GetRootFolder(name);
        var instance = new FakeInstance(this.GetRelativeFolder(root, "Website"), this.GetRelativeFolder(root.Parent, "data"), GetDatabases(name, this.GetRelativeFolder(root, "Databases")));
        this.RootPathTest("#6 data is too far, but databases are fine (root/website, root/../data, root/databases)", root, instance);
      }
    }

    [TestMethod]
    public void RootPathTest7()
    {
      {
        var name = this.GetName();
        var root = GetRootFolder(name);
        var rootDrive = FileSystem.FileSystem.Local.Directory.GetDirectoryRoot(root.FullName);
        var drive = Environment.GetLogicalDrives().First(d => !d.EqualsIgnoreCase(rootDrive));
        var instance = new FakeInstance(this.GetRelativeFolder(root, "Website"), this.GetRelativeFolder(root, "Data"), GetDatabases(name, new DirectoryInfo(drive + "databases")));
        this.RootPathTest("#7 databases on another drive (root/website, root/data, " + drive + "databases)", root, instance);
      }
    }

    [TestMethod]
    public void RootPathTest8()
    {
      {
        var name = this.GetName();
        var root = GetRootFolder(name);
        var rootDrive = FileSystem.FileSystem.Local.Directory.GetDirectoryRoot(root.FullName);
        var drive = Environment.GetLogicalDrives().First(d => !d.EqualsIgnoreCase(rootDrive));
        var instance = new FakeInstance(this.GetRelativeFolder(root, "Website"), this.GetRelativeFolder(root.Parent, "Data"), GetDatabases(name, new DirectoryInfo(drive + "databases")));
        this.RootPathTest("#8 databases on another drive, data too far (root/website, root/data, " + drive + "databases)", root, instance);
      }
    }

    [TestMethod]
    public void RootPathTest9()
    {
      {
        var name = this.GetName();
        var root = GetRootFolder(name);
        var rootDrive = FileSystem.FileSystem.Local.Directory.GetDirectoryRoot(root.FullName);
        var drive = Environment.GetLogicalDrives().First(d => !d.EqualsIgnoreCase(rootDrive));
        var instance = new FakeInstance(this.GetRelativeFolder(root, "Website"), this.GetRelativeFolder(root.Parent, "Data"), new Database[0]);
        this.RootPathTest("#9 no databases, data too far (root/website, root/data, " + drive + "databases)", root, instance);
      }
    }

    [TestMethod]
    public void RootPathTest10()
    {
      {
        var name = this.GetName();
        var root = GetRootFolder(name);
        var rootDrive = FileSystem.FileSystem.Local.Directory.GetDirectoryRoot(root.FullName);
        var drive = Environment.GetLogicalDrives().First(d => !d.EqualsIgnoreCase(rootDrive));
        var website = this.GetRelativeFolder(root, "Website");
        var instance = new FakeInstance(website, this.GetRelativeFolder(website, "Data"), new Database[0]);
        this.RootPathTest("#9 no databases, data is inside (root/website, root/website/data, " + drive + "databases)", website, instance);
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

        this.AreEqual(rootPath, root, desc);
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

    private readonly string fileName;

    #endregion

    #region Constructors

    public FakeDatabase(string name, string realName, DirectoryInfo databasesFolder, string fileName)
    {
      this.Name = name;
      this.RealName = realName;
      this.fileName = Path.Combine(databasesFolder.FullName, fileName);
    }

    #endregion

    #region Public properties

    public override string FileName
    {
      get
      {
        return this.fileName;
      }
    }

    #endregion
  }

  public class FakeInstance : Instance
  {
    #region Fields

    private readonly string dataFolderPath;
    private readonly Database[] getAttachedDatabases;
    private readonly string webRootPath;

    #endregion

    #region Constructors

    public FakeInstance(string webRootPath, string dataFolderPath, Database[] databases)
      : base(0)
    {
      this.webRootPath = webRootPath;
      this.dataFolderPath = dataFolderPath;
      this.getAttachedDatabases = databases;
    }

    public FakeInstance(DirectoryInfo webroot, DirectoryInfo dataFolder, Database[] databases)
      : this(webroot.FullName, dataFolder.FullName, databases)
    {
    }

    #endregion

    #region Public properties

    public override sealed string WebRootPath
    {
      get
      {
        return this.webRootPath;
      }
    }

    #endregion

    #region Protected methods

    protected override ICollection<Database> GetAttachedDatabases()
    {
      return this.getAttachedDatabases;
    }

    protected override string GetDataFolderPath()
    {
      return this.dataFolderPath;
    }

    #endregion
  }
}