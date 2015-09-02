namespace SIM.Tests.Base
{
  using System;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class XmldExTest
  {
    #region Fields

    private int iteration;

    #endregion

    #region Public methods

    [TestMethod]
    public void MergeTest()
    {
      string str1;
      string str2;
      string expected;

      // {
      // str1 = "<d />";
      // str2 = "<k />";
      // try
      // {
      // // here it should throw an exception
      // MergeTest(str1, str2, string.Empty);
      // Assert.Fail();
      // }
      // catch (Exception)
      // {
      // }
      // }
      this.iteration = 0;
      {
        str1 = "<d><n1>n1</n1></d>";
        str2 = "<d><n2>n2</n2></d>";
        expected = "<d><n1>n1</n1><n2>n2</n2></d>";
        this.MergeTest(str1, str2, expected);
      }
      {
        str1 = "<d><n1 a1=\"v1\">n1</n1></d>";
        str2 = "<d><n2 a2=\"v2\">n2</n2></d>";
        expected = "<d><n1 a1=\"v1\">n1</n1><n2 a2=\"v2\">n2</n2></d>";
        this.MergeTest(str1, str2, expected);
      }
      {
        str1 = "<d><n1 a1=\"v1\"><nn1>nn1</nn1></n1></d>";
        str2 = "<d><n1 a1=\"v1\"><nn2>nn2</nn2></n1></d>";
        expected = "<d><n1 a1=\"v1\"><nn1>nn1</nn1><nn2>nn2</nn2></n1></d>";
        this.MergeTest(str1, str2, expected);
      }
      {
        str1 = "<d><n1 a1=\"v1\"><nn1>nn1</nn1></n1></d>";
        str2 = "<d><n1 a2=\"v2\"><nn2>nn2</nn2></n1></d>";
        expected = "<d><n1 a1=\"v1\"><nn1>nn1</nn1></n1><n1 a2=\"v2\"><nn2>nn2</nn2></n1></d>";
        this.MergeTest(str1, str2, expected);
      }
      {
        str1 = "<d><n1 a1=\"v1\"><nn1 aa1=\"vv1\">nn1</nn1></n1></d>";
        str2 = "<d><n1 a1=\"v1\"><nn1 aa1=\"vv1\"><nnn2>nnn2</nnn2></nn1></n1></d>";
        expected = "<d><n1 a1=\"v1\"><nn1 aa1=\"vv1\">nn1<nnn2>nnn2</nnn2></nn1></n1></d>";
        this.MergeTest(str1, str2, expected);
      }
      {
        str1 = "<manifest version=\"1.4\"><package><name>Sitecore Web Forms for Marketers</name><install><postStepActions skipStandard=\"true\"><add type=\"Sitecore.Form.Core.Configuration.Installation, Sitecore.Forms.Core\" method=\"ChooseSQLiteVersionDll\" /></postStepActions><after><params><param name=\"{Restricting Placeholders}\" title=\"Please choose Restricting Placeholders\" defaultValue=\"content\" mode=\"multiselect\" getOptionsType=\"SIM.Pipelines.ConfigurationActions, SIM.Pipelines\" getOptionsMethod=\"GetPlaceholderNames\" /></params><actions><publish mode=\"incremental\" /><setRestrictingPlaceholders names=\"{Restricting Placeholders}\" /></actions></after></install></package></manifest>";
        str2 = "<manifest version=\"1.4\"><package /></manifest>";
        expected = str1;
        this.MergeTest(str1, str2, expected);
      }
    }

    [TestMethod]
    public void SetElementValueTest()
    {
      string expected;
      string path;
      string value = "some value";
      string str;
      {
        str = "<d><n1>n1</n1><n2>n2</n2></d>";
        path = "/d/n2";
        expected = "<d><n1>n1</n1><n2>some value</n2></d>";
        this.SetElementValueTest(str, path, value, expected);
      }
      {
        str = "<d><n1>n1</n1><n2>n2</n2></d>";
        path = "/d/n3";
        expected = "<d><n1>n1</n1><n2>n2</n2><n3>some value</n3></d>";
        this.SetElementValueTest(str, path, value, expected);
      }
      {
        str = "<d><n1>n1</n1><n2>n2</n2></d>";
        path = "d/n2";
        expected = "<d><n1>n1</n1><n2>some value</n2></d>";
        this.SetElementValueTest(str, path, value, expected);
      }
      {
        str = "<d><n1>n1</n1><n2>n2</n2></d>";
        path = "d/n3";
        expected = "<d><n1>n1</n1><n2>n2</n2><n3>some value</n3></d>";
        this.SetElementValueTest(str, path, value, expected);
      }
    }

    #endregion

    #region Private methods

    private void MergeTest(string str1, string str2, string expected)
    {
      this.iteration++;
      XmlDocumentEx doc1 = XmlDocumentEx.LoadXml(str1);
      XmlDocumentEx doc2 = XmlDocumentEx.LoadXml(str2);
      var actual = doc1.Merge(doc2);
      Assert.AreEqual(this.iteration + Environment.NewLine + expected + Environment.NewLine, this.iteration + Environment.NewLine + actual.OuterXml + Environment.NewLine);
    }

    private void SetElementValueTest(string str, string path, string value, string expected)
    {
      var xml = XmlDocumentEx.LoadXml(str);
      xml.SetElementValue(path, value);
      Assert.AreEqual(Environment.NewLine + expected + Environment.NewLine, Environment.NewLine + xml.OuterXml + Environment.NewLine);
    }

    #endregion
  }
}