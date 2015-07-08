using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net.Config;
using SIM.Base;

namespace SIM.Tests
{
  public static class TestHelper
  {
    public static string LogFilePath
    {
      get { return Path.Combine(Environment.CurrentDirectory, "log.txt"); }
    }

    public static void Initialize()
    {
      XmlConfigurator.Configure(XmlDocumentEx.LoadXml(@"<log4net>
  <appender name=""LogFileAppender"" type=""log4net.Appender.FileAppender, log4net"">
    <file value=""log.txt"" />
    <layout type=""log4net.Layout.PatternLayout"">
      <conversionPattern value=""%4t %d{ABSOLUTE} %-5p %m%n"" />
    </layout>
    <filter type=""log4net.Filter.LevelRangeFilter"">
      <acceptOnMatch value=""true"" />
      <levelMin value=""DEBUG"" />
    </filter>
  </appender>
  <root>
    <priority value=""DEBUG"" />
    <appender-ref ref=""LogFileAppender"" />    
  </root>
</log4net>").DocumentElement);

      Log.Debug("Debug logging initialized");
    }

    public static void AreEqual(string actualText, string expectedText, string message)
    {
      if (!string.Equals(actualText, expectedText, StringComparison.OrdinalIgnoreCase))
      {
        actualText = actualText.Replace("\r\n", "\n");
        expectedText = expectedText.Replace("\r\n", "\n");
        var actualLines = actualText.Split('\n');
        var expectedLines = expectedText.Split('\n');
        var linesCount = Math.Min(actualLines.Length, expectedLines.Length);
        for (int lineNumber = 0; lineNumber < linesCount; ++lineNumber)
        {
          var actualLine = actualLines[lineNumber];
          var expectedLine = expectedLines[lineNumber];
          if (!string.Equals(actualLine, expectedLine, StringComparison.OrdinalIgnoreCase))
          {
            throw new Exception(message + @"

Line {0} (Actual): {1}
Line {0} (Expect): {2}
  
Log file: {3}".FormatWith(lineNumber + 1, actualLine, expectedLine, LogFilePath));
          }
        }
      }
    }

    public static void AreEqual(IComparer actual, IComparer expected, string message = null)
    {
      if (actual.Compare(actual, expected) != 0)
      {
        throw new Exception(@"Objects are different. Comment: {0}

Actual: {1}
Expect: {2}

Log file: {3}".FormatWith(message, actual, expected, LogFilePath));
      }
    }

    public static void AreEqual(XmlDocumentEx actual, XmlDocumentEx expected, string footerMessage = null)
    {
      var afp = actual.FilePath;
      if (string.IsNullOrEmpty(afp) || !File.Exists(afp))
      {
        afp = Path.GetTempFileName() + ".xml";
        actual.Save(afp);
      }
      var efp = expected.FilePath;
      if (string.IsNullOrEmpty(efp) || !File.Exists(efp))
      {
        efp = Path.GetTempFileName() + ".xml";
        expected.Save(efp);
      }

      AreEqual(XmlDocumentEx.Normalize(actual.OuterXml), XmlDocumentEx.Normalize(expected.OuterXml), @"Xml files are different
Path (Actual): {0}
Path (Expect): {1}

Compare CMD: 
WinMergeU ""{0}"" ""{1}""

{2}".FormatWith(afp, efp, footerMessage));
    }

    public static void AreEqual(IEnumerable<string> actual, IEnumerable<string> expected, string message = null)
    {
      var actualArray = actual.ToArray();
      var expectedArray = expected.ToArray();
      var expectedLength = expectedArray.Length;
      var actualLength = actualArray.Length;
      var n = Math.Min(actualLength, expectedLength);
      for (int itemNumber = 0; itemNumber < n; ++itemNumber)
      {
        var actualElement = actualArray[itemNumber];
        var expectedElement = expectedArray[itemNumber];
        if (!string.Equals(actualElement, expectedElement, StringComparison.OrdinalIgnoreCase))
        {
          throw new Exception(@"IEnumerables are different. Comment: {6}

Item {0} (Actual): {1}
Item {0} (Expect): {2}

Size (Actual): {4}
Size (Expect): {5}

Log file: {3}".FormatWith(itemNumber + 1, actualElement, expectedElement, Path.Combine(Environment.CurrentDirectory, "log.txt"), actualLength, expectedLength, message));
        }
      }

      if (actualLength != expectedLength)
      {
        var lines = string.Join(Environment.NewLine, (actualLength > expectedLength ? actualArray : expectedArray).Skip(n));
        throw new Exception(@"IEnumerables are different. Comment: {0}

Size (Actual): {1}
Size (Expect): {2}

{3} lines: 
{4}

Log file: {5}".FormatWith(message, actualLength, expectedLength, actualLength > expectedLength ? "Spare" : "Missed", lines, Path.Combine(Environment.CurrentDirectory, "log.txt")));

      }
    }

    public static void MustFail(Action action)
    {
      try
      {
        action();
        throw new Exception("Method did not throw an exception, but it was expected to.");
      }
      catch
      {
      }
    }
  }
}
