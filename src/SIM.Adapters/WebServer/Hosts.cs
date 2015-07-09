namespace SIM.Adapters.WebServer
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  public static class Hosts
  {
    #region Constants

    private const string ExactRegexPattern = @"^\s*\d{{1,3}}\.\d{{1,3}}\.\d{{1,3}}\.\d{{1,3}}\s+({0})\s*$";

    #endregion

    #region Fields

    private static readonly string AppendPattern = Environment.NewLine + "127.0.0.1\t{0}";

    #endregion

    #region Public Methods

    public static void Append([NotNull] string hostName)
    {
      Assert.ArgumentNotNull(hostName, "hostName");

      string path = GetHostsFilePath();
      string[] lines = FileSystem.FileSystem.Local.File.ReadAllLines(path);

      const string DefaultRegexPattern = @"^\s*\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\s+(\S+)$";
      Regex regex = new Regex(DefaultRegexPattern);
      if (lines.All(line => !LineMatches(regex, line, hostName)))
      {
        Log.Info("Appending host: {0}".FormatWith(hostName), typeof(Hosts));
        FileSystem.FileSystem.Local.File.AppendAllText(path, AppendPattern.FormatWith(hostName));
      }
    }

    public static bool Contains([NotNull] string hostName)
    {
      Assert.ArgumentNotNullOrEmpty(hostName, "hostName");

      string path = GetHostsFilePath();
      string[] lines = FileSystem.FileSystem.Local.File.ReadAllLines(path);
      Regex regex = new Regex(ExactRegexPattern.FormatWith(Regex.Escape(hostName)));
      return lines.Any(regex.IsMatch);
    }

    public static void Remove([NotNull] IEnumerable<string> hostNames)
    {
      Assert.ArgumentNotNull(hostNames, "hostNames");

      foreach (string hostName in hostNames)
      {
        Log.Info("Removing host: {0}".FormatWith(hostName), typeof(Hosts));
        Remove(hostName);
      }
    }

    #endregion

    #region Methods

    #region Constants

    private const char separator = '\t';

    #endregion

    #region Private methods

    [NotNull]
    private static string GetHostsFilePath()
    {
      const string HostsPath = @"%WINDIR%\System32\drivers\etc\hosts";
      return Environment.ExpandEnvironmentVariables(HostsPath);
    }

    private static bool LineMatches([NotNull] Regex regex, [NotNull] string line, [NotNull] string hostName)
    {
      Assert.ArgumentNotNull(regex, "regex");
      Assert.ArgumentNotNull(line, "line");
      Assert.ArgumentNotNull(hostName, "hostName");

      return regex.Match(line).Groups[1].Value.EqualsIgnoreCase(hostName);
    }

    [NotNull]
    private static string NormalizeLine([NotNull] string line)
    {
      Assert.ArgumentNotNull(line, "line");

      return line.Trim(' ', separator).Replace("  ", " ").Replace("  ", " ").Replace(' ', separator).Replace(separator.ToString() + separator, separator.ToString());
    }

    private static void Remove([NotNull] string hostName)
    {
      Assert.ArgumentNotNull(hostName, "hostName");

      string path = GetHostsFilePath();
      string[] lines = FileSystem.FileSystem.Local.File.ReadAllLines(path);
      Regex regex = new Regex(ExactRegexPattern.FormatWith(Regex.Escape(hostName)));
      using (StreamWriter writer = new StreamWriter(path, false))
      {
        foreach (string line in lines)
        {
          string record = NormalizeLine(line);
          if (!string.IsNullOrEmpty(record) && !regex.IsMatch(record))
          {
            writer.WriteLine(line);
          }
        }
      }
    }

    #endregion

    #endregion

    #region Public methods

    public static IEnumerable<HostRecord> GetRecords()
    {
      string path = GetHostsFilePath();
      string[] lines = FileSystem.FileSystem.Local.File.ReadAllLines(path);
      foreach (string line in lines)
      {
        string record = NormalizeLine(line);
        if (!string.IsNullOrEmpty(record) && record[0] != '#')
        {
          var r = record.Split(separator);
          yield return new HostRecord(r[0], r[1]);
        }
      }
    }

    public static void Save(IEnumerable<HostRecord> records)
    {
      string path = GetHostsFilePath();
      string text = FileSystem.FileSystem.Local.File.ReadAllText(path);
      Log.Info("A backup of the hosts file" + Environment.NewLine + text, typeof(string));
      var sb = new StringBuilder();
      foreach (HostRecord hostRecord in records)
      {
        sb.AppendLine(hostRecord.ToString());
      }

      FileSystem.FileSystem.Local.File.WriteAllText(path, sb.ToString());
    }

    #endregion

    #region Nested type: HostRecord

    public class HostRecord
    {
      #region Fields

      private static int _id;
      private string id;

      #endregion

      #region Constructors

      public HostRecord(string ip, string host = null)
      {
        this.IP = ip;
        this.Host = host ?? string.Empty;
        this.id = _id++.ToString();
      }

      #endregion

      #region Public properties

      public string Host { get; set; }

      public string ID
      {
        get
        {
          return this.id;
        }
      }

      public string IP { get; set; }

      #endregion

      #region Public methods

      public override string ToString()
      {
        return this.IP + separator + this.Host;
      }

      #endregion
    }

    #endregion
  }
}