#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SIM.Base;

#endregion

namespace SIM.Adapters.WebServer
{
  #region

  

  #endregion

  /// <summary>
  ///   The hosts.
  /// </summary>
  public static class Hosts
  {
    #region Constants

    /// <summary>
    ///   The exact regex pattern.
    /// </summary>
    private const string ExactRegexPattern = @"^\s*\d{{1,3}}\.\d{{1,3}}\.\d{{1,3}}\.\d{{1,3}}\s+({0})\s*$";

    #endregion

    #region Fields

    /// <summary>
    ///   The append pattern.
    /// </summary>
    private static readonly string AppendPattern = Environment.NewLine + "127.0.0.1\t{0}";

    #endregion

    #region Public Methods

    /// <summary>
    /// The append.
    /// </summary>
    /// <param name="hostName">
    /// The host name. 
    /// </param>
    public static void Append([NotNull] string hostName)
    {
      Assert.ArgumentNotNull(hostName, "hostName");

      string path = GetHostsFilePath();
      string[] lines = FileSystem.Local.File.ReadAllLines(path);

      const string DefaultRegexPattern = @"^\s*\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\s+(\S+)$";
      Regex regex = new Regex(DefaultRegexPattern);
      if (lines.All(line => !LineMatches(regex, line, hostName)))
      {
        SIM.Base.Log.Info("Appending host: {0}".FormatWith(hostName), typeof(Hosts));
        FileSystem.Local.File.AppendAllText(path, AppendPattern.FormatWith(hostName));
      }
    }

    /// <summary>
    /// The contains.
    /// </summary>
    /// <param name="hostName">
    /// The host name. 
    /// </param>
    /// <returns>
    /// The contains. 
    /// </returns>
    public static bool Contains([NotNull] string hostName)
    {
      Assert.ArgumentNotNullOrEmpty(hostName, "hostName");

      string path = GetHostsFilePath();
      string[] lines = FileSystem.Local.File.ReadAllLines(path);
      Regex regex = new Regex(ExactRegexPattern.FormatWith(Regex.Escape(hostName)));
      return lines.Any(regex.IsMatch);
    }

    /// <summary>
    /// The remove.
    /// </summary>
    /// <param name="hostNames">
    /// The host names. 
    /// </param>
    public static void Remove([NotNull] IEnumerable<string> hostNames)
    {
      Assert.ArgumentNotNull(hostNames, "hostNames");

      foreach (string hostName in hostNames)
      {
        SIM.Base.Log.Info("Removing host: {0}".FormatWith(hostName), typeof(Hosts));
        Remove(hostName);
      }
    }

    #endregion

    #region Methods

    private const char separator = '\t';

    /// <summary>
    ///   The get hosts file path.
    /// </summary>
    /// <returns> The get hosts file path. </returns>
    [NotNull]
    private static string GetHostsFilePath()
    {
      const string HostsPath = @"%WINDIR%\System32\drivers\etc\hosts";
      return Environment.ExpandEnvironmentVariables(HostsPath);
    }

    /// <summary>
    /// The line matches.
    /// </summary>
    /// <param name="regex">
    /// The regex. 
    /// </param>
    /// <param name="line">
    /// The line. 
    /// </param>
    /// <param name="hostName">
    /// The host name. 
    /// </param>
    /// <returns>
    /// The line matches. 
    /// </returns>
    private static bool LineMatches([NotNull] Regex regex, [NotNull] string line, [NotNull] string hostName)
    {
      Assert.ArgumentNotNull(regex, "regex");
      Assert.ArgumentNotNull(line, "line");
      Assert.ArgumentNotNull(hostName, "hostName");

      return regex.Match(line).Groups[1].Value.EqualsIgnoreCase(hostName);
    }

    /// <summary>
    /// The normalize line.
    /// </summary>
    /// <param name="line">
    /// The line. 
    /// </param>
    /// <returns>
    /// The normalize line. 
    /// </returns>
    [NotNull]
    private static string NormalizeLine([NotNull] string line)
    {
      Assert.ArgumentNotNull(line, "line");

      return line.Trim(' ', separator).Replace("  ", " ").Replace("  ", " ").Replace(' ', separator).Replace(separator.ToString() + separator, separator.ToString());
    }

    /// <summary>
    /// The remove.
    /// </summary>
    /// <param name="hostName">
    /// The host name. 
    /// </param>
    private static void Remove([NotNull] string hostName)
    {
      Assert.ArgumentNotNull(hostName, "hostName");

      string path = GetHostsFilePath();
      string[] lines = FileSystem.Local.File.ReadAllLines(path);
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

    public static IEnumerable<HostRecord> GetRecords()
    {
      string path = GetHostsFilePath();
      string[] lines = FileSystem.Local.File.ReadAllLines(path);
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
      string text = FileSystem.Local.File.ReadAllText(path);
      Log.Info("A backup of the hosts file" + Environment.NewLine + text, typeof(string));
      var sb = new StringBuilder();
      foreach (HostRecord hostRecord in records)
      {
        sb.AppendLine(hostRecord.ToString());
      }
      FileSystem.Local.File.WriteAllText(path, sb.ToString());
    }

    #region Nested type: HostRecord

    public class HostRecord
    {
      private static int _id;
      private string id;

      public HostRecord(string ip, string host = null) 
      {
        this.IP = ip;
        this.Host = host ?? string.Empty;
        this.id = _id++.ToString();
      }

      public string IP { get; set; }

      public string Host { get; set; }

      public string ID
      {
        get { return id; }
      }

      public override string ToString()
      {
        return this.IP + separator + this.Host;
      }
    }

    #endregion
  }
}