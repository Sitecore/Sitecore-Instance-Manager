using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class SqlPefixValidator : IValidator
  {
    public SqlPefixValidator()
    {
      this.Data = new Dictionary<string, string>();
    }
    public Dictionary<string, string> Data { get; set; }

    public IEnumerable<ValidationResult> Evaluate(IEnumerable<Tasks.Task> tasks)
    {
      string server = this.Data["Server"];
      string user = this.Data["User"];
      string pass = this.Data["Password"];
      string prefix = this.Data["Prefix"];
      List<Tuple<string, string, string, string>> servers = new List<Tuple<string, string, string, string>>();
      foreach (Task task in tasks.Where(t => t.LocalParams.Any(p => p.Name == user) && t.LocalParams.Any(p => p.Name == prefix)))
      {
        string serverValue = task.LocalParams.First(p => p.Name == server).Value;
        string userValue = task.LocalParams.First(p => p.Name == user).Value;
        string passValue = task.LocalParams.First(p => p.Name == pass).Value;
        string prefixValue = task.LocalParams.First(p => p.Name == prefix).Value;
        if (!servers.Any(s => s.Item1 == serverValue && s.Item2 == userValue && s.Item3 == passValue && s.Item4 == prefixValue))
        {
          servers.Add(new Tuple<string, string, string, string>(serverValue, userValue, passValue, prefixValue));
        }
      }

      foreach(Tuple<string, string, string, string> set in servers)
      {
        if (this.GetDbList(set.Item1, set.Item2, set.Item3).Any(name => name.StartsWith(set.Item4, StringComparison.InvariantCultureIgnoreCase)))
        {
          yield return new ValidationResult(ValidatorState.Error, $"Database with prefix {set.Item4} already exists on server {set.Item1}",null);
        }
      }

      yield return new ValidationResult(ValidatorState.Success, string.Empty, null);
    }
    
    protected internal virtual IEnumerable<string> GetDbList(string server, string user, string password)
    {
      string connectionstring = $"Data Source={server};" +
                         $"Initial Catalog=master;User ID={user};" +
                         $"Password={password}";
      SqlConnection conn = new SqlConnection(connectionstring);
      try
      {
        conn.Open();
        SqlCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT name FROM master.dbo.sysdatabases";
        SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
          yield return (string)reader["name"];
        }
      }
      finally
      {
        conn.Close();
      }
    }
  }
}
