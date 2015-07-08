#region Usings

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIM.Base;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

#endregion

namespace SIM.Tests.Base
{
    
    
    /// <summary>
    ///This is a test class for FileSystemTest and is intended
    ///to contain all FileSystemTest Unit Tests
    ///</summary>
  [TestClass()]
  public class FileSystemTest
  {


    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

      #region Additional test attributes
      // 
      //You can use the following additional attributes as you write your tests:
      //
      //Use ClassInitialize to run code before running the first test in the class
      //[ClassInitialize()]
      //public static void MyClassInitialize(TestContext testContext)
      //{
      //}
      //
      //Use ClassCleanup to run code after all tests in a class have run
      //[ClassCleanup()]
      //public static void MyClassCleanup()
      //{
      //}
      //
      //Use TestInitialize to run code before running each test
      //[TestInitialize()]
      //public void MyTestInitialize()
      //{
      //}
      //
      //Use TestCleanup to run code after each test has run
      //[TestCleanup()]
      //public void MyTestCleanup()
      //{
      //}
      //
      #endregion

      /// <summary>
    ///A test for GetDistance
    ///</summary>
    [TestMethod()]
    public void GetDistanceTest()
    {
      string left;
      string right;
      int expected; // TODO: Initialize to an appropriate value
      int actual;

      left = "C:\\";
      right = "C:\\";
      expected = 0;
      actual = FileSystem.Local.Directory.GetDistance(left, right);
      Assert.AreEqual(expected, actual);

      //
      
      left = "C:\\";
      right = "C:\\Website";
      expected = 1;
      actual = FileSystem.Local.Directory.GetDistance(left, right);
      Assert.AreEqual(expected, actual);

      //

      left = "C:\\Website";
      right = "C:\\";
      expected = 1;
      actual = FileSystem.Local.Directory.GetDistance(left, right);
      Assert.AreEqual(expected, actual);

      //

      left = "C:\\Website\\";
      right = "C:\\";
      expected = 1;
      actual = FileSystem.Local.Directory.GetDistance(left, right);
      Assert.AreEqual(expected, actual);

      //

      left = "C:\\";
      right = "C:\\Website\\";
      expected = 1;
      actual = FileSystem.Local.Directory.GetDistance(left, right);
      Assert.AreEqual(expected, actual);

      //

      left = "C:\\";
      right = "C:\\Website\\Website";
      expected = 2;
      actual = FileSystem.Local.Directory.GetDistance(left, right);
      Assert.AreEqual(expected, actual);

      //

      left = "C:\\123";
      right = "C:\\Website";
      expected = 0;
      actual = FileSystem.Local.Directory.GetDistance(left, right);
      Assert.AreEqual(expected, actual);

      //

      left = "C:\\OtherWebsite";
      right = "C:\\Website\\OtherWebsite\\bin";
      expected = 2;
      actual = FileSystem.Local.Directory.GetDistance(left, right);
      Assert.AreEqual(expected, actual);

      //

      left = "C:\\Website\\OtherWebsite";
      right = "C:\\";
      expected = 2;
      actual = FileSystem.Local.Directory.GetDistance(left, right);
      Assert.AreEqual(expected, actual);

      //

      left  = @"C:\a\b\c\d\e\f\g\h\i\j\k\l";
      right = @"C:\a\b\c\d\1\2";
      expected = 8;
      actual = FileSystem.Local.Directory.GetDistance(left, right);
      Assert.AreEqual(expected, actual);

      //

      left = "D:\\";
      right = "C:\\Website\\";
      try
      {
        FileSystem.Local.Directory.GetDistance(left, right);
        Assert.Fail("The InvalidOperationException wasn't thorwn");
      }
      catch (InvalidOperationException)
      {
      }
    }

    /// <summary>
    ///A test for GetPathRoot
    ///</summary>
    [TestMethod()]
    public void GetPathRootTest()
    {
      string path; 
      string expected; 
      string actual;

      path = "C:\\Website\\OtherWebsite";
      expected = "C:\\";
      actual = FileSystem.Local.Directory.GetPathRoot(path);
      Assert.AreEqual(expected, actual);

      path = "D:\\Website\\OtherWebsite";
      expected = "D:\\";
      actual = FileSystem.Local.Directory.GetPathRoot(path);
      Assert.AreEqual(expected, actual);

      path = "C:";
      expected = "C:\\";
      actual = FileSystem.Local.Directory.GetPathRoot(path);
      Assert.AreEqual(expected, actual);
    }

    /// <summary>
    ///A test for HaveSameRoot
    ///</summary>
    [TestMethod()]
    public void HaveSameRootTest()
    {
      string directory1;
      string directory2;
      bool expected;
      bool actual;

      directory1 = "C:\\Website";
      directory2 = "C:\\OtherWebsite";
      expected = true;
      actual = FileSystem.Local.Directory.HaveSameRoot(directory1, directory2);
      Assert.AreEqual(expected, actual);

      directory1 = "C:\\OtherWebsite\\Website";
      directory2 = "C:\\OtherWebsite";
      expected = true;
      actual = FileSystem.Local.Directory.HaveSameRoot(directory1, directory2);
      Assert.AreEqual(expected, actual);

      directory1 = "C:\\Website";
      directory2 = "D:\\OtherWebsite";
      expected = false;
      actual = FileSystem.Local.Directory.HaveSameRoot(directory1, directory2);
      Assert.AreEqual(expected, actual);

      directory1 = "C:\\Website";
      directory2 = "\\OtherWebsite";
      expected = false;
      actual = FileSystem.Local.Directory.HaveSameRoot(directory1, directory2);
      Assert.AreEqual(expected, actual);
    }
  }
}
