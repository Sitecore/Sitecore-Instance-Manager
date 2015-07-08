#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIM.Adapters.SqlServer;
using SIM.Base;
using SIM.Instances;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

#endregion

namespace SIM.Tests.Instances
{
  [TestClass()]
  public class InstanceTest
  {
    [TestMethod]
    public void RootPathTest()
    {
      {
        var name = GetName();
        var root = GetRootFolder(name);
        var instance = new FakeInstance(GetRelativeFolder(root, "Website"), GetRelativeFolder(root, "Data"), GetDatabases(name, GetRelativeFolder(root, "Databases")));
        RootPathTest("#1 regular (root/website, root/data, root/databases)", root, instance);
      }
      {
        var name = GetName();
        var root = GetRootFolder(name);

        var instance = new FakeInstance(GetRelativeFolder(root, "Website"), GetRelativeFolder(root, "Website\\Data"), GetDatabases(name, GetRelativeFolder(root, "Databases")));
        RootPathTest("#2 data inside (root/website, root/website/data, root/databases)", root, instance);
      }
      {
        var name = GetName();
        var root = GetRootFolder(name);

        var instance = new FakeInstance(GetRelativeFolder(root, "Website"), GetRelativeFolder(root, "Website"), GetDatabases(name, GetRelativeFolder(root, "Databases")));
        RootPathTest("#3 data is website (root/website, root/website, root/databases)", root, instance);
      }
      {
        var name = GetName();
        var root = GetRootFolder(name);

        var instance = new FakeInstance(root, root, GetDatabases(name, root));
        RootPathTest("#4 all are root (root, root, root)", root, instance);
      }
      {
        var name = GetName();
        var root = GetRootFolder(name);
        var rootDrive = FileSystem.Local.Directory.GetDirectoryRoot(root.FullName);
        var drive = Environment.GetLogicalDrives().First(d => !d.EqualsIgnoreCase(rootDrive));
        var instance = new FakeInstance(GetRelativeFolder(root, "Website"), new DirectoryInfo(drive + "data"), GetDatabases(name, GetRelativeFolder(root, "Databases")));
        RootPathTest("#5 data on another drive (root/website, " + drive + "data, root/databases)", root, instance);
      }
      {
        var name = GetName();
        var root = GetRootFolder(name);
        var instance = new FakeInstance(GetRelativeFolder(root, "Website"), GetRelativeFolder(root.Parent, "data"), GetDatabases(name, GetRelativeFolder(root, "Databases")));
        RootPathTest("#6 data is too far, but databases are fine (root/website, root/../data, root/databases)", root, instance);
      }
      {
        var name = GetName();
        var root = GetRootFolder(name);
        var rootDrive = FileSystem.Local.Directory.GetDirectoryRoot(root.FullName);
        var drive = Environment.GetLogicalDrives().First(d => !d.EqualsIgnoreCase(rootDrive));
        var instance = new FakeInstance(GetRelativeFolder(root, "Website"), GetRelativeFolder(root, "Data"), GetDatabases(name, new DirectoryInfo(drive + "databases")));
        RootPathTest("#7 databases on another drive (root/website, root/data, " + drive + "databases)", root, instance);
      }
      {
        var name = GetName();
        var root = GetRootFolder(name);
        var rootDrive = FileSystem.Local.Directory.GetDirectoryRoot(root.FullName);
        var drive = Environment.GetLogicalDrives().First(d => !d.EqualsIgnoreCase(rootDrive));
        var instance = new FakeInstance(GetRelativeFolder(root, "Website"), GetRelativeFolder(root.Parent, "Data"), GetDatabases(name, new DirectoryInfo(drive + "databases")));
        RootPathTest("#8 databases on another drive, data too far (root/website, root/data, " + drive + "databases)", root, instance, typeof(InvalidConfigurationException));
      }
      {
        var name = GetName();
        var root = GetRootFolder(name);
        var rootDrive = FileSystem.Local.Directory.GetDirectoryRoot(root.FullName);
        var drive = Environment.GetLogicalDrives().First(d => !d.EqualsIgnoreCase(rootDrive));
        var instance = new FakeInstance(GetRelativeFolder(root, "Website"), GetRelativeFolder(root.Parent, "Data"), new Database[0]);
        RootPathTest("#9 no databases, data too far (root/website, root/data, " + drive + "databases)", root, instance, typeof(InvalidConfigurationException));
      }
      {
        var name = GetName();
        var root = GetRootFolder(name);
        var rootDrive = FileSystem.Local.Directory.GetDirectoryRoot(root.FullName);
        var drive = Environment.GetLogicalDrives().First(d => !d.EqualsIgnoreCase(rootDrive));
        var website = GetRelativeFolder(root, "Website");
        var instance = new FakeInstance(website, GetRelativeFolder(website, "Data"), new Database[0]);
        RootPathTest("#9 no databases, data is inside (root/website, root/website/data, " + drive + "databases)", website, instance);
      }
    }

    private void RootPathTest(string desc, DirectoryInfo root, Instance instance, Type exception = null)
    {
      try
      {
        string rootPath = instance.RootPath;
        if (exception != null) Assert.Fail("No {0} exception for {1}".FormatWith(exception.Name, desc));
        this.AreEqual(rootPath, root, desc);
      }
      catch(Exception ex)
      {
        if(exception == null) 
        {
          throw new Exception(desc, ex);
        }
        if(ex.GetType() != exception)
        {
          throw new Exception("Exception is {0} instead of expected {1} for {2}".FormatWith(ex.GetType().Name, exception.Name, desc));
        }
      }
      finally
      {
        root.Delete(true);
      }
    }

    private static Database[] GetDatabases(string name, DirectoryInfo databasesFolder)
    {
      return new Database[]
                 {
                   new FakeDatabase ("core", name + "Sitecore_Core", databasesFolder, "Sitecore.Core.mdf"),
                   new FakeDatabase ("master", name + "Sitecore_Master", databasesFolder, "Sitecore.Master.mdf"),
                   new FakeDatabase ("web", name + "Sitecore_Web", databasesFolder, "Sitecore.Web.mdf")
                 };
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
      if(FileSystem.Local.Directory.Exists(path)) return new DirectoryInfo(path);
      return FileSystem.Local.Directory.CreateDirectory(path);
    }

    private static DirectoryInfo GetRootFolder(string name)
    {
      var tmp = Path.GetTempPath();
      var root = FileSystem.Local.Directory.CreateDirectory(Path.Combine(tmp, name));
      return root;
    }
  }

  public class FakeDatabase : Database
  {
    private readonly string fileName;

    public FakeDatabase(string name, string realName, DirectoryInfo databasesFolder, string fileName)
    {
      this.Name = name;
      this.RealName = realName;
      this.fileName = Path.Combine(databasesFolder.FullName, fileName);
    }

    public override string FileName
    {
      get
      {
        return this.fileName;
      }
    }
  }

  public class FakeInstance : Instance
  {
    private readonly string dataFolderPath;
    private readonly Database[] getAttachedDatabases;
    private readonly string webRootPath;

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

    public override sealed string WebRootPath
    {
      get { return this.webRootPath; }
    }

    protected override string GetDataFolderPath()
    {
      return this.dataFolderPath;
    }

    protected override ICollection<Database> GetAttachedDatabases()
    {
      return this.getAttachedDatabases; 
    }
  }
}
