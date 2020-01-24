using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SitecoreInstaller.Validation.Abstractions;

namespace SitecoreInstaller.Validation.SQL
{
  public class SqlDbChecker : IInstallationValidator
  {
    public SqlDbChecker()
    {
      Name = "SqlDbChecker";
    }

    private List<string> GetAllDbs(string connectionString)
    {
      List<string> databases = new List<string>();
      using (SqlConnection sql = new SqlConnection(connectionString))
      {
        try
        {
          SqlCommand getAllDbs = new SqlCommand("SELECT name FROM sys.databases", sql);
          sql.Open();
          using (IDataReader rd = getAllDbs.ExecuteReader())
          {
            while (rd.Read())
            {
              databases.Add(rd[0].ToString());
            }
          }
          sql.Close();
        }
        catch (SqlException e)
        {
          this.Details += " SQL connection exception occurred";
        }
        
      }

      return databases;
    }

    public bool DbExists(string name, string connectionString)
    {
      List<string> databases = GetAllDbs(connectionString);
      return databases.Any(x => x.Equals(name, StringComparison.InvariantCultureIgnoreCase));

    }

    public string Name { get; set; }

    public ValidationResult Result
    {
      get;
      private set;
    }

    public ValidationResult Validate(Dictionary<string, string> installParams)
    {
      this.Result = ValidationResult.Ok;
      Dictionary<string, string> dbNames = new Dictionary<string, string>
      {
        {"Sql.Database.Core", installParams["SqlDbPrefix"] + "_Core"},
        {"Sql.Database.Master", installParams["SqlDbPrefix"] + "_Master"},
        {"Sql.Database.Web", installParams["SqlDbPrefix"] + "_Web"},
        {"Sql.Database.Reporting", installParams["SqlDbPrefix"] + "_Reporting"},
        {"Sql.Database.Reference", installParams["SqlDbPrefix"] + "_ReferenceData"},
        {"Sql.Database.Forms", installParams["SqlDbPrefix"] + "_ExperienceForms"},
        {"Sql.Database.Pools", installParams["SqlDbPrefix"] + "_Processing.Pools"},
        {"Sql.Database.Tasks", installParams["SqlDbPrefix"] + "_Processing.Tasks"},
        {"Sql.Database.MarketingAutomation", installParams["SqlDbPrefix"] + "_MarketingAutomation"},
        {"Sql.Database.EXM.Master", installParams["SqlDbPrefix"] + "_EXM.Master"},
        {"Sql.Database.Messaging", installParams["SqlDbPrefix"] + "_Messaging"},
        { "Sql.Database.ShardMapManager", installParams["SqlDbPrefix"] + "_Xdb.Collection.ShardMapManager"},
        { "Sql.Database.Shard0", installParams["SqlDbPrefix"] + "_Xdb.Collection.Shard0"},
        { "Sql.Database.Shard1", installParams["SqlDbPrefix"] + "_Xdb.Collection.Shard1"}
      };
      string connString = "Data Source="+installParams["SqlServer"] +";User ID=" +
                          installParams["SqlAdminUser"] + ";Password=" + installParams["SqlAdminPassword"];
      List<string> dbList = GetAllDbs(connString);
      foreach (var dbName in dbNames)
      {
        if (dbList.Any(x => x.Equals(dbName.Value, StringComparison.InvariantCultureIgnoreCase)))
        {
          this.Details += dbName.Value + " already exists;";
          this.Result = ValidationResult.Error;
        }
      }

      return this.Result;
    }

    public ValidationResult Validate()
    {
      throw new NotImplementedException();
    }

    public string Details { get ;
       set;
    }

    public ValidationResult Validate(string siteName)
    {
      return ValidationResult.Warning;
    }
  }
}
