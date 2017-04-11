namespace SIM.Core.Common
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;
  using SIM.Extensions;

  public sealed class CustomException
  {
    private static Regex StripSourceFileRegex { get; } = new Regex(@"^(.+) in (\\\\[^\\]+\\\\)?[^\\]+\\\\?.+\:line \d+$", RegexOptions.Compiled);

    public CustomException(Exception ex)
    {
      ClassName = ex.GetType().FullName;
      Message = ex.Message;
      Data = ex.Data;
      StackTrace = ex.StackTrace?.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => StripSourceFile(x).Trim()) ?? new string[0];
      InnerException = ex.InnerException.With(x => new CustomException(x));
    }

    public string ClassName { get; private set; }

    public string Message { get; private set; }

    public IDictionary Data { get; private set; }

    public IEnumerable<string> StackTrace { get; private set; }

    public CustomException InnerException { get; private set; }

    private string StripSourceFile(string line)
    {
      if (string.IsNullOrEmpty(line))
      {
        return string.Empty;
      }

      var match = StripSourceFileRegex.Match(line);
      return !match.Success ? line : match.Groups[1].Value;
    }
  }
}