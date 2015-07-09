using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using System.Xml;

namespace SIM
{
  public class ProfileSection : IDisposable
  {
    #region Constants

    private const string End = "{0}ms    ";
    public const string Start = "              ";

    #endregion

    #region Fields

    private static int instancesCount;
    private readonly string message;
    private readonly Stopwatch stopwatch = new Stopwatch();

    #endregion

    #region Constructors

    public ProfileSection(string message, object caller)
    {
      instancesCount++;
      this.message = message;
      var move = ShiftLog("-> ");
      var pattern = Start + move + "{0}";
      var str = pattern.FormatWith(this.message);
      Log.Debug(str, null, true);
      if (caller != null)
      {
        var type = caller as Type;
        if (type != null)
        {
          caller = "{0} (static class)".FormatWith(type.FullName);
        }

        // Log.Debug("Caller: {0}".FormatWith(caller.ToString()));
      }

      this.stopwatch.Start();
    }

    #endregion

    #region IDisposable Members

    public static string ShiftLog(string sign = null)
    {
      string move = string.Empty;
      for (int i = 0; i < instancesCount; ++i)
      {
        move += sign ?? "   ";
      }

      return move;
    }

    public virtual void Dispose()
    {
      this.stopwatch.Stop();
      var move = ShiftLog("-  ");
      string pattern = End + move + "{1}";
      Log.Debug(pattern.FormatWith(PrettyPrint(this.stopwatch.Elapsed.TotalMilliseconds, 4, 2), this.message), null, true);
      instancesCount--;
    }

    #endregion

    #region Public methods

    public static void Argument(string name, object value)
    {
      var text = GetValueInfoText(value);

      // Log.Debug("Argument {0}: {1}".FormatWith(name, text));
      Log.Debug("params {0}: {1}".FormatWith(name, text));
    }

    public static string PrettyPrint(double i, int left, int right)
    {
      string str = i.ToString(CultureInfo.InvariantCulture);
      var minus = str[0] == '-';
      if (minus)
      {
        str = str.Substring(1);
      }

      var dot = str.IndexOfAny(new[]
      {
        '.', ','
      });
      if (right >= 0 && str.Length - dot > 2)
      {
        str = str.Substring(0, dot + right + 1);
      }

      if (left >= 0)
      {
        var diff = dot - left;
        if (diff < 0)
        {
          str = (minus ? '-' : ' ') + str;
          while (diff < 0)
          {
            str = ' ' + str;
            diff++;
          }
        }
      }

      return str;
    }

    public static T Result<T>(T value)
    {
      var text = GetValueInfoText(value);

      Log.Debug("return {0}".FormatWith(text));

      return value;
    }

    #endregion

    #region Private methods

    private static string GetValueInfoText(object value)
    {
      if (value == null)
      {
        return "null";
      }

      var str = value as string;
      if (str != null)
      {
        return "\"{0}\"".FormatWith(str);
      }

      var collection = value as ICollection;
      if (collection != null)
      {
        return "ICollection, " + collection.Count + " elements" + (collection as IEnumerable<string>).With(x => ": \"" + string.Join("\", \"", x).TrimEnd(", \""));
      }

      var identity = value as IdentityReference;
      if (identity != null)
      {
        return identity.Value + " (IdentityReference)";
      }

      var element = value as XmlElement;
      if (element != null)
      {
        return "XmlElement, XPath: \"" + element.GetXPath() + "\"" + (element.OwnerDocument as XmlDocumentEx).With(x => "; FilePath: " + x.FilePath);
      }

      var document = value as XmlDocument;
      if (document != null)
      {
        return "XmlDocument" + (document as XmlDocumentEx).With(x => ", FilePath: \"" + x.FilePath + "\"");
      }

      return "{0} ({1})".FormatWith(value, value.GetType().FullName);
    }

    #endregion
  }
}