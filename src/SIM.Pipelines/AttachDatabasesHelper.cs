namespace SIM.Pipelines
{
  #region Usings

  using System;
  using System.Data.SqlClient;
  using System.IO;
  using System.IO.Compression;
  using System.Linq;
  using SIM.Adapters.SqlServer;
  using SIM.Adapters.WebServer;
  using SIM.Pipelines.Install;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;
  using SIM.FileSystem;
  using SIM.Instances;

  #endregion

  public static class AttachDatabasesHelper
  {
    // total number of steps in entire install/reinstall wizard without this one is around 5000, so we can assume attaching databases takes 5% so 250
    public const int StepsCount = 250;

    #region Public methods

    public static void AttachDatabase(string name, string sqlPrefix, bool attachSql, string databasesFolderPath, ConnectionString connectionString, SqlConnectionStringBuilder defaultConnectionString, IPipelineController controller)
    {
      SetConnectionStringNode(name, sqlPrefix, defaultConnectionString, connectionString);

      if (!attachSql)
      {
        return;
      }

      var databaseName = connectionString.GenerateDatabaseName(name, sqlPrefix);

      var databasePath =
        DatabaseFilenameHook(Path.Combine(databasesFolderPath, connectionString.DefaultFileName), 
          connectionString.Name.Replace("yafnet", "forum"), databasesFolderPath);

      if (!FileSystem.Local.File.Exists(databasePath))
      {
        var file = Path.GetFileName(databasePath);
        if (connectionString.Name.EqualsIgnoreCase("reporting"))
        {
          databasePath = Path.Combine(Path.GetDirectoryName(databasePath), "Sitecore.Analytics.mdf");
        } else if (connectionString.Name.EqualsIgnoreCase("exm.dispatch"))
        {
          databasePath = Path.Combine(Path.GetDirectoryName(databasePath), "Sitecore.Exm.mdf");
        }
        else if (connectionString.Name.EqualsIgnoreCase("session"))
        {
          databasePath = Path.Combine(Path.GetDirectoryName(databasePath), "Sitecore.Sessions.mdf");
        }
      }

      FileSystem.Local.File.AssertExists(databasePath, databasePath + " file doesn't exist");

      if (SqlServerManager.Instance.DatabaseExists(databaseName, defaultConnectionString))
      {
        databaseName = ResolveConflict(defaultConnectionString, connectionString, databasePath, databaseName, controller);
      }

      if (databaseName != null)
      {
        SqlServerManager.Instance.AttachDatabase(databaseName, databasePath, defaultConnectionString);
      }
    }

    public static void AttachDatabase(ConnectionString connectionString, SqlConnectionStringBuilder defaultConnectionString, string name, string sqlPrefix, bool attachSql, string databasesFolderPath, string instanceName, IPipelineController controller)
    {
      if (connectionString.IsMongoConnectionString)
      {
        connectionString.Value = AttachDatabasesHelper.GetMongoConnectionString(connectionString.Name, instanceName);
        connectionString.SaveChanges();
        return;
      }

      if (connectionString.IsSqlConnectionString)
      {
        try
        {
          AttachDatabasesHelper.AttachDatabase(name, sqlPrefix, attachSql, databasesFolderPath, connectionString, defaultConnectionString, controller);
        }
        catch (Exception ex)
        {
          if (connectionString.Name == "reporting.secondary")
          {
            throw;
          }

          Log.Warn(ex, "Attaching reporting.secondary database failed. Skipping...");
        }
      }
    }

    public static string GetSqlPrefix(string cstr1, string cstr2)
    {
      var one = new SqlConnectionStringBuilder(cstr1).InitialCatalog;
      var two = new SqlConnectionStringBuilder(cstr2).InitialCatalog;
      var min = Math.Min(one.Length, two.Length);
      var sqlPrefix = string.Empty;
      for (var i = 0; i < min; ++i)
      {
        if (one[i] == two[i])
        {
          sqlPrefix = one.Substring(0, i + 1);
        }
      }

      Assert.IsNotNull(sqlPrefix.EmptyToNull(), "two first database names have different prefixes when they must be similar");
      return sqlPrefix.Replace(".", "_").TrimEnd('_');
    }

    [NotNull]
    public static string DatabaseFilenameHook([NotNull] string databasePath, [NotNull] string databaseName, [NotNull] string databasesFolderPath)
    {
      Assert.ArgumentNotNull(databasePath, nameof(databasePath));
      Assert.ArgumentNotNull(databaseName, nameof(databaseName));

      if (!FileSystem.Local.File.Exists(databasePath))
      {
        string[] files = FileSystem.Local.Directory.GetFiles(databasesFolderPath, "*" + databaseName + ".mdf");
        var file = files.SingleOrDefault();
        if (!String.IsNullOrEmpty(file) && FileSystem.Local.File.Exists(file))
        {
          return file;
        }
      }

      return databasePath;
    }

    public static string ResolveConflict(SqlConnectionStringBuilder defaultConnectionString, ConnectionString connectionString, string databasePath, string databaseName, IPipelineController controller)
    {
      var existingDatabasePath = SqlServerManager.Instance.GetDatabaseFileName(databaseName, defaultConnectionString);

      if (String.IsNullOrEmpty(existingDatabasePath))
      {
        var m = "The database with the same '{0}' name is already exists in the SQL Server metabase but points to non-existing file. ".FormatWith(databaseName);
        if (controller.Confirm(m + "Would you like to delete it?"))
        {
          SqlServerManager.Instance.DeleteDatabase(databaseName, defaultConnectionString);
          return databaseName;
        }

        throw new Exception(m);
      }

      if (existingDatabasePath.EqualsIgnoreCase(databasePath))
      {
        return null;
      }

      // todo: replce this with shiny message box
      var delete = "Delete the '{0}' database".FormatWith(databaseName);
      const string AnotherName = "Use another database name";
      const string Cancel = "Terminate current action";
      string[] options = new[]
      {
        delete, AnotherName, Cancel
      };
      var m2 = "The database with '{0}' name already exists".FormatWith(databaseName);
      var result = controller.Select(m2, options);
      switch (result)
      {
        case Cancel:
          throw new Exception(m2);
        case AnotherName:
          databaseName = ResolveConflictByUnsedName(defaultConnectionString, connectionString, databaseName);
          break;
        default:
          SqlServerManager.Instance.DeleteDatabase(databaseName, defaultConnectionString);
          break;
      }

      return databaseName;
    }

    #endregion

    #region Private methods

    [NotNull]
    private static string GetMongoConnectionString([NotNull] string connectionStringName, [NotNull] string instanceName)
    {
      Assert.ArgumentNotNull(connectionStringName, nameof(connectionStringName));
      Assert.ArgumentNotNull(instanceName, nameof(instanceName));

      var mongoDbName = instanceName + "_" + connectionStringName;
      var invalidChars = new[]
      {
        '\0', ' ', '.', '$', '/', '\\'
      };
      foreach (var @char in invalidChars)
      {
        mongoDbName = mongoDbName.Replace(@char, "_"); // #SIM-128 Fixed
      }

      var value = Settings.AppMongoConnectionString.Value.TrimEnd('/') + @"/" + mongoDbName;
      return value;
    }

    [NotNull]
    private static string GetUnusedDatabaseName([NotNull] SqlConnectionStringBuilder defaultConnectionString, [NotNull] string databaseName)
    {
      Assert.ArgumentNotNull(defaultConnectionString, nameof(defaultConnectionString));
      Assert.ArgumentNotNull(databaseName, nameof(databaseName));

      const int K = 100;
      for (int i = 1; i <= K; i++)
      {
        if (!SqlServerManager.Instance.DatabaseExists(databaseName + '_' + i, defaultConnectionString))
        {
          return databaseName + "_" + i;
        }
      }

      throw new InvalidOperationException("Something weird happen... Do you really have '{0}' file?".FormatWith(databaseName + "_" + K));
    }

    [NotNull]
    private static string ResolveConflictByUnsedName([NotNull] SqlConnectionStringBuilder defaultConnectionString, [NotNull] ConnectionString connectionString, [NotNull] string databaseName)
    {
      Assert.ArgumentNotNull(defaultConnectionString, nameof(defaultConnectionString));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));
      Assert.ArgumentNotNull(databaseName, nameof(databaseName));

      databaseName = GetUnusedDatabaseName(defaultConnectionString, databaseName);
      connectionString.RealName = databaseName;
      connectionString.SaveChanges();
      return databaseName;
    }

    private static void SetConnectionStringNode([NotNull] string name, [NotNull] string sqlPrefix, [NotNull] SqlConnectionStringBuilder defaultConnectionString, [NotNull] ConnectionString connectionString)
    {
      Assert.ArgumentNotNull(name, nameof(name));
      Assert.ArgumentNotNull(sqlPrefix, nameof(sqlPrefix));
      Assert.ArgumentNotNull(defaultConnectionString, nameof(defaultConnectionString));
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(defaultConnectionString.ConnectionString)
      {
        InitialCatalog = connectionString.GenerateDatabaseName(name, sqlPrefix), 
        IntegratedSecurity = false
      };
      connectionString.Value = builder.ToString();
      connectionString.SaveChanges();
    }

    #endregion

    public static string GetSqlPrefix(Instance instance)
    {
      var connectionStrings = instance.Configuration.ConnectionStrings.Where(x => x.IsSqlConnectionString).ToArray();
      Assert.IsTrue(connectionStrings.Length >= 2, "2 or more sql connection strings are required");

      return AttachDatabasesHelper.GetSqlPrefix(connectionStrings[0].Value, connectionStrings[1].Value);
    }

    public static void ExtractReportingDatabase(Instance instance, FileInfo destination)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));
      Assert.ArgumentNotNull(destination, nameof(destination));

      var product = instance.Product;
      Assert.IsNotNull(product.PackagePath.EmptyToNull(), "The {0} product distributive is not available in local repository", instance.ProductFullName);

      var package = new FileInfo(product.PackagePath);
      Assert.IsTrue(package.Exists, $"The {package.FullName} file does not exist");

      using (var zip = ZipFile.OpenRead(package.FullName))
      {
        var entry = zip.Entries.FirstOrDefault(x => x.FullName.EndsWith("Sitecore.Analytics.mdf"));
        Assert.IsNotNull(entry, "Cannot find Sitecore.Analytics.mdf in the {0} file", package.FullName);

        entry.ExtractToFile(destination.FullName);
      }
    }
  }
}