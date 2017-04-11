namespace SIM.Adapters.WebServer
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  #region

  #endregion

  public static class Hosts
  {
    #region Fields

    private static string AppendPattern { get; } = Environment.NewLine + "127.0.0.1\t{0}";

    #endregion

    #region Public Methods

    public static void Append([NotNull] string hostName)
    {
      Assert.ArgumentNotNull(hostName, nameof(hostName));

      var path = GetHostsFilePath();
      string[] lines = FileSystem.FileSystem.Local.File.ReadAllLines(path);

      Log.Info($"Appending host: {hostName}");
      if (lines.Any(line => Matches(hostName, line)))
      {
        Log.Info("Host already exists");
        return;
      }
      
      FileSystem.FileSystem.Local.File.AppendAllText(path, AppendPattern.FormatWith(hostName));
    }

    private static bool Matches(string hostName, string line)
    {
      return (line + " ").Replace("\t", " ").Contains(" " + hostName + " ");
    }
    
    public static void Remove([NotNull] IEnumerable<string> hostNames)
    {
      Assert.ArgumentNotNull(hostNames, nameof(hostNames));

      foreach (string hostName in hostNames)
      {
        Log.Info($"Removing host: {hostName}");
        Remove(hostName);
      }
    }

    #endregion

    #region Methods

    #region Private methods

    [NotNull]
    private static string GetHostsFilePath()
    {
      const string HostsPath = @"%WINDIR%\System32\drivers\etc\hosts";
      return Environment.ExpandEnvironmentVariables(HostsPath);
    }
    
    private static void Remove([NotNull] string hostName)
    {
      Assert.ArgumentNotNull(hostName, nameof(hostName));

      var path = GetHostsFilePath();
      var records = GetRecords().ToArray();
      using (StreamWriter writer = new StreamWriter(path, false))
      {
        foreach (var record in records)
        {
          var hostRecord = record as IpHostRecord;
          if (hostRecord == null || !hostRecord.Host.EqualsIgnoreCase(hostName))
          {
            writer.WriteLine(record);
          }
        }
      }
    }

    #endregion

    #endregion

    #region Public methods

    public static IEnumerable<IHostRecord> GetRecords()
    {
      var path = GetHostsFilePath();
      string[] lines = FileSystem.FileSystem.Local.File.ReadAllLines(path);
      foreach (string line in lines)
      {
        if (!string.IsNullOrEmpty(line))
        {
          if (line[0] != '#')
          {
            var arr = line.Split(" \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var arrLength = arr.Length;
            if (arrLength < 2)
            {
              continue;
            }

            var ip = arr[0];
            for (var i = 1; i < arrLength; ++i)
            {
              yield return new IpHostRecord(ip, arr[i]);
            }
          }
          else
          {
            yield return new CommentHostRecord(line);
          }
        }
      }
    }

    public class CommentHostRecord : IHostRecord
    {
      private string Line { get; }

      public CommentHostRecord(string line)
      {
        this.Line = line;
      }

      public override string ToString()
      {
        return this.Line;
      }
    }

    public static void Save(IEnumerable<IHostRecord> records)
    {
      var path = GetHostsFilePath();
      var text = FileSystem.FileSystem.Local.File.ReadAllText(path);
      Log.Info($"A backup of the hosts file\r\n{text}");
      var sb = new StringBuilder();
      foreach (IHostRecord hostRecord in records)
      {
        sb.AppendLine(hostRecord.ToString());
      }

      FileSystem.FileSystem.Local.File.WriteAllText(path, sb.ToString());
    }

    #endregion

    #region Nested type: HostRecord

    public class IpHostRecord : IHostRecord
    {
      #region Fields

      private static int NextId { get; set; }

      #endregion

      #region Constructors

      public IpHostRecord(string ip, string host = null)
      {
        this.IP = ip;
        this.Host = host ?? string.Empty;
        this.ID = NextId++.ToString();
      }

      #endregion

      #region Public properties

      public string Host { get; set; }

      public string ID { get; }

      public string IP { get; set; }

      #endregion

      #region Public methods

      public override string ToString()
      {
        return this.IP + '\t' + this.Host;
      }

      #endregion
    }

    public interface IHostRecord
    {
    }

    #endregion
  }
}