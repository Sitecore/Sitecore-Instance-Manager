namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.Data.SqlClient;
  using System.IO;
  using System.IO.Compression;
  using System.Linq;
  using System.Windows;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Adapters.SqlServer;
  using SIM.Core.Common;
  using SIM.Extensions;
  using SIM.Instances;
  using SIM.Pipelines;
  using SIM.Tool.Base.Plugins;

  public class AttachReportingSecondaryDatabaseButton : IMainWindowButton
  {
    [UsedImplicitly]
    public AttachReportingSecondaryDatabaseButton()
    {
    }

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));

      var database = instance.AttachedDatabases?.FirstOrDefault();
      Assert.IsNotNull(database, "The databases information is not available");

      var databaseFilePath = database.FileName.EmptyToNull();
      Assert.IsNotNull(databaseFilePath, nameof(databaseFilePath));

      var databasesFolderPath = Path.GetDirectoryName(databaseFilePath);
      Assert.IsNotNull(databasesFolderPath, nameof(databasesFolderPath));

      var primaryCstr = instance.Configuration.ConnectionStrings.FirstOrDefault(x => x.Name == "reporting");
      Assert.IsNotNull(primaryCstr, "reporting connection string is missing");

      var primarySqlName = new SqlConnectionStringBuilder(primaryCstr.Value).InitialCatalog;
      var primaryDatabase = instance.AttachedDatabases.FirstOrDefault(x => x.RealName.Equals(primarySqlName));
      Assert.IsNotNull(primaryDatabase, nameof(primaryDatabase));

      var primaryFile = new FileInfo(primaryDatabase.FileName);
      Assert.IsTrue(primaryFile.Exists, "The {0} reporting database file does not exist", primaryFile.FullName);

      var secondaryCstr = instance.Configuration.ConnectionStrings.FirstOrDefault(x => x.Name == "reporting.secondary");
      var mgmtCstr = new SqlConnectionStringBuilder(Profile.Read().ConnectionString);
      if (secondaryCstr != null)
      {
        var name = new SqlConnectionStringBuilder(secondaryCstr.Value).InitialCatalog;
        if (SqlServerManager.Instance.DatabaseExists(name, mgmtCstr))
        {
          SqlServerManager.Instance.DeleteDatabase(name, mgmtCstr);
        }

        secondaryCstr.Delete();
      }

      var primaryIsSecondary = primaryFile.Name.EndsWith("secondary.mdf", StringComparison.OrdinalIgnoreCase);
      var secondaryFile = new FileInfo(Path.Combine(databasesFolderPath, primaryIsSecondary ? "Sitecore.Reporting.mdf" : "Sitecore.Reporting.Secondary.mdf"));
      if (secondaryFile.Exists)
      {
        secondaryFile.Delete();
      }
                                   
      ExtractReportingDatabase(instance, secondaryFile);      

      var sqlPrefix = AttachDatabasesHelper.GetSqlPrefix(instance);
      var sqlName = sqlPrefix + "_reporting";
      if (sqlName.EqualsIgnoreCase(primarySqlName))
      {
        sqlName += "_secondary";
      }

      SqlServerManager.Instance.AttachDatabase(sqlName, secondaryFile.FullName, mgmtCstr);
      var secondaryBuilder = new SqlConnectionStringBuilder(mgmtCstr.ToString())
      {
        InitialCatalog = sqlName
      };

      instance.Configuration.ConnectionStrings.Add("reporting.secondary", secondaryBuilder);
    }

    private static void ExtractReportingDatabase(Instance instance, FileInfo reportingSecondary)
    {
      var product = instance.Product;
      var package = new FileInfo(product.PackagePath);
      Assert.IsTrue(package.Exists, $"The {package.FullName} file does not exist");

      using (var zip = ZipFile.OpenRead(package.FullName))
      {
        var entry = zip.Entries.FirstOrDefault(x => x.FullName.EndsWith("Sitecore.Analytics.mdf"));
        Assert.IsNotNull(entry, "Cannot find Sitecore.Analytics.mdf in the {0} file", package.FullName);

        entry.ExtractToFile(reportingSecondary.FullName);
      }
    }
  }
}
