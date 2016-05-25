namespace SIM.Core.Common
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;
  using System.Threading;

  public sealed class CustomException
  {
    public CustomException(Exception ex)
    {
      this.ClassName = ex.GetType().FullName;
      this.Message = ex.Message;
      this.Data = ex.Data;
      this.StackTrace = ex.StackTrace.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => StripSourceFile(x).Trim());
      this.InnerException = ex.InnerException.With(x => new CustomException(x));
    }

    private static readonly Regex StripSourceFileRegex = new Regex(@"^(.+) in (\\\\[^\\]+\\\\)?[^\\]+\\\\?.+\:line \d+$", RegexOptions.Compiled);

    private string StripSourceFile(string line)
    {
      if (string.IsNullOrEmpty(line))
      {
        return string.Empty;
      }

      var match = StripSourceFileRegex.Match(line);
      return !match.Success ? line : match.Groups[1].Value;
    }

    public string ClassName { get; private set; }

    public string Message { get; private set; }

    public IDictionary Data { get; private set; }

    public IEnumerable<string> StackTrace { get; private set; }

    public CustomException InnerException { get; private set; }
  }
}