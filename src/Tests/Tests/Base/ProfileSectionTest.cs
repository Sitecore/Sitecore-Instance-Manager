#region Usings

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIM.Base;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

#endregion

namespace SIM.Tests.Base
{
    
    
    /// <summary>
    ///This is a test class for ProfileSectionTest and is intended
    ///to contain all ProfileSectionTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ProfileSectionTest
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
    ///A test for PrettyPrint
    ///</summary>
    [TestMethod()]
    public void PrettyPrintTest()
    {
      double number;
      int left;
      int right;
      string expected;
      string actual;
      number = 9876.12345d;
      left = 2;
      right = 2;
      expected = "9876.12";
      actual = ProfileSection.PrettyPrint(number, left, right);
      Assert.AreEqual(expected, actual);
      number = 0.12345d;
      left = 2;
      right = 2;
      expected = "  0.12";
      actual = ProfileSection.PrettyPrint(number, left, right);
      Assert.AreEqual(expected, actual);
      number = -44.12345d;
      left = 4;
      right = 4;
      expected = "  -44.1234";
      actual = ProfileSection.PrettyPrint(number, left, right);
      Assert.AreEqual(expected, actual);
    }
  }
}
