using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Task = SIM.Sitecore9Installer.Tasks.Task;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class SqlVersionValidator:IValidator
  {
    public IEnumerable<ValidationResult> Evaluate(IEnumerable<Task> tasks)
    {
      string server = this.Data["Server"];
      string user = this.Data["User"];
      string pass = this.Data["Password"];
      foreach (Task task in tasks.Where(t => t.LocalParams.Any(p => p.Name == user)))
      {
        SqlConnection conn =
          new SqlConnection($"Data Source={task.LocalParams.Single(p=>p.Name==server).Value};" +
                            $"Initial Catalog=master;User ID={task.LocalParams.Single(p => p.Name == user).Value};" +
                            $"Password={task.LocalParams.Single(p => p.Name == pass).Value}");
        conn.Open();
        try
        {
          string[] versions= Data["Versions"].Split(',');
          if (!versions.Any(v => Regex.Match(conn.ServerVersion, v).Success))
          {
            yield return new ValidationResult(ValidatorState.Error, "SQL server version is not compatible", null);
          }
        }
        finally
        {
          conn.Close();
        }
      }

      yield return new ValidationResult(ValidatorState.Success, null, null);
    }

    public Dictionary<string, string> Data { get; set; }
  }
}
