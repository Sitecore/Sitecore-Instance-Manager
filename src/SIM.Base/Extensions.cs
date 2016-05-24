namespace SIM
{
  #region

  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Xml;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  #endregion

  public static class Extensions
  {
    #region Public Methods

    public static bool BaseTypesContain(this Type type, Type value)
    {
      while (type != null && type != typeof(object))
      {
        if (type.BaseType == value)
        {
          return true;
        }

        type = type.BaseType;
      }

      return false;
    }

    public static bool ContainsIgnoreCase(this string source, string target)
    {
      return source == null || target == null ? source == target : source.ToLower().Contains(target.ToLower());
    }

    [CanBeNull]
    public static string EmptyToNull([CanBeNull] this string source)
    {
      return string.IsNullOrEmpty(source) ? null : source;
    }

    public static bool EqualsIgnoreCase(this string source, string value, bool ignoreSpaces = false)
    {
      return ignoreSpaces ? string.Equals(source.Replace(" ", string.Empty), value.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase) : string.Equals(source, value, StringComparison.OrdinalIgnoreCase);
    }

    public static IEnumerable<string> Extract(this string message, char startChar, char endChar, bool includeBounds)
    {
      int end = -1;
      int length = message.Length;
      while (true)
      {
        int start = message.IndexOf(startChar, end + 1);
        if (start < 0)
        {
          yield break;
        }

        Assert.IsTrue(start + 1 < length, "Cannot replace variables in the \"{0}\" string - line unexpectedly ends after {1} position", message, start);
        end = message.IndexOf(endChar, start + 1);
        Assert.IsTrue(end > start, "Cannot replace variables in the \"{0}\" string - no closing {1} character after {2} position", message, end, start);
        start += includeBounds ? 0 : 1;
        int len = end - start + (includeBounds ? 1 : 0);
        Assert.IsTrue(len > 0, "Cannot replace variables in the \"{0}\" string - the string contains invalid '{{}}' statement", message);
        yield return message.Substring(start, len);
      }
    }

    [NotNull]
    public static string FormatWith([CanBeNull] this string format, [CanBeNull] params object[] parameters)
    {
      return parameters == null ? format ?? string.Empty : string.Format(format ?? string.Empty, parameters);
    }

    public static string GetNonEmptyAttribute(this XmlElement element, string name)
    {
      return element.GetAttribute(name).EmptyToNull().IsNotNull("{0} doesn't have the {1} attribute filled in".FormatWith(element.OuterXml, name));
    }

    [CanBeNull]
    public static string GetXPath(this XmlElement element)
    {
      string result = string.Empty;
      XmlElement iterator = element;
      while (iterator != null)
      {
        result = iterator.Name + '/' + result;
        iterator = iterator.ParentNode as XmlElement;
      }

      return result.TrimEnd('/');
    }

    [NotNull]
    public static T1 IsNotNull<T1>([CanBeNull] this T1 source, [CanBeNull] string message = null) where T1 : class
    {
      Assert.IsNotNull(source, message ?? "Value is null");

      return source;
    }

    public static bool IsNull([CanBeNull] this object @object)
    {
      return @object == null;
    }

    public static bool IsNullOrEmpty([CanBeNull] this string @string)
    {
      return string.IsNullOrEmpty(@string);
    }

    [NotNull]
    public static T1 IsTrue<T1>([CanBeNull] this T1 source, [NotNull] Func<T1, bool> act, [NotNull] string message) where T1 : class
    {
      Assert.ArgumentNotNull(act, "act");
      Assert.ArgumentNotNull(message, "message");

      Assert.IsNotNull(source, message);
      Assert.IsTrue(act(source), message);

      return source;
    }

    [NotNull]
    public static IEnumerable<T1> NotNull<T1>([CanBeNull] this IEnumerable<T1> source) where T1 : class
    {
      return source.Where(item => item != null);
    }

    public static bool NullOrEmpty<T>(this ICollection<T> arr)
    {
      return arr.NullOrTrue(x => x.Count == 0);
    }

    public static bool NullOrTrue<T>(this T arr, Func<T, bool> condition) where T : class
    {
      if (arr == null)
      {
        return true;
      }

      return condition(arr);
    }

    [CanBeNull]
    public static T SafeCall<T>(this object obj, Func<T> func) where T : class
    {
      try
      {
        return func();
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "SafeCall of the {0} method failed", func.ToString());
        return null;
      }
    }

    [CanBeNull]
    public static IEnumerable<XmlElement> SelectElements(this XmlDocument document, string xpath)
    {
      return document.SelectNodes(xpath).OfType<XmlElement>();
    }

    [CanBeNull]
    public static IEnumerable<XmlElement> SelectElements(this XmlElement element, string xpath)
    {
      return element.SelectNodes(xpath).OfType<XmlElement>();
    }

    [CanBeNull]
    public static XmlElement SelectSingleElement(this XmlDocument document, string xpath)
    {
      return document.SelectSingleNode(xpath) as XmlElement;
    }

    [CanBeNull]
    public static XmlElement SelectSingleElement(this XmlElement element, string xpath)
    {
      return element.SelectSingleNode(xpath) as XmlElement;
    }

    [NotNull]
    public static IEnumerable<string> Split([NotNull] this string source, [NotNull] string delimiter, bool skipEmpty = true)
    {
      Assert.ArgumentNotNullOrEmpty(source, "source");
      Assert.ArgumentNotNullOrEmpty(delimiter, "delimiter");

      var delimiterLength = delimiter.Length;
      var oldPos = 0;
      while (true)
      {
        var pos = source.IndexOf(delimiter, oldPos, System.StringComparison.Ordinal);
        if (pos < 0)
        {
          if (oldPos <= 0)
          {
            yield return source;
          }

          yield break;
        }

        var len = pos - oldPos;
        if (!skipEmpty || len > 0)
        {
          yield return source.Substring(oldPos, len);
        }

        oldPos = pos + delimiterLength;
        if (oldPos >= source.Length)
        {
          if (!skipEmpty && oldPos == source.Length)
          {
            yield return string.Empty;
          }

          yield break;
        }
      }
    }

    [NotNull]
    public static string TrimEnd([NotNull] this string source, [NotNull] string value)
    {
      Assert.ArgumentNotNull(source, "source");
      Assert.ArgumentNotNull(value, "value");

      return source.EndsWith(value) ? source.Substring(0, source.Length - value.Length) : source;
    }

    public static string TrimStart(this string source, params string[] prefixes)
    {
      bool atLeastOne;
      do
      {
        atLeastOne = false;
        foreach (var prefix in prefixes)
        {
          while (source.Length >= prefix.Length && source.StartsWith(prefix))
          {
            atLeastOne = true;
            source = source.Length > prefix.Length ? source.Substring(prefix.Length) : string.Empty;
          }
        }
      }
      while (atLeastOne);

      return source;
    }

    [NotNull]
    public static string TrimStart([NotNull] this string source, [NotNull] string value)
    {
      Assert.ArgumentNotNull(source, "source");
      Assert.ArgumentNotNull(value, "value");

      return source.StartsWith(value) ? source.Substring(value.Length) : source;
    }

    #endregion

    // Monad maybe
    #region Public methods

    public static IEnumerable<T> Add<T>(this IEnumerable<T> @this, T item)
    {
      foreach (var obj in @this)
      {
        yield return obj;
      }

      yield return item;
    }

    public static XmlElement AddElement(this XmlNode node, string name)
    {
      var element = node.OwnerDocument.CreateElement(name);
      node.AppendChild(element);
      return element;
    }

    public static string EnsureEnd(this string @this, string end)
    {
      return @this.TrimEnd(end) + end;
    }

    public static string GetComparable(this string @this)
    {
      return @this.ToLower().Replace(" ", string.Empty);
    }

    public static IEnumerable<T> Insert<T>(this IEnumerable<T> @this, int position, T item)
    {
      var enumerator = @this.GetEnumerator();
      for (var i = 0; i < position && enumerator.MoveNext(); ++i)
      {
        yield return enumerator.Current;
      }

      yield return item;

      while (enumerator.MoveNext())
      {
        yield return enumerator.Current;
      }
    }

    public static string Join<T>(this IEnumerable<T> @this, string separator)
    {
      return @this.Join(separator, string.Empty, string.Empty);
    }

    public static string Join<T>(this IEnumerable<T> @this, string separator, string wrapperLeft, string wrapperRight)
    {
      return @this.Join(separator, wrapperLeft, wrapperRight, string.Empty, string.Empty);
    }

    public static string Join<T>(this IEnumerable<T> @this, string separator, string wrapperLeft, string wrapperRight, string before, string after)
    {
      return before + @this.Aggregate(string.Empty, (sentense, item) => sentense + separator + wrapperLeft + item + wrapperRight).TrimStart(separator) + after;
    }

    // String extensions
    public static string PathCombine(this string @this, string combineWith)
    {
      return Path.Combine(@this, combineWith);
    }

    public static string Replace(this string @this, string source, char target)
    {
      return @this.Replace(source, target.ToString());
    }

    public static string Replace(this string @this, char source, string target)
    {
      return @this.Replace(source.ToString(), target);
    }

    [NotNull]
    public static string Replace([NotNull] this string str, [NotNull] string source, [NotNull] Func<string> target)
    {
      Assert.ArgumentNotNull(str, "str");
      Assert.ArgumentNotNull(source, "source");
      Assert.ArgumentNotNull(target, "target");

      if (str.Contains(source))
      {
        return str.Replace(source, target());
      }

      return str;
    }

    public static string ReplaceExtension(this string @this, string extension)
    {
      return Path.Combine(Path.GetDirectoryName(@this), 
        Path.GetFileNameWithoutExtension(@this) + '.' + extension.TrimStart('.'));
    }

    public static string SubstringEx(this string @this, int start, int end)
    {
      return end + start >= @this.Length ? @this.Substring(start) : @this.Substring(start, end);
    }

    [CanBeNull]
    public static TResult With<TInput, TResult>(this TInput @this, Func<TInput, TResult> evaluator)
      where TResult : class
      where TInput : class
    {
      if (@this == null)
      {
        return null;
      }

      return evaluator(@this);
    }

    #endregion
  }
}