namespace SIM.Extensions
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;

  public static class StringExtensions
  {                                  
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
      var end = -1;
      var length = message.Length;
      while (true)
      {
        var start = message.IndexOf(startChar, end + 1);
        if (start < 0)
        {
          yield break;
        }

        Assert.IsTrue(start + 1 < length, $"Cannot replace variables in the \"{message}\" string - line unexpectedly ends after {start} position");
        end = message.IndexOf(endChar, start + 1);
        Assert.IsTrue(end > start, $"Cannot replace variables in the \"{message}\" string - no closing {end} character after {start} position");
        start += includeBounds ? 0 : 1;
        var len = end - start + (includeBounds ? 1 : 0);
        Assert.IsTrue(len > 0, $"Cannot replace variables in the \"{message}\" string - the string contains invalid '{{}}' statement");
        yield return message.Substring(start, len);
      }
    }

    [NotNull]
    public static string FormatWith([CanBeNull] this string format, [CanBeNull] params object[] parameters)
    {
      return parameters == null ? format ?? string.Empty : string.Format(format ?? string.Empty, parameters);
    }

    public static bool IsNullOrEmpty([CanBeNull] this string @string)
    {
      return string.IsNullOrEmpty(@string);
    }

    [NotNull]
    public static IEnumerable<string> Split([NotNull] this string source, [NotNull] string delimiter, bool skipEmpty = true)
    {
      Assert.ArgumentNotNullOrEmpty(source, nameof(source));
      Assert.ArgumentNotNullOrEmpty(delimiter, nameof(delimiter));

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
      Assert.ArgumentNotNull(source, nameof(source));
      Assert.ArgumentNotNull(value, nameof(value));

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
      Assert.ArgumentNotNull(source, nameof(source));
      Assert.ArgumentNotNull(value, nameof(value));

      return source.StartsWith(value) ? source.Substring(value.Length) : source;
    }

    public static string EnsureEnd(this string @this, string end)
    {
      return @this.TrimEnd(end) + end;
    }

    // String extensions
    public static string PathCombine(this string @this, string combineWith)
    {
      return Path.Combine(@this, combineWith);
    }

    public static string Replace(this string @this, char source, string target)
    {
      return @this.Replace(source.ToString(), target);
    }

    [NotNull]
    public static string Replace([NotNull] this string str, [NotNull] string source, [NotNull] Func<string> target)
    {
      Assert.ArgumentNotNull(str, nameof(str));
      Assert.ArgumentNotNull(source, nameof(source));
      Assert.ArgumentNotNull(target, nameof(target));

      if (str.Contains(source))
      {
        return str.Replace(source, target());
      }

      return str;
    }

    public static string SubstringEx(this string @this, int start, int end)
    {
      return end + start >= @this.Length ? @this.Substring(start) : @this.Substring(start, end);
    }
  }
}
