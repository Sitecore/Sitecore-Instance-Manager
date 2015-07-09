namespace SIM.Tests.Base
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass()]
  public class ProfileSectionTest
  {
    #region Fields

    private TestContext testContextInstance;

    #endregion

    #region Public properties

    public TestContext TestContext
    {
      get
      {
        return this.testContextInstance;
      }

      set
      {
        this.testContextInstance = value;
      }
    }

    #endregion

    #region Additional test attributes

    // You can use the following additional attributes as you write your tests:
    // Use ClassInitialize to run code before running the first test in the class
    // [ClassInitialize()]
    // public static void MyClassInitialize(TestContext testContext)
    // {
    // }
    // Use ClassCleanup to run code after all tests in a class have run
    // [ClassCleanup()]
    // public static void MyClassCleanup()
    // {
    // }
    // Use TestInitialize to run code before running each test
    // [TestInitialize()]
    // public void MyTestInitialize()
    // {
    // }
    // Use TestCleanup to run code after each test has run
    // [TestCleanup()]
    // public void MyTestCleanup()
    // {
    // }
    #endregion

    #region Public methods

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

    #endregion
  }
}