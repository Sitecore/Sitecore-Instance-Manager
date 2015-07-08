using System.Linq;
using SIM.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace SIM.Tests.Base
{
  [TestClass]
  public class ExtensionsTest
  {
    [TestMethod]
    public void ExtractTest()
    {
      string message;
      char startChar = '{';
      char endChar = '}';
      bool includeBounds;
      string[] expected;
      string[] actual;

      message = "some text test ololo";
      expected = new string[0];
      includeBounds = false;
      actual = message.Extract(startChar, endChar, includeBounds).ToArray();
      AreEqual(expected, actual);

      message = "some text test ololo";
      expected = new string[0];
      includeBounds = true;
      actual = message.Extract(startChar, endChar, includeBounds).ToArray();
      AreEqual(expected, actual);

      message = "some text {test} ololo";
      expected = new[] { "test" };
      includeBounds = false;
      actual = message.Extract(startChar, endChar, includeBounds).ToArray();
      AreEqual(expected, actual);

      message = "some text {test} ololo";
      expected = new[] { "{test}" };
      includeBounds = true;
      actual = message.Extract(startChar, endChar, includeBounds).ToArray();
      AreEqual(expected, actual);

      message = "some text {test} ololo {test2}";
      expected = new[] { "test", "test2" };
      includeBounds = false;
      actual = message.Extract(startChar, endChar, includeBounds).ToArray();
      AreEqual(expected, actual);

      message = "some text {test} ololo {test2}";
      expected = new[] { "{test}", "{test2}" };
      includeBounds = true;
      actual = message.Extract(startChar, endChar, includeBounds).ToArray();
      AreEqual(expected, actual);

      try
      {
        message = "some text {test} ololo{";
        message.Extract(startChar, endChar, includeBounds).ToArray();
        Assert.Fail("the exception must have been thrown #1");
      }
      catch (Exception)
      {
      }

      try
      {
        message = "some text {test} ololo}";
        message.Extract(startChar, endChar, includeBounds).ToArray();
        Assert.Fail("the exception must have been thrown #2");
      }
      catch (Exception)
      {
      }

      try
      {
        message = "some text {test} ololo{}";
        message.Extract(startChar, endChar, includeBounds).ToArray();
        Assert.Fail("the exception must have been thrown #3");
      }
      catch (Exception)
      {
      }
    }

    [TestMethod]
    public void TestSplit()
    {
      {
        var str = "abcdef";
        var res = str.Split("g");
        Assert.AreEqual(str, res.SingleOrDefault());
      }
      {
        var str = "abLOLcdLOLOLOLefLOL";
        var res = str.Split("LOL").ToArray();
        Assert.AreEqual("ab", res[0]);
        Assert.AreEqual("cd", res[1]);
        Assert.AreEqual("O", res[2]);
        Assert.AreEqual("ef", res[3]);
      }
      {
        var str = "abLOLcdLOLLOLefLOL";
        var res = str.Split("LOL", false).ToArray();
        Assert.AreEqual("ab", res[0]);
        Assert.AreEqual("cd", res[1]);
        Assert.AreEqual("", res[2]);
        Assert.AreEqual("ef", res[3]);
        Assert.AreEqual("", res[4]);
      }
    }

    [TestMethod()]
    public void JoinTest()
    {
      var arr = new[] { 1, 2, 3 };
      string actual;
      {
        actual = arr.Join(", ");
        Assert.AreEqual("1, 2, 3", actual);
      }
      {
        actual = arr.Join(", ", "[", "]");
        Assert.AreEqual("[1], [2], [3]", actual);
      }
      {
        actual = arr.Join(", ", "[", "]", "{", "}");
        Assert.AreEqual("{[1], [2], [3]}", actual);
      }
    }

    [TestMethod]
    public void TrimStartTest()
    {
      const string source = "hellodearhellodeahelou";
      Assert.AreEqual("deahelou", source.TrimStart("hello", "dear"));
    }

    private void AreEqual(string[] expected, string[] actual)
    {
      Assert.AreEqual(expected.Length, actual.Length, "length are different");
      for (int i = 0; i < expected.Length; ++i)
      {
        string actualElement = actual[i];
        string expectedElement = expected[i];
        Assert.AreEqual(expectedElement, actualElement, "elements number {0} are different: expected '{1}' vs actual '{2}'".FormatWith(i, expectedElement, actualElement));
      }
    }    
  }
}
