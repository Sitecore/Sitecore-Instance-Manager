namespace SIM.Tests.Instances
{
  using System.Linq;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using SIM.Instances;

  [TestClass]
  public class InstanceHelperTests
  {
    #region Public methods

    [TestMethod]
    public void GetLogGroupsTest()
    {
      var files = new[]
      {
        "C:\\Crawling.log.20140905.135957.txt",
        "D:\\Crawling.log.20140905.txt",
        "log.20140905.135957.txt",
        "E:\\assaasd\\log.20140905.txt",
        "readme.12324.txt",
        "Search.log.20140905.135957.txt",
        "Search.log.20140905.txt",
        "WebDAV.log.20140905.135957.txt",
        "WebDAV.log.20140905.txt"
      };
      var results = InstanceHelper.GetLogGroups(files).OrderBy(x => x).Select(x => x.ToLower()).ToArray();
      var expected = new[]
      {
        "crawling.log", "log", "search.log", "webdav.log"
      }.OrderBy(x => x).ToArray();
      Assert.AreEqual(results.Count(), expected.Count());
      for (int i = 0; i < expected.Count(); ++i)
      {
        Assert.AreEqual(expected[i], results[i]);
      }
    }

    #endregion
  }
}